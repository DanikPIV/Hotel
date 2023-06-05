using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        AppContext appContext;

        public MainWindow()
        {
            InitializeComponent();

            appContext = new AppContext();
        }

        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {

            string root = root1.Text.Trim();
            string login = Login.Text.Trim();
            string pass = Password.Password.Trim();
            string pass2 = Password_2.Password.Trim();


            if (pass.Length >= 8 && pass == pass2 && login.Length >= 4)
            {
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
                    User user = new User(root, login, pass);
                    appContext.Users.Add(user);
                    appContext.SaveChanges();
                    MessageBox.Show("Пользователь добавлен");
                    Close();
                }
            }
            else { MessageBox.Show("Выполните все условия"); }


        }

        private void Login_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Login.Text.Length < 4)
            {
                Login.ToolTip = "Не менее 4 символов";
                Login.Foreground = Brushes.Red;
            }
            else
            {
                Login.Foreground = Brushes.Black;
            }
        }

        private void Password_TextChanged(object sender, RoutedEventArgs e)
        {

            if (Password.Password.Length < 8)
            {
                Password.ToolTip = "Не менее 8 символов";
                Password.Foreground = Brushes.Red;
            }
            else
            {
                Password.Foreground = Brushes.Black;
            }

        }

        private void Password_TextChanged2(object sender, RoutedEventArgs e)
        {
            if (Password_2.Password.Length < 8)
            {
                Password_2.ToolTip = "Не менее 8 символов";

                Password_2.Foreground = Brushes.Red;
            }
            else
            {
                Password_2.Foreground = Brushes.Black;
            }
            if (Password.Password != Password_2.Password)
            {
                Password_2.ToolTip = "Пароли не совпадают";

                Password_2.Foreground = Brushes.Red;
                Password.Foreground = Brushes.Red;
            }
            else
            {
                Password_2.Foreground = Brushes.Black;
                Password.Foreground = Brushes.Black;
            }
        }
    }
}
