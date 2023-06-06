using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для ClientsWindow.xaml
    /// </summary>
    public partial class ClientsWindow : System.Windows.Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT name AS 'ФИО', status_clients.status AS 'Статус', pasport AS 'Паспорт', gender AS 'Пол', strftime('%d.%m.%Y', birthday) AS 'Дата рождения', address AS 'Адрес', clients.description AS 'Описание' FROM clients, status_clients where status_clients.id = clients.status";
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
            try
            {
                sqlConnection.Open();
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataAdapter.Fill(dataTable);
                dataGrid.ItemsSource = dataTable.DefaultView;
                ClearTxt();
                sqlConnection.Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message); sqlConnection.Close(); }
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
                                    "VALUES (@name, (select id from status_clients where status like '%'||@status||'%'), @pasport, @gender, @birthday, @address, @description)";

                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                    command.Parameters.AddWithValue("@name", txt_name.Text);
                    command.Parameters.AddWithValue("@status", comboBox_status.Text);
                    command.Parameters.AddWithValue("@pasport", txt_pasport.Text);
                    command.Parameters.AddWithValue("@gender", comboBox_gender.Text);
                    DateTime date = DateTime.ParseExact(data_picker_birthday.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    string newDateStr = date.ToString("yyyy-MM-dd");
                    command.Parameters.AddWithValue("@birthday", newDateStr);
                    command.Parameters.AddWithValue("@address", txt_address.Text);
                    command.Parameters.AddWithValue("@description", txt_description.Text);
                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                    refresh_table();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); sqlConnection.Close(); }

            }
        }



        private void txt_find_KeyUp(object sender, KeyEventArgs e)
        {
            if (txt_find.Text == "")
                query = "SELECT  name AS 'ФИО', status_clients.status AS 'Статус', pasport AS 'Паспорт', gender AS 'Пол', strftime('%d.%m.%Y', birthday) AS 'Дата рождения', address AS 'Адрес', clients.description AS 'Описание' FROM clients, status_clients where status_clients.id = clients.status";
            else
                query = "SELECT  name AS 'ФИО', status_clients.status AS 'Статус', pasport AS 'Паспорт', gender AS 'Пол', strftime('%d.%m.%Y', birthday) AS 'Дата рождения', address AS 'Адрес', clients.description AS 'Описание' FROM clients, status_clients where status_clients.id = clients.status AND name Like '%'||@find||'%'";
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
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM clients WHERE name = '{name}'", connection);
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
                                SQLiteCommand command = new SQLiteCommand("UPDATE clients SET name = @name, status =  (select id from status_clients where status like '%'||@status||'%'), pasport = @pasport, gender = @gender, birthday = @birthday, address = @address, description = @description WHERE name = @name1", connection);
                                command.Parameters.AddWithValue("@name1", name);
                                command.Parameters.AddWithValue("@name", txt_name.Text);
                                command.Parameters.AddWithValue("@status", comboBox_status.Text);
                                command.Parameters.AddWithValue("@pasport", txt_pasport.Text);
                                command.Parameters.AddWithValue("@gender", comboBox_gender.Text);
                                DateTime date = DateTime.ParseExact(data_picker_birthday.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                                string newDateStr = date.ToString("yyyy-MM-dd");
                                command.Parameters.AddWithValue("@birthday", newDateStr);
                                command.Parameters.AddWithValue("@address", txt_address.Text);
                                command.Parameters.AddWithValue("@description", txt_description.Text);
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
                comboBox_status.Text = Convert.ToString(selectedRow["Статус"]);
                txt_pasport.Text = Convert.ToString(selectedRow["Паспорт"]);
                comboBox_gender.Text = Convert.ToString(selectedRow["Пол"]);
                data_picker_birthday.Text = Convert.ToString(selectedRow["Дата рождения"]);
                txt_address.Text = Convert.ToString(selectedRow["Адрес"]);
                txt_description.Text = Convert.ToString(selectedRow["Описание"]);

                name = Convert.ToString(selectedRow["ФИО"]);
            }
        }

        private void print_Click(object sender, RoutedEventArgs e)
        {
            // Создаем Excel приложение
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            try
            {

                var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Клиенты.xlsx";

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel (*.xlsx)|*.xlsx",
                    Title = "Export data to an Excel file",
                    FileName = filename
                };
                if (saveFileDialog.ShowDialog() != true)
                    return;


                SQLiteConnection conn = new SQLiteConnection("Data Source=hotel.db");

                var dataTable = new System.Data.DataTable();
                using (var connection = new SQLiteConnection(conn))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand("SELECT name AS n, status_clients.status AS s, pasport AS p, gender AS g, strftime('%d.%m.%Y', birthday) AS b, address AS a, clients.description AS d FROM clients, status_clients where status_clients.id = clients.status", connection))
                    {
                        var adapter = new SQLiteDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                    connection.Close();
                }
                excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                // Заполняем лист данными
                Worksheet worksheet = excelApp.ActiveSheet;
                worksheet.Name = "Клиенты";


                worksheet.Cells[1, 1] = DateTime.Now.ToString("dd.MM.yyyy");
                worksheet.Cells[1, 1].Font.Bold = true;
                worksheet.Range["A1:C1"].Merge();

                worksheet.Range["A3:G3"].Merge();
                worksheet.Cells[3, 1].Font.Bold = true;
                worksheet.Cells[3, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                worksheet.Cells[3, 1] = "Клиенты";

                worksheet.Cells[5, 1] = "ФИО";
                worksheet.Cells[5, 2] = "Статус";
                worksheet.Cells[5, 3] = "Паспорт";
                worksheet.Cells[5, 4] = "Пол";
                worksheet.Cells[5, 5] = "Дата рождения";
                worksheet.Cells[5, 6] = "Адрес";
                worksheet.Cells[5, 7] = "Описание";
                worksheet.Range["A5:G5"].Font.Bold = true;
                worksheet.Range["A5:A25"].Font.Bold = true;

                //worksheet.Range["A6:F30"].EntireColumn.AutoFit();
                worksheet.Columns[1].ColumnWidth = 35;
                worksheet.Columns[2].ColumnWidth = 13;
                worksheet.Columns[3].ColumnWidth = 80;
                worksheet.Columns[4].ColumnWidth = 12;
                worksheet.Columns[5].ColumnWidth = 15;
                worksheet.Columns[6].ColumnWidth = 50;
                worksheet.Columns[7].ColumnWidth = 50;
                //worksheet.Range["D6:D30"].NumberFormat = "0.00";
                worksheet.Range["A:Z"].Font.Name = "Times New Roman";

                // Заполнение ячеек листа из объекта DataTable.
                int row = 6;
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    worksheet.Cells[row, 1] = dataRow["n"];
                    worksheet.Cells[row, 2] = dataRow["s"];
                    worksheet.Cells[row, 3] = dataRow["p"];
                    worksheet.Cells[row, 4] = dataRow["g"];
                    worksheet.Cells[row, 5] = dataRow["b"];
                    worksheet.Cells[row, 6] = dataRow["a"];
                    worksheet.Cells[row, 7] = dataRow["d"];
                    row++;
                }
                //var range = worksheet.Range["A5:F" + --row];
                //var borders = range.Borders;
                //borders.LineStyle = XlLineStyle.xlContinuous;
                //borders.Weight = 2d;

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
    }
}
