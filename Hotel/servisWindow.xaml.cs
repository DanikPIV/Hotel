using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hotel
{
    /// <servicemary>
    /// Логика взаимодействия для servisWindow.xaml
    /// </servicemary>
    public partial class servisWindow : System.Windows.Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT name AS 'ФИО', id AS 'Лиц счет', pasport AS 'Паспорт', address AS 'Адрес', clients.description AS 'Описание' FROM clients";
        string query1 = "SELECT services_log.id AS 'Шифр',  services.service as 'Услуга', strftime('%d.%m.%Y', date) AS 'Дата', CAST(price as text) as 'Цена руб', paid as 'Оплачено' from services_log JOIN services ON services.id = services_log.service";
        string id = null;
        string id_t = null;
        string name = null;
        //string date = null;
        public servisWindow()
        {
            InitializeComponent();

            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
            System.Data.DataTable dataTable = new System.Data.DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = dataTable.DefaultView;

            string query1 = "SELECT service FROM services";
            SQLiteCommand command = new SQLiteCommand(query1, sqlConnection);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string num = reader.GetString(0);
                    txt_serv.Items.Add(num);
                }
            }
            sqlConnection.Close();
        }

        public void refresh_table1()
        {
            try
            {
                sqlConnection.Open();
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query1, sqlConnection);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataAdapter.Fill(dataTable);
                dataGrid1.ItemsSource = dataTable.DefaultView;
                ClearTxt();
                sqlConnection.Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message); }
        }


        public void ClearTxt()
        {
            txt_serv.Text = "";
            date_picker_t.Text = DateTime.Now.ToShortDateString();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (dataGrid.Items.Count != 0 && selectedRow != null)
            {
                id = Convert.ToString(selectedRow["Лиц счет"]);
                name = Convert.ToString(selectedRow["ФИО"]);
                query1 = "SELECT services_log.id AS 'Шифр',  services.service as 'Услуга', strftime('%d.%m.%Y', date) AS 'Дата', price as 'Цена руб', paid as 'Оплачено' from services_log JOIN services ON services.id = services_log.service where personal_account = " + id;
                refresh_table1();
            }
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid1.SelectedItem;
            if (dataGrid.Items.Count != 0 && selectedRow != null)
            {
                id_t = Convert.ToString(selectedRow["Шифр"]);
                txt_serv.Text = Convert.ToString(selectedRow["Услуга"]);
                date_picker_t.Text = Convert.ToString(selectedRow["Дата"]);
            }
        }

        private void txt_find_KeyUp(object sender, KeyEventArgs e)
        {
            if (txt_find.Text == "")
                query = "SELECT name AS 'ФИО', id AS 'Лиц счет', pasport AS 'Паспорт', address AS 'Адрес', clients.description AS 'Описание' FROM clients";
            else
                query = "SELECT name AS 'ФИО', id AS 'Лиц счет', pasport AS 'Паспорт', address AS 'Адрес', clients.description AS 'Описание' FROM clients WHERE name Like '%'||@find||'%'";
            sqlConnection.Open();
            SQLiteCommand command = new SQLiteCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@find", txt_find.Text);
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
            dataAdapter.SelectCommand = command;
            System.Data.DataTable dataTable = new System.Data.DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = dataTable.DefaultView;
            sqlConnection.Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_serv.Text.Length == 0 || date_picker_t.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (id == null) MessageBox.Show("Выберите клиента!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                try
                {

                    sqlConnection.Open();
                    string sql = "INSERT INTO services_log (date,personal_account, service, paid) VALUES (@date, '" + id + "', (select id from services where service like '%'||@service||'%') , 'Нет')";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);

                    DateTime date = DateTime.Parse(date_picker_t.Text);
                    string formattedDate = date.ToString("yyyy-MM-dd");
                    command.Parameters.AddWithValue("@date", formattedDate);
                    command.Parameters.AddWithValue("@service", txt_serv.Text);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();

                    refresh_table1();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); sqlConnection.Close(); }

            }
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
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM services_log WHERE id = '{id_t}'", connection);
                            command.ExecuteNonQuery();
                            refresh_table1();
                        }
                        id_t = null;
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }


                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_serv.Text.Length == 0 || date_picker_t.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                                SQLiteCommand command = new SQLiteCommand("UPDATE services_log SET service = (select id from services where service = @service),  date = @date WHERE id = @id_t", connection);
                                command.Parameters.AddWithValue("@id_t", id_t);

                                DateTime date = DateTime.Parse(date_picker_t.Text);
                                string formattedDate = date.ToString("yyyy-MM-dd");
                                command.Parameters.AddWithValue("@date", formattedDate);
                                command.Parameters.AddWithValue("@service", txt_serv.Text);
                                command.ExecuteNonQuery();
                                refresh_table1();
                            }
                        }
                        catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (id != null)
            {
                string sqlQuery = $"SELECT SUM(price) FROM services JOIN services_log ON services.id = services_log.service where paid = 'Нет' AND {id} = services_log.personal_account ";
                double sum = 0;

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
                if (MessageBox.Show("Провести по счету клиента " + name + " расход в сумме " + sum + "?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"INSERT INTO transactions (personal_account, date_transaction, sum, description, complite) VALUES( '" + id + "', '" + DateTime.Now.ToString("yyyy-MM-dd") + "', CAST('-'||@sum AS REAL),   'Оплата за услуги от '||strftime('%d.%m.%Y %H:%M', 'now', 'localtime'), 'Да')", connection);
                            command.Parameters.AddWithValue("@sum", sum);
                            command.ExecuteNonQuery();
                            SQLiteCommand command1 = new SQLiteCommand("UPDATE services_log SET paid = 'Да' where personal_account = " + id, connection);
                            command1.ExecuteNonQuery();
                            MessageBox.Show("Транзакция проведена успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            dataGrid1.ItemsSource = string.Empty;
                        }
                        name = null;
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
            }
            else MessageBox.Show("Сначала выберите клиента");
        }



        private void Clientbtn_Click(object sender, RoutedEventArgs e)
        {
            ClientsWindow clientsWindow = new ClientsWindow();
            clientsWindow.ShowDialog();

            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
            System.Data.DataTable dataTable = new System.Data.DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = dataTable.DefaultView;
            sqlConnection.Close();
        }

        private void Clientbtn_Click_1(object sender, RoutedEventArgs e)
        {
            Services_Window servicesWindow = new Services_Window();
            servicesWindow.ShowDialog();

            sqlConnection.Open();

            txt_serv.Items.Clear();
            string query1 = "SELECT service FROM services";
            SQLiteCommand command = new SQLiteCommand(query1, sqlConnection);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string num = reader.GetString(0);
                    txt_serv.Items.Add(num);
                }
            }
            sqlConnection.Close();
        }

        private void print_Click(object sender, RoutedEventArgs e)
        {
            if (id != null)
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                try
                {
                    var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-услуги_" + name.Replace(" ", "_") + ".xlsx";

                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel (*.xlsx)|*.xlsx",
                        Title = "Export data to an Excel file",
                        FileName = filename
                    };

                    if (saveFileDialog.ShowDialog() != true) return;


                    excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                    workbook.SaveAs(saveFileDialog.FileName);

                    var dataTable = new System.Data.DataTable();
                    using (var connection = new SQLiteConnection(sqlConnection))
                    {
                        connection.Open();
                        using (var command = new SQLiteCommand(query1, connection))
                        {
                            var adapter = new SQLiteDataAdapter(command);
                            adapter.Fill(dataTable);
                        }
                        connection.Close();
                    }

                    var worksheet = workbook.Worksheets.Add();
                    worksheet.Name = "Услуги";

                    // Заполнение ячеек листа из объекта DataTable.
                    worksheet.Cells[1, 1] = DateTime.Now.ToString("dd.MM.yyyy"); ;
                    worksheet.Cells[3, 1] = "Услуга";
                    worksheet.Cells[3, 2] = "Дата оказания услуги";
                    worksheet.Cells[3, 3] = "Цена руб";
                    worksheet.Cells[3, 4] = "Оплачено";
                    worksheet.Cells[1, 1].Font.Bold = true;
                    worksheet.Cells[3, 1].Font.Bold = true;
                    worksheet.Cells[3, 2].Font.Bold = true;
                    worksheet.Cells[3, 3].Font.Bold = true;
                    worksheet.Cells[3, 4].Font.Bold = true;
                    worksheet.Range["A6:D10"].EntireColumn.AutoFit();


                    int row = 4;
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        worksheet.Cells[row, 1] = dataRow["Услуга"];
                        worksheet.Cells[row, 2] = dataRow["Дата"];
                        worksheet.Cells[row, 3] = dataRow["Цена руб"];
                        worksheet.Cells[row, 4] = dataRow["Оплачено"];
                        row++;
                    }

                    worksheet.Cells[row + 2, 1].Font.Bold = true;
                    worksheet.Cells[row + 2, 1] = "Услуги предоставленные клиенту: " + name;
                    // Сохраняем файл
                    workbook.SaveAs(saveFileDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);

                    // Открываем файл на просмотр
                    Process.Start("Excel.exe", saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    workbook.Close();
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);
                }

            }
            else MessageBox.Show("Сначала выберите клиента");
        }


    }
}
