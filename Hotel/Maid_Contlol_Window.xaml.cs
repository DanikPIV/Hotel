using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Maid_Contlol_Window.xaml
    /// </summary>
    public partial class Maid_Contlol_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=hotel.db");
        string query = "SELECT control_maids.id as 'Шифр', strftime('%d.%m.%Y', date) AS 'Дата и время', num AS 'Номер',  services_rooms.description AS 'Вид работ', name AS 'ФИО ответственного', complited AS 'Выполнено' FROM maids, control_maids, services_rooms, rooms where service = services_rooms.id AND maid = maids.id AND room_num = rooms.id";
        string id = null;
        public Maid_Contlol_Window()
        {
            InitializeComponent();

            refresh_table();

            sqlConnection.Open();
            string query = "SELECT num FROM rooms";
            SQLiteCommand command = new SQLiteCommand(query, sqlConnection);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string num = reader.GetString(0);
                    comboBox_num.Items.Add(num);
                }
                reader.Close();
            }

            string query1 = "SELECT description FROM services_rooms";
            SQLiteCommand command1 = new SQLiteCommand(query1, sqlConnection);
            using (SQLiteDataReader reader1 = command1.ExecuteReader())
            {
                while (reader1.Read())
                {
                    string serv = reader1.GetString(0);
                    comboBox_service.Items.Add(serv);
                }
            }
            string query2 = "SELECT name FROM maids";
            SQLiteCommand command2 = new SQLiteCommand(query2, sqlConnection);
            using (SQLiteDataReader reader2 = command2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    string name = reader2.GetString(0);
                    comboBox_name.Items.Add(name);
                }
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
            ClearTxt();
            sqlConnection.Close();
        }

        public void ClearTxt()
        {
            date_picker1.Text = "";
            comboBox_name.Text = "";
            comboBox_service.Text = "";
            comboBox_num.Text = "";
            comboBox_complite.Text = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_name.Text.Length == 0 || comboBox_service.Text.Length == 0
            || comboBox_num.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {

                try
                {
                    sqlConnection.Open();

                    string sql = "INSERT INTO  control_maids (maid, date, service, room_num, complited) " +
                                    "VALUES ( (select id from maids where name like @maid), @date, (select id from services_rooms where description like @service), (select id from rooms where num like @room_num), @complited)";

                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);

                    command.Parameters.AddWithValue("@maid", comboBox_name.Text);
                    DateTime date = DateTime.Parse(date_picker1.Text);
                    string formattedDate = date.ToString("yyyy-MM-dd");
                    command.Parameters.AddWithValue("@date", formattedDate);
                    command.Parameters.AddWithValue("@service", comboBox_service.Text);
                    command.Parameters.AddWithValue("@room_num", comboBox_num.Text);
                    command.Parameters.AddWithValue("@complited", comboBox_complite.Text);
                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                    refresh_table();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" +ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);  sqlConnection.Close(); }

            }
        }





        private void delButton_Click(object sender, RoutedEventArgs e)
        {

            if (id == null)
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Удалить запись {id}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM control_maids WHERE id = '{id}'", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                        }
                        id = null;
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" +ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }


                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_name.Text.Length == 0 || comboBox_service.Text.Length == 0
            || comboBox_num.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (id == null)
                    MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    if (MessageBox.Show($"Редактировать запись {id}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                            {
                                connection.Open();
                                SQLiteCommand command = new SQLiteCommand("UPDATE control_maids SET maid = (select id from maids where name like @maid), date =   @date, service = (select id from services_rooms where description like @service), room_num =  (select id from rooms where num like @room_num), complited = @complited WHERE id = @id", connection);
                                command.Parameters.AddWithValue("@id", id);
                                command.Parameters.AddWithValue("@maid", comboBox_name.Text);
                                DateTime date = DateTime.Parse(date_picker1.Text);
                                string formattedDate = date.ToString("yyyy-MM-dd");
                                command.Parameters.AddWithValue("@date", formattedDate);
                                command.Parameters.AddWithValue("@service", comboBox_service.Text);
                                command.Parameters.AddWithValue("@room_num", comboBox_num.Text);
                                command.Parameters.AddWithValue("@complited", comboBox_complite.Text);
                                command.ExecuteNonQuery();
                                refresh_table();
                            }
                            id = null;
                        }
                        catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" +ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                    }
                }
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (dataGrid.Items.Count != 0 && selectedRow != null)
            {
                comboBox_name.Text = Convert.ToString(selectedRow["ФИО ответственного"]);
                date_picker1.Text = Convert.ToString(selectedRow["Дата и время"]);
                comboBox_service.Text = Convert.ToString(selectedRow["Вид работ"]);
                comboBox_num.Text = Convert.ToString(selectedRow["Номер"]);
                comboBox_complite.Text = Convert.ToString(selectedRow["Выполнено"]);

                id = Convert.ToString(selectedRow["Шифр"]);
            }
        }

        private void CheckBox1_Checked(object sender, RoutedEventArgs e)
        {

            query = "SELECT control_maids.id as 'Шифр', strftime('%d.%m.%Y', date) AS 'Дата и время', num AS 'Номер',  services_rooms.description AS 'Вид работ', name AS 'ФИО ответственного', complited AS 'Выполнено' FROM maids, control_maids, services_rooms, rooms where service = services_rooms.id AND maid = maids.id AND room_num = rooms.id AND complited = 'Нет'";
            refresh_table();

        }

        private void CheckBox1_UnChecked(object sender, RoutedEventArgs e)
        {
            query = "SELECT control_maids.id as 'Шифр', strftime('%d.%m.%Y', date) AS 'Дата и время', num AS 'Номер',  services_rooms.description AS 'Вид работ', name AS 'ФИО ответственного', complited AS 'Выполнено' FROM maids, control_maids, services_rooms, rooms where service = services_rooms.id AND maid = maids.id AND room_num = rooms.id";
            refresh_table();
        }

        private void date_picker_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void maids_service_btn_Click(object sender, RoutedEventArgs e)
        {
            Servises_of_room_Window servises_Of_Room_Window = new Servises_of_room_Window();
            servises_Of_Room_Window.ShowDialog();

            comboBox_service.Items.Clear();
            string query1 = "SELECT description FROM services_rooms";
            SQLiteCommand command1 = new SQLiteCommand(query1, sqlConnection);
            using (SQLiteDataReader reader1 = command1.ExecuteReader())
            {
                while (reader1.Read())
                {
                    string serv = reader1.GetString(0);
                    comboBox_service.Items.Add(serv);
                }
            }
        }

        private void maids_btn_Click(object sender, RoutedEventArgs e)
        {
            Maid_Window maid_Window = new Maid_Window();
            maid_Window.ShowDialog();
            comboBox_name.Items.Clear();
            string query2 = "SELECT name FROM maids";
            SQLiteCommand command2 = new SQLiteCommand(query2, sqlConnection);
            using (SQLiteDataReader reader2 = command2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    string name = reader2.GetString(0);
                    comboBox_name.Items.Add(name);
                }
            }
        }
    }
}
