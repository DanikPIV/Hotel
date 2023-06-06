using System;
using System.Data;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для account_control_Window.xaml
    /// </summary>
    public partial class account_control_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT id, login AS \"Логин\", root AS \"Права\" FROM  users";
        string id = null;
        string login = null;

        public account_control_Window()
        {
            InitializeComponent();
            refresh_table();
        }
        public void refresh_table()
        {
            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = dataTable.DefaultView;
            ClearTxt();
            sqlConnection.Close();
        }

        public void ClearTxt()
        {
            txt_login.Text = "";
            comboBox_root.Text = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
            refresh_table();
        }

        private void delButton_Click(object sender, RoutedEventArgs e)
        {

            if (id == null)
                MessageBox.Show("Выберите пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Удалить пользователя {login}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM users WHERE id = \"{id}\"", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                            MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        id = null;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (id == null)
                MessageBox.Show("Выберите пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (txt_login.Text.Length == 0 || comboBox_root.Text.Length == 0)
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Редактировать данные {login}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        string pass = txt_password.Password.Trim();

                        if (pass.Length >= 8)
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
                            }

                            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                            {
                                connection.Open();
                                SQLiteCommand command = new SQLiteCommand("UPDATE users SET login = \"" + txt_login.Text + "\", password = \"" + pass + "\", root = '" + comboBox_root.Text + "' WHERE id = @id", connection);
                                command.Parameters.AddWithValue("@id", id);
                                command.ExecuteNonQuery();

                                MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                refresh_table();
                            }
                        }
                        else
                            MessageBox.Show("Слишком короткий пароль");

                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                }
            }

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (dataGrid.Items.Count != 0 && selectedRow != null)
            {
                txt_login.Text = Convert.ToString(selectedRow["Логин"]);
                comboBox_root.Text = Convert.ToString(selectedRow["Права"]);
                login = Convert.ToString(selectedRow["Логин"]);
                id = Convert.ToString(selectedRow["id"]);
            }
        }
    }
}
