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
    public partial class ClientsWindow : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT name AS \"ФИО\", status_clients.status AS \"Статус\", pasport AS \"Паспорт\", gender AS \"Пол\", birthday AS \"Дата рождения\", address AS \"Адрес\", clients.description AS \"Описание\" FROM clients, status_clients where status_clients.id = clients.status";
        string name = null;
        public ClientsWindow()
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
        public void refresh_table()
        {
            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = dataTable.DefaultView;


            sqlConnection.Close();
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWin mainWin = new MainWin();
            mainWin.Show();
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {


            sqlConnection.Open();

            string sql = "INSERT INTO clients (name,  pasport, gender, birthday, address, description) " +
                            "VALUES (\"" + txt_name.Text + "\", \"" + txt_pasport.Text + "\", \"" + comboBox_gender.Text + "\", \"" + date_picker_birthday.Text + "\", \"" + txt_address.Text + "\", \"" + txt_description.Text + "\")";

            SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);

            command.ExecuteNonQuery();

            sqlConnection.Close();

            refresh_table();
        }



        private void txt_find_KeyUp(object sender, KeyEventArgs e)
        {
            if (txt_find.Text == "")
                query = "SELECT  name AS \"ФИО\", status_clients.status AS \"Статус\", pasport AS \"Паспорт\", gender AS \"Пол\", birthday AS \"Дата рождения\", address AS \"Адрес\", clients.description AS \"Описание\" FROM clients, status_clients where status_clients.id = clients.status";
            else
                query = "SELECT  name AS \"ФИО\", status_clients.status AS \"Статус\", pasport AS \"Паспорт\", gender AS \"Пол\", birthday AS \"Дата рождения\", address AS \"Адрес\", clients.description AS \"Описание\" FROM clients, status_clients where status_clients.id = clients.status AND name Like \"%" + txt_find.Text + "%\"";
            refresh_table();

        }

        private void dataGrid_Selected(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dataGrid.Items.Count != 0)
            {
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                txt_name.Text = Convert.ToString(selectedRow["ФИО"]);
                comboBox_status.Text = Convert.ToString(selectedRow["Статус"]);
                txt_pasport.Text = Convert.ToString(selectedRow["Паспорт"]);
                comboBox_gender.Text = Convert.ToString(selectedRow["Пол"]);
                date_picker_birthday.Text = Convert.ToString(selectedRow["Дата рождения"]);
                txt_address.Text = Convert.ToString(selectedRow["Адрес"]);
                txt_description.Text = Convert.ToString(selectedRow["Описание"]);

                string name = Convert.ToString(selectedRow["ФИО"]);
            }
        }

        private void delButton_Click(object sender, RoutedEventArgs e)
        {
            if (name == null)
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (Convert.ToBoolean(MessageBox.Show($"Удалить запись №{name}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question)) == DialogResult)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                    {
                        connection.Open();
                        SQLiteCommand command = new SQLiteCommand("DELETE FROM clients WHERE name = @name", connection);
                        command.Parameters.AddWithValue("@name", name);
                        command.ExecuteNonQuery();
                        refresh_table();
                    }
                }
            }
        }
    }
}
