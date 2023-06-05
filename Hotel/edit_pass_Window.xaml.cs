using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для edit_pass_Window.xaml
    /// </summary>
    public partial class edit_pass_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        public edit_pass_Window()
        {
            InitializeComponent();
            try
            {
                string connectionString = "Data Source = hotel.db; Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT login FROM users WHERE current = 1";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string login = reader.GetString(0);
                                resultTextBlock.Text = "Смена пароля для: " + login;
                            }
                        }
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {

            string opass = oldPassword.Password.Trim();
            string pass = Password.Password.Trim();
            string pass2 = Password_2.Password.Trim();


            if (pass.Length >= 8 && pass == pass2)
            {
                // Строим объект хэш-алгоритма SHA256
                using (SHA256Managed sha256 = new SHA256Managed())
                {
                    // Преобразуем пароль в массив байтов
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(pass);
                    byte[] passwordBytes1 = Encoding.UTF8.GetBytes(opass);

                    // Вычисляем хэш
                    byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
                    byte[] hashedBytes1 = sha256.ComputeHash(passwordBytes);

                    // Преобразуем хэш в строку в шестнадцатеричном формате
                    string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "");
                    string hashedString1 = BitConverter.ToString(hashedBytes).Replace("-", "");

                    // Выводим зашифрованный пароль
                    pass = hashedString;
                    opass = hashedString1;

                    string oldpass = null;
                    string connectionString = "Data Source = hotel.db; Version=3;";
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = "SELECT password FROM users WHERE current = 1";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string login = reader.GetString(0);
                                    oldpass = login;
                                }
                            }
                        }   
                    }

                    if (opass == oldpass)
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                            {
                                connection.Open();
                                SQLiteCommand command = new SQLiteCommand("UPDATE users SET password = '"+pass+"' WHERE current = 1", connection);
                                command.ExecuteNonQuery();
                                MessageBox.Show("Пароль изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                Close();
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                    }
                }
            }
            else { MessageBox.Show("Выполните все условия"); }


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

        private void Button_censel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
