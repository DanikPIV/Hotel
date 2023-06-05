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
        string query = "SELECT name AS 'ФИО', status_clients.status AS 'Статус', pasport AS 'Паспорт', gender AS 'Пол', birthday AS 'Дата рождения', address AS 'Адрес', clients.description AS 'Описание' FROM clients, status_clients where status_clients.id = clients.status";
        string query1 = "SELECT id as 'Шифр', type AS 'Тип питания', description AS 'Описание' FROM  type_of_foods";
        string name = null;
        string id1 = null;
        string type = null;
        public choiceClientsWindow()
        {
            InitializeComponent();

            refresh_table();

        }

        public choiceClientsWindow(string id)
        {
            InitializeComponent();
            id1 = id;
            refresh_table();
            refresh_table1();

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
            try
            {
            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = dataTable.DefaultView;
            ClearTxt();
            sqlConnection.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void ClearTxt()
        {
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       



        private void txt_find_KeyUp(object sender, KeyEventArgs e)
        {
            if (txt_find.Text == "")
                query = "SELECT  name AS 'ФИО', status_clients.status AS 'Статус', pasport AS 'Паспорт', gender AS 'Пол', birthday AS 'Дата рождения', address AS 'Адрес', clients.description AS 'Описание' FROM clients, status_clients where status_clients.id = clients.status";
            else
                query = "SELECT  name AS 'ФИО', status_clients.status AS 'Статус', pasport AS 'Паспорт', gender AS 'Пол', birthday AS 'Дата рождения', address AS 'Адрес', clients.description AS 'Описание' FROM clients, status_clients where status_clients.id = clients.status AND name Like '%" + txt_find.Text + "%'";
            refresh_table();

        }



       
        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid1.SelectedItem;
            type = Convert.ToString(selectedRow["Шифр"]);
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (dataGrid.Items.Count != 0 && selectedRow != null)
            {

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
                    Close();

                }
                else MessageBox.Show("Выберите тип питания");

            }
            else MessageBox.Show("Выберите клиента");
        }

    }
}
