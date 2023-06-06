using System;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для services_Window.xaml
    /// </summary>
    public partial class Services_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT service AS 'Услуга', CAST(price AS TEXT) AS 'Цена', description AS 'Описание' FROM  services";
        string service = null;

        public Services_Window()
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
            txt_service.Text = "";
            txt_price.Text = "";
            txt_description.Text = "";
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var isValid = Regex.IsMatch(txt_price.Text + e.Text, @"\A[0-9]+(?:[.,])?(?:[0-9]{1,2})?\z");
            if (!isValid)
            {
                e.Handled = true;
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_service.Text.Length == 0 || txt_price.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "INSERT INTO services (service, price, description) VALUES (@service,  @price,  @description)";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                    command.Parameters.AddWithValue("@service", txt_service.Text);
                    command.Parameters.AddWithValue("@price", txt_price.Text.Replace(",", "."));
                    command.Parameters.AddWithValue("@description", txt_description.Text);
                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                    refresh_table();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); sqlConnection.Close(); }

            }
        }

        private void delButton_Click(object sender, RoutedEventArgs e)
        {

            if (service == null)
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Удалить запись {service}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM services WHERE service = '{service}'", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                        }
                        service = null;
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_service.Text.Length == 0)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (service == null)
                    MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    if (MessageBox.Show($"Редактировать запись {service}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                            {
                                connection.Open();
                                SQLiteCommand command = new SQLiteCommand("UPDATE services SET service = @service, price = @price, description = @description WHERE service = @service1", connection);
                                command.Parameters.AddWithValue("@service1", service);
                                command.Parameters.AddWithValue("@service", txt_service.Text);
                                command.Parameters.AddWithValue("@price", txt_price.Text.Replace(",", "."));
                                command.Parameters.AddWithValue("@description", txt_description.Text);
                                command.ExecuteNonQuery();
                                refresh_table();
                            }
                            service = null;
                        }
                        catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                    }
                }
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (dataGrid.Items.Count != 0 && selectedRow != null)
            {
                txt_service.Text = Convert.ToString(selectedRow["Услуга"]);
                txt_price.Text = Convert.ToString(selectedRow["Цена"]);
                txt_description.Text = Convert.ToString(selectedRow["Описание"]);

                service = Convert.ToString(selectedRow["Услуга"]);
            }
        }
    }
}
