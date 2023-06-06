using System;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Button_Auth_Click(object sender, RoutedEventArgs e)
        {

            string login = Login.Text.Trim();
            string pass = Password.Password.Trim();


            // Строим объект хэш-алгоритма SHA256
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                // Преобразуем пароль в массив байтов
                byte[] passwordBytes = Encoding.UTF8.GetBytes(pass);

                // Вычисляем хэш
                byte[] hashedBytes = sha256.ComputeHash(passwordBytes);

                // Преобразуем хэш в строку в шестнадцатеричном формате
                string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "");

                // Выводим зашифрованный пароль
                pass = hashedString;
            }


            User authUser = null;
            using (AppContext appContext = new AppContext())
            {
                authUser = appContext.Users.Where(b => b.login == login && b.password == pass).FirstOrDefault();
            }
            if (authUser != null)
            {
                MainWin mainWin = new MainWin();
                try
                {
                    SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
                    using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                    {
                        connection.Open();
                        SQLiteCommand command = new SQLiteCommand("UPDATE users SET current = 1 WHERE login = '" + login + "'", connection);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Добро пожаловать!");
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                mainWin.Show();

                Close();
            }

            else
                MessageBox.Show("Неварный логин или пароль!");

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=hotel.db");
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM users", conn);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();

            Hide();

            if (count == 0)
            {
                MainWindow reg = new MainWindow();
                reg.ShowDialog();
                Show();
            }
            else
            {
                // Открываем окно авторизации
                Show();

                conn.Close();

            }
        }
    }

}
