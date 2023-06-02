using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для ClientsWindow.xaml
    /// </summary>
    public partial class choiceClientsWindow : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT name AS \"ФИО\", status_clients.status AS \"Статус\", pasport AS \"Паспорт\", gender AS \"Пол\", birthday AS \"Дата рождения\", address AS \"Адрес\", clients.description AS \"Описание\" FROM clients, status_clients where status_clients.id = clients.status";
        string query1 = "SELECT id as 'Шифр', type AS \"Тип питания\", description AS \"Описание\" FROM  type_of_foods";
        string name = null;
        string id1 = null;
        string type = null;
        public choiceClientsWindow()
        {
            InitializeComponent();

            refresh_table();

            sqlConnection.Open();
            string query = "SELECT status FROM status_clients";
            SQLiteCommand command = new SQLiteCommand(query, sqlConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string status = reader.GetString(0);
                comboBox_status.Items.Add(status);
            }
            sqlConnection.Close();

        }

        public choiceClientsWindow(string id)
        {
            InitializeComponent();
            id1 = id;
            refresh_table();
            refresh_table1();
            sqlConnection.Open();
            string query = "SELECT status FROM status_clients";
            SQLiteCommand command = new SQLiteCommand(query, sqlConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string status = reader.GetString(0);
                comboBox_status.Items.Add(status);
            }
            sqlConnection.Close();

        }

        public void refresh_table1()
        {
            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query1, sqlConnection);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid1.ItemsSource = dataTable.DefaultView;
            ClearTxt();
            sqlConnection.Close();
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
            txt_name.Text = "";
            comboBox_status.Text = "";
            txt_pasport.Text = "";
            comboBox_gender.Text = "";
            data_picker_birthday.Text = "";
            txt_address.Text = "";
            txt_description.Text = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_name.Text.Length == 0 || comboBox_status.SelectedIndex == -1 || txt_pasport.Text.Length == 0 || comboBox_gender.SelectedIndex == -1
            || data_picker_birthday.Text.Length == 0 || txt_address.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Convert.ToDateTime(data_picker_birthday.Text) >= DateTime.Now) MessageBox.Show("Неверная дата", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "INSERT INTO clients (name, status,  pasport, gender, birthday, address, description) " +
                                    "VALUES (\"" + txt_name.Text + "\", (select id from status_clients where status like \"%" + comboBox_status.Text + "%\"), \"" + txt_pasport.Text + "\", \"" + comboBox_gender.Text + "\", \"" + data_picker_birthday.Text + "\", \"" + txt_address.Text + "\", \"" + txt_description.Text + "\")";

                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);

                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                    refresh_table();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

            }
        }



        private void txt_find_KeyUp(object sender, KeyEventArgs e)
        {
            if (txt_find.Text == "")
                query = "SELECT  name AS \"ФИО\", status_clients.status AS \"Статус\", pasport AS \"Паспорт\", gender AS \"Пол\", birthday AS \"Дата рождения\", address AS \"Адрес\", clients.description AS \"Описание\" FROM clients, status_clients where status_clients.id = clients.status";
            else
                query = "SELECT  name AS \"ФИО\", status_clients.status AS \"Статус\", pasport AS \"Паспорт\", gender AS \"Пол\", birthday AS \"Дата рождения\", address AS \"Адрес\", clients.description AS \"Описание\" FROM clients, status_clients where status_clients.id = clients.status AND name Like \"%" + txt_find.Text + "%\"";
            refresh_table();

        }



        private void delButton_Click(object sender, RoutedEventArgs e)
        {

            if (name == null)
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Удалить запись {name}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM clients WHERE name = \"{name}\"", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                            MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }


                    name = null;
                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_name.Text.Length == 0 || comboBox_status.SelectedIndex == -1 || txt_pasport.Text.Length == 0 || comboBox_gender.SelectedIndex == -1
            || data_picker_birthday.Text.Length == 0 || txt_address.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Convert.ToDateTime(data_picker_birthday.Text) >= DateTime.Now) MessageBox.Show("Неверная дата", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (name == null)
                    MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    if (MessageBox.Show($"Редактировать запись {name}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                            {
                                connection.Open();
                                SQLiteCommand command = new SQLiteCommand("UPDATE clients SET name =\"" + txt_name.Text + "\", status =  (select id from status_clients where status like \"%" + comboBox_status.Text + "%\"), pasport = \"" + txt_pasport.Text + "\", gender = \"" + comboBox_gender.Text + "\", birthday = \"" + data_picker_birthday.Text + "\", address = \"" + txt_address.Text + "\", description = \"" + txt_description.Text + "\" WHERE name = @name", connection);
                                command.Parameters.AddWithValue("@name", name);
                                command.ExecuteNonQuery();
                                MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                refresh_table();
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                        name = null;
                    }
                }
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (dataGrid.Items.Count != 0 && selectedRow != null)
            {
                txt_name.Text = Convert.ToString(selectedRow["ФИО"]);
                comboBox_status.Text = Convert.ToString(selectedRow["Статус"]);
                txt_pasport.Text = Convert.ToString(selectedRow["Паспорт"]);
                comboBox_gender.Text = Convert.ToString(selectedRow["Пол"]);
                data_picker_birthday.Text = Convert.ToString(selectedRow["Дата рождения"]);
                txt_address.Text = Convert.ToString(selectedRow["Адрес"]);
                txt_description.Text = Convert.ToString(selectedRow["Описание"]);

                name = Convert.ToString(selectedRow["ФИО"]);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (name != null)
            {
                if (type != null)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand("UPDATE clients SET order_code = " + id1 + ", type_food = " + type + " WHERE name = @name", connection);
                            command.Parameters.AddWithValue("@name", name);
                            command.ExecuteNonQuery();
                        }

                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                    //refresh_table1("select name AS ФИО, type_of_foods.type AS 'Тип питания' from clients,  type_of_foods where type_of_foods.id = clients.type_food AND order_code = "+id1);
                    Close();

                }
                else MessageBox.Show("Выберите тип питания");

            }
            else MessageBox.Show("Выберите клиента");
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid1.SelectedItem;
            type = Convert.ToString(selectedRow["Шифр"]);
        }
    }
}
