using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Maid_Window.xaml
    /// </summary>
    public partial class Maid_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT name AS 'ФИО', position AS 'Должность', employe_mode AS 'Режим работы' FROM  maids";
        string name = null;

        public Maid_Window()
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
            txt_name.Text = "";
            txt_position.Text = "";
            txt_employe_mode.Text = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_name.Text.Length == 0 || txt_position.Text.Length == 0 || txt_employe_mode.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "INSERT INTO maids (name, position, employe_mode) VALUES (@name,  @position,  @employe_mode)";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                    command.Parameters.AddWithValue("@name", txt_name.Text);
                    command.Parameters.AddWithValue("@position", txt_position.Text);
                    command.Parameters.AddWithValue("@employe_mode", txt_employe_mode.Text);
                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);   sqlConnection.Close();}

                refresh_table();
            }
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
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM maids WHERE name = '{name}'", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                        }
                        name = null;
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_name.Text.Length == 0 || txt_position.Text.Length == 0 || txt_employe_mode.Text.Length == 0)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                                SQLiteCommand command = new SQLiteCommand("UPDATE maids SET name = @name, position = @position, employe_mode = @employe_mode WHERE name = @name", connection);
                                command.Parameters.AddWithValue("@name", name);
                                command.Parameters.AddWithValue("@position", txt_position.Text);
                                command.Parameters.AddWithValue("@employe_mode", txt_employe_mode.Text);
                                command.ExecuteNonQuery();

                                refresh_table();
                            }
                            name = null;
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
                txt_name.Text = Convert.ToString(selectedRow["ФИО"]);
                txt_position.Text = Convert.ToString(selectedRow["Должность"]);
                txt_employe_mode.Text = Convert.ToString(selectedRow["Режим работы"]);

                name = Convert.ToString(selectedRow["ФИО"]);
            }
        }
    }
}
