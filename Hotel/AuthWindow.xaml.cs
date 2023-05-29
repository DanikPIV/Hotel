using System;
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
                MessageBox.Show("Вы авторизованы!");
                MainWin mainWin = new MainWin();
                mainWin.Show();
                Hide();
            }

            else
                MessageBox.Show("Неварный логин или пароль!");

        }

    }
}
