using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Type_Room_Window.xaml
    /// </summary>
    public partial class Type_Room_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT type AS \"Тип\", count AS \"Мест\", description AS \"Описание\" FROM  room_types";
        string type = null;
        public Type_Room_Window()
        {
            InitializeComponent();
            refresh_table();


        }
        public void refresh_table()
        {
            using (SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db"))
            {
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dataGrid.ItemsSource = dataTable.DefaultView;
                ClearTxt();
            }
        }

        public void ClearTxt()
        {
            txt_type.Text = "";
            txt_count.Text = "";
            txt_description.Text = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_type.Text.Length == 0 || txt_count.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    string desc = txt_description.Text;
                    desc.Replace("\\\"", "");
                    sqlConnection.Open();

                    string sql = "INSERT INTO room_types (type, count, description) VALUES (\"" + txt_type.Text + "\", \"" + txt_count.Text + "\", \"" + desc + "\")";

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

            if (type == null)
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Удалить запись {type}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM room_types WHERE type = \"{type}\"", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                            MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }


                    type = null;
                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_type.Text.Length == 0 || txt_count.Text.Length == 0)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (type == null)
                    MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    if (MessageBox.Show($"Редактировать запись {type}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                            {
                                connection.Open();
                                SQLiteCommand command = new SQLiteCommand("UPDATE room_types SET type = \"" + txt_type.Text + "\", count = \"" + txt_count.Text + "\", description = \"" + txt_description.Text + "\" WHERE type = \"" + type + "\"", connection);
                                command.Parameters.AddWithValue("@type", type);
                                command.ExecuteNonQuery();

                                MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                refresh_table();
                            }
                            type = null;
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
                txt_type.Text = Convert.ToString(selectedRow["Тип"]);
                txt_count.Text = Convert.ToString(selectedRow["Мест"]);
                txt_description.Text = Convert.ToString(selectedRow["Описание"]);

                type = Convert.ToString(selectedRow["Тип"]);
            }
        }
    }
}
