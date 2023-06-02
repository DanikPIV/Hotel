using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Status_Clients_Window.xaml
    /// </summary>
    public partial class Status_Clients_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT status_clients.status AS \"Статус\", description AS \"Описание\" FROM  status_clients";
        string status = null;

        public Status_Clients_Window()
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
            txt_status.Text = "";
            txt_description.Text = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWin mainWin = new MainWin();
            mainWin.Show();
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_status.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "INSERT INTO status_clients (status, description) VALUES (\"" + txt_status.Text + "\",  \"" + txt_description.Text + "\")";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                refresh_table();
            }
        }

        private void delButton_Click(object sender, RoutedEventArgs e)
        {

            if (status == null)
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Удалить запись {status}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM status_clients WHERE status = \"{status}\"", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                            MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        status = null;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_status.Text.Length == 0)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (status == null)
                    MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    if (MessageBox.Show($"Редактировать запись {status}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                            {
                                connection.Open();
                                SQLiteCommand command = new SQLiteCommand("UPDATE status_clients SET status = \"" + txt_status.Text + "\", description = \"" + txt_description.Text + "\" WHERE status = @status", connection);
                                command.Parameters.AddWithValue("@status", status);
                                command.ExecuteNonQuery();

                                MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                refresh_table();
                            }
                            status = null;
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                    }
                }
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (dataGrid.Items.Count != 0 && selectedRow != null)
            {
                txt_status.Text = Convert.ToString(selectedRow["Статус"]);
                txt_description.Text = Convert.ToString(selectedRow["Описание"]);

                status = Convert.ToString(selectedRow["Статус"]);
            }
        }
    }
}
