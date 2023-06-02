using System;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Persolal_Account_Window.xaml
    /// </summary>
    public partial class Persolal_Account_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT name AS \"ФИО\", id AS \"Лиц счет\", pasport AS \"Паспорт\", address AS \"Адрес\", clients.description AS \"Описание\" FROM clients";
        string query1 = "SELECT complite as 'Проведено', id as '№ транзакции', date_transaction as 'Дата', sum AS \"Сумма руб\", description AS \"Назначение\" FROM  transactions where personal_acount = 0";
        string id = null;
        string id_t = null;
        public Persolal_Account_Window()
        {
            InitializeComponent();

            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = dataTable.DefaultView;
            ClearTxt();
            sqlConnection.Close();
        }

        public void refresh_table1()
        {
            //try
            //{
            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query1, sqlConnection);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid1.ItemsSource = dataTable.DefaultView;
            ClearTxt();
            sqlConnection.Close();
            ShowTransactionSum();

            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }   
        }


        public void ClearTxt()
        {
            txt_sum.Text = "";
            date_picker_t.Text = DateTime.Now.ToShortDateString();
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
            if (txt_sum.Text.Length == 0 || date_picker_t.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Convert.ToDateTime(date_picker_t.Text) >= DateTime.Now) MessageBox.Show("Неверная дата", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (id == null) MessageBox.Show("Выберите клиента!","", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "INSERT INTO transactions (date_transaction, sum, personal_account, description) VALUES ('" + date_picker_t.Text + "', '" + txt_sum.Text + "' , '" + id + "', '" + txt_description.Text + "')";

                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);

                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                    refresh_table1();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

            }
        }



        private void txt_find_KeyUp(object sender, KeyEventArgs e)
        {
            if (txt_find.Text == "")
                query = "SELECT name AS \"ФИО\", id AS \"Лиц счет\", pasport AS \"Паспорт\", address AS \"Адрес\", clients.description AS \"Описание\" FROM clients";
            else
                query = "SELECT name AS \"ФИО\", id AS \"Лиц счет\", pasport AS \"Паспорт\", address AS \"Адрес\", clients.description AS \"Описание\" FROM clients WHERE name Like \"%" + txt_find.Text + "%\"";
            
            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = dataTable.DefaultView;
            ClearTxt();
            sqlConnection.Close();
        }



        private void delButton_Click(object sender, RoutedEventArgs e)
        {

            if (id_t == null)
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Удалить запись?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM transactions WHERE id = \"{id_t}\"", connection);
                            command.ExecuteNonQuery();
                            refresh_table1();
                            MessageBox.Show("Запись удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        id_t = null;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }


                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_sum.Text.Length == 0 || date_picker_t.Text.Length == 0 || txt_description.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Convert.ToDateTime(date_picker_t.Text) >= DateTime.Now) MessageBox.Show("Неверная дата", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (id_t == null)
                    MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    if (MessageBox.Show($"Редактировать запись?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                            {
                                connection.Open();
                                SQLiteCommand command = new SQLiteCommand("UPDATE transactions SET sum =\"" + txt_sum.Text + "\",  date_transaction = \"" + date_picker_t.Text +"\", description = \"" + txt_description.Text + "\" WHERE id = @id_t", connection);
                                command.Parameters.AddWithValue("@id_t", id_t);
                                command.ExecuteNonQuery();
                                MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                refresh_table1();
                            }
                            id_t = null;
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
                id = Convert.ToString(selectedRow["Лиц счет"]);
                query1 = "SELECT complite as 'Проведено', id as '№ транзакции', date_transaction as 'Дата', sum AS \"Сумма руб\", description AS \"Назначение\" FROM  transactions where personal_account = " + id;
                refresh_table1();
            }
        }

        private void ShowTransactionSum()
        {
            string sqlQuery = "SELECT SUM(sum) FROM transactions where personal_account = " + id;
            double sum = 0.0;

            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
                {
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        sum = Convert.ToDouble(result);
                    }
                }
            }

            txt_balance.Text = "Остаток: "+sum.ToString()+" руб.";
            if(sum > 0)
            {
                txt_balance.Foreground = Brushes.Blue;
            }
            else
                txt_balance.Foreground = Brushes.Red;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (id != null)
            {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand("UPDATE transactions SET complite = 'Да'  WHERE personal_account = @id", connection);
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                            MessageBox.Show("Записи проведены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        refresh_table1();
                        ShowTransactionSum();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            else MessageBox.Show("Выберите клиента");
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid1.SelectedItem;
            if (dataGrid.Items.Count != 0 && selectedRow != null)
            {
                id_t = Convert.ToString(selectedRow["№ транзакции"]);
                date_picker_t.Text = Convert.ToString(selectedRow["Дата"]);
                txt_sum.Text = Convert.ToString(selectedRow["Сумма руб"]);
                txt_description.Text = Convert.ToString(selectedRow["Назначение"]);
            }
        }

        public class ValueToBrushConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values.Length < 2) return null;

                if (values[0] == null || values[0] == DependencyProperty.UnsetValue) return null;
                decimal amount = 0;
                try
                {
                    amount = System.Convert.ToDecimal(values[0], CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    return null;
                }

                if (amount == 0)
                {
                    return Brushes.Black;
                }
                else if (amount < 0)
                {
                    return Brushes.Red;
                }
                else
                {
                    return Brushes.Blue;
                }
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }

        private void Clientbtn_Click(object sender, RoutedEventArgs e)
        {
            ClientsWindow clientsWindow = new ClientsWindow();
            clientsWindow.ShowDialog();
        }
    }
}
