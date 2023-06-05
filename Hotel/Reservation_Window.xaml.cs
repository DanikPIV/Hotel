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
    /// Логика взаимодействия для Reservation_Window.xaml
    /// </summary>
    public partial class Reservation_Window : Window
    {

        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=hotel.db");
        string query = "SELECT CAST(reservation.id AS CHAR (15) ) AS 'Шифр', strftime('%d.%m.%Y', reservation.date_from) AS 'Занят с', strftime('%d.%m.%Y', reservation.date_to) AS 'по', reserv AS 'Бронь',(SELECT num  FROM rooms WHERE room = rooms.id) AS 'Номер', CASE WHEN reservation.reserv = 'Бронь' THEN (select reservation_price from price_list WHERE holyday = 'Да') ELSE (select price from price_list WHERE holyday = 'Да')  END AS 'Цена в раб', CASE WHEN reservation.reserv = 'Бронь' THEN (select reservation_price from price_list WHERE holyday = 'Да') ELSE (select price from price_list WHERE holyday = 'Да')  END AS 'Цена в вых', reservation.holyday AS 'Кол-во вых',                       CAST(CASE WHEN reservation.reserv = 'Бронь' THEN (select reservation_price from price_list WHERE holyday = 'Нет') ELSE (select price from price_list WHERE holyday = 'Нет')  END AS REAL)+ (CAST(CASE WHEN reservation.reserv = 'Бронь' THEN (select reservation_price from price_list WHERE holyday = 'Да') ELSE (select price from price_list WHERE holyday = 'Да')  END  AS REAL) * CAST(reservation.holyday AS INTEGER)) + CAST( doplata AS REAL) AS 'Итого' FROM reservation, rooms where room = rooms.id";
        string id = null;
        string name = null;
        string sum = null;
        string query1 = null;
        string date1 = null, date2 = null, res = null, num = null;

        public Reservation_Window()
        {
            InitializeComponent();
            win.Title = "Сдача / бронирование номеров. Сегодня " + DateTime.Now.ToShortDateString();
            refresh_table();
        }
        public void refresh_table()
        {
            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = dataTable.DefaultView;

            comboBox_num.Items.Clear();
            comboBox_num_f.Items.Clear();
            string query1 = "SELECT num FROM rooms";
            SQLiteCommand command = new SQLiteCommand(query1, sqlConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string num = reader.GetString(0);
                comboBox_num.Items.Add(num);
                comboBox_num_f.Items.Add(num);

            }

            ClearTxt();
            sqlConnection.Close();
        }

        public void refresh_table1()
        {
            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query1, sqlConnection);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid1.ItemsSource = dataTable.DefaultView;
            sqlConnection.Close();
        }

        public void ClearTxt()
        {
            comboBox_num.Text = "";
            date_picker1.Text = "";
            date_picker2.Text = "";
            txt_holiday.Text = "";
            comboBox_res.Text = "";
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var isValid = Regex.IsMatch(txt_holiday.Text + e.Text, @"\A[0-9]+\z");
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
            if (comboBox_num.Text.Length == 0 ||
            date_picker1.Text.Length == 0 ||
            date_picker2.Text.Length == 0 ||
            txt_holiday.Text.Length == 0 ||
            comboBox_res.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Convert.ToDateTime(date_picker1.Text) >= Convert.ToDateTime(date_picker2.Text))
            {
                MessageBox.Show("Не корректные даты!");
                date_picker1.Text = "";
                date_picker2.Text = "";
            }
            else
            {

                try
                {
                    sqlConnection.Open();

                    string sql = "INSERT INTO reservation (date_from, date_to, reserv, room, holyday) " +
                                    "VALUES (@date1, @date2, @reserv, (select rooms.id from rooms WHERE num == @room), @holyday)";

                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection); 

                    DateTime date = DateTime.Parse(date_picker1.Text);
                    string formattedDate = date.ToString("yyyy-MM-dd");
                    command.Parameters.AddWithValue("@date1", formattedDate);

                    date = DateTime.Parse(date_picker2.Text);
                    formattedDate = date.ToString("yyyy-MM-dd");
                    command.Parameters.AddWithValue("@date2", formattedDate);

                    command.Parameters.AddWithValue("@reserv", comboBox_res.Text);
                    command.Parameters.AddWithValue("@room", comboBox_num.Text);
                    command.Parameters.AddWithValue("@holyday", txt_holiday.Text);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();
                  
                    refresh_table();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);   sqlConnection.Close();}

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
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM reservation WHERE id = '{id}'", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                        }
                        id = null;
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }


                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_num.Text.Length == 0 ||
            date_picker1.Text.Length == 0 ||
            date_picker2.Text.Length == 0 ||
            txt_holiday.Text.Length == 0 ||
            comboBox_res.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Convert.ToDateTime(date_picker1.Text) >= Convert.ToDateTime(date_picker2.Text))
            {
                MessageBox.Show("Не корректные даты!");
                date_picker1.Text = "";
                date_picker2.Text = "";
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
                                SQLiteCommand command = new SQLiteCommand("UPDATE reservation SET date_from =   @date1, date_to = @date2, reserv = @reserv, room = (select rooms.id from rooms WHERE num == @room), holyday = @holyday WHERE id = @id", connection);
                                command.Parameters.AddWithValue("@id", id);

                                DateTime date = DateTime.Parse(date_picker1.Text);
                                string formattedDate = date.ToString("yyyy-MM-dd");
                                command.Parameters.AddWithValue("@date1", formattedDate);

                                date = DateTime.Parse(date_picker2.Text);
                                formattedDate = date.ToString("yyyy-MM-dd");
                                command.Parameters.AddWithValue("@date2", formattedDate);

                                command.Parameters.AddWithValue("@reserv", comboBox_res.Text);
                                command.Parameters.AddWithValue("@room", comboBox_num.Text);
                                command.Parameters.AddWithValue("@holyday", txt_holiday.Text);
                                command.ExecuteNonQuery();
                                refresh_table();
                            }
                            id = null;
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
                date1 = date_picker1.Text = Convert.ToString(selectedRow["Занят с"]);
                date2 = date_picker2.Text = Convert.ToString(selectedRow["по"]);
                res = comboBox_res.Text = Convert.ToString(selectedRow["Бронь"]);
                num = comboBox_num.Text = Convert.ToString(selectedRow["Номер"]);
                sum = Convert.ToString(selectedRow["Итого"]);
                txt_holiday.Text = Convert.ToString(selectedRow["Кол-во вых"]);
                name = null;
                id = Convert.ToString(selectedRow["Шифр"]);
                query1 = "select name AS ФИО, type_of_foods.type AS 'Тип питания' from clients, type_of_foods where type_of_foods.id = clients.type_food AND order_code = " + id;
                refresh_table1();
            }
        }

        private void button_filter_Click(object sender, RoutedEventArgs e)
        {
            query = "SELECT CAST(reservation.id AS CHAR (15) ) AS 'Шифр', strftime('%d.%m.%Y', reservation.date_from) AS 'Занят с', strftime('%d.%m.%Y', reservation.date_to) AS 'по', reserv AS 'Бронь',(SELECT num  FROM rooms WHERE room = rooms.id) AS 'Номер', CASE WHEN reservation.reserv = 'Бронь' THEN (select reservation_price from price_list WHERE holyday = 'Да') ELSE (select price from price_list WHERE holyday = 'Да')  END AS 'Цена в раб', CASE WHEN reservation.reserv = 'Бронь' THEN (select reservation_price from price_list WHERE holyday = 'Да') ELSE (select price from price_list WHERE holyday = 'Да')  END AS 'Цена в вых', reservation.holyday AS 'Кол-во вых',                       CAST(CASE WHEN reservation.reserv = 'Бронь' THEN (select reservation_price from price_list WHERE holyday = 'Нет') ELSE (select price from price_list WHERE holyday = 'Нет')  END AS REAL)+ (CAST(CASE WHEN reservation.reserv = 'Бронь' THEN (select reservation_price from price_list WHERE holyday = 'Да') ELSE (select price from price_list WHERE holyday = 'Да')  END  AS REAL) * CAST(reservation.holyday AS INTEGER)) + CAST( doplata AS REAL) AS 'Итого' FROM reservation, rooms where room = rooms.id ";
            sqlConnection.Open();
            if (CheckBox1.IsChecked == true && comboBox_num_f.Text.Length != 0)
            {
                query = query + " AND [Номер] = @num";

                if (CheckBox2.IsChecked == true && date_picker_f.Text.Length != 0)
                {
                    query = query + " AND  strftime('%s',reservation.date_from) <= strftime('%s',  @date) AND strftime('%s',reservation.date_to) >= strftime('%s',  @date)";

                    if (CheckBox3.IsChecked == true && comboBox_res_f.Text.Length != 0)
                    {
                        query = query + " AND  reserv = @reserv";
                    }
                }
                else if (CheckBox3.IsChecked == true && comboBox_res_f.Text.Length != 0)
                {
                    query = query + " AND  reserv = @reserv";
                }

            }
            else if (CheckBox2.IsChecked == true && date_picker_f.Text.Length != 0)
            {
                query = query + " AND  strftime('%s',reservation.date_from) <= strftime('%s', @date) AND strftime('%s',reservation.date_to) >= strftime('%s',  @date)";

                if (CheckBox3.IsChecked == true && comboBox_res_f.Text.Length != 0)
                {
                    query = query + " AND  reserv = @reserv";
                }
            }
            else if (CheckBox3.IsChecked == true && comboBox_res_f.Text.Length != 0)
            {
                query = query + " AND reserv = @reserv";

            }
            SQLiteCommand command = new SQLiteCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@num", comboBox_num_f.Text);
            if (CheckBox2.IsChecked == true && date_picker_f.Text.Length != 0)
            {
                DateTime date = DateTime.Parse(date_picker_f.Text);
                string formattedDate = date.ToString("yyyy-MM-dd");
                command.Parameters.AddWithValue("@date", formattedDate);
            }

            command.Parameters.AddWithValue("@reserv", comboBox_res_f.Text);
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
            dataAdapter.SelectCommand = command;
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = dataTable.DefaultView;
            sqlConnection.Close();
        }

        private void addButton1_Click(object sender, RoutedEventArgs e)
        {
            if (id != null)
            {
                choiceClientsWindow choiceClientsWindow = new choiceClientsWindow(id);
                choiceClientsWindow.ShowDialog();
                refresh_table1();
            }
            else
            {
                MessageBox.Show("Выберите заказ!");
            }


        }

        private void delButton1_Click(object sender, RoutedEventArgs e)
        {
            if (name == null)
                MessageBox.Show("Выберите клиента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Удалить запись {name}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand("UPDATE clients SET order_code = 0 WHERE name = @name", connection);
                            command.Parameters.AddWithValue("@name", name);
                            command.ExecuteNonQuery();
                            dataGrid1.ItemsSource = string.Empty;
                        }
                        name = null;
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }


                }
            }
        }


        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid1.SelectedItem;
            if (dataGrid.Items.Count != 0 && selectedRow != null)
            {
                name = Convert.ToString(selectedRow["ФИО"]);
            }
        }

        private void numButton_Click(object sender, RoutedEventArgs e)
        {
            Rooms_Window rooms_Window = new Rooms_Window();
            rooms_Window.ShowDialog();
            refresh_table();
        }

        private void cliButton_Click(object sender, RoutedEventArgs e)
        {
            ClientsWindow clients_Window = new ClientsWindow();
            clients_Window.ShowDialog();
        }

        private void foodButton_Click(object sender, RoutedEventArgs e)
        {
            type_of_food_Window type_Of_Food_Window = new type_of_food_Window();
            type_Of_Food_Window.ShowDialog();
        }

        private void transaction_btn_Click(object sender, RoutedEventArgs e)
        {
            if (id != null && name != null)
            {
                if (MessageBox.Show("Провести по счету клиента " + name + " расход в сумме " + sum + "?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"INSERT INTO transactions (personal_account, date_transaction, sum, description, complite) VALUES( (select id from clients where name = @name), '" + DateTime.Now.ToShortDateString() + "', CAST('-'||@sum AS REAL),   'Оплата за номер №'||@num||' ('||@res||') c '||@date1||' по '||@date2||'.', 'Да')", connection);
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@sum", sum);
                            command.Parameters.AddWithValue("@num", num);
                            command.Parameters.AddWithValue("@res", res);
                            command.Parameters.AddWithValue("@date1", date1);
                            command.Parameters.AddWithValue("@date2", date2);
                            command.ExecuteNonQuery();
                            MessageBox.Show("Транзакция проведена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
            }
            else MessageBox.Show("Сначала выберите клиента");
        }
    }
}
