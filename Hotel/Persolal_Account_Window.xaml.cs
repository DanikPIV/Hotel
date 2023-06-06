using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Persolal_Account_Window.xaml
    /// </summary>
    public partial class Persolal_Account_Window : System.Windows.Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT name AS 'ФИО', id AS 'Лиц счет', pasport AS 'Паспорт', address AS 'Адрес', clients.description AS 'Описание' FROM clients";
        string query1 = "SELECT complite as 'Проведено', id as '№ транзакции', strftime('%d.%m.%Y', date_transaction) as 'Дата', CAST(sum  as text) AS 'Сумма руб', description AS 'Назначение' FROM  transactions where personal_acount = 0";
        string name = null;
        string id = null;
        string id_t = null;
        public Persolal_Account_Window()
        {
            InitializeComponent();

            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
            System.Data.DataTable dataTable = new System.Data.DataTable();
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
            System.Data.DataTable dataTable = new System.Data.DataTable();
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
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var isValid = Regex.IsMatch(txt_sum.Text + e.Text, @"\A-?(?:[0-9]+)?(?:[.,])?(?:[0-9]{1,2})?\z");
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
            if (txt_sum.Text.Length == 0 || date_picker_t.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Convert.ToDateTime(date_picker_t.Text) >= DateTime.Now) MessageBox.Show("Неверная дата", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (id == null) MessageBox.Show("Выберите клиента!");
            else
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "INSERT INTO transactions (date_transaction, sum, personal_account, description) VALUES (@date1, @sum , @personal_account, @description)";

                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);

                    DateTime date = DateTime.Parse(date_picker_t.Text);
                    string formattedDate = date.ToString("yyyy-MM-dd");
                    command.Parameters.AddWithValue("@date1", formattedDate);
                    command.Parameters.AddWithValue("@sum", txt_sum.Text.Replace(",", "."));
                    command.Parameters.AddWithValue("@personal_account", id);
                    command.Parameters.AddWithValue("@description", txt_description.Text);
                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                    refresh_table1();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); sqlConnection.Close(); }

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
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM transactions WHERE id = '{id_t}'", connection);
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
                                SQLiteCommand command = new SQLiteCommand("UPDATE transactions SET sum = @sum,  date_transaction = @date1, description = @description WHERE id = @id_t", connection);
                                command.Parameters.AddWithValue("@id_t", id_t);

                                DateTime date = DateTime.Parse(date_picker_t.Text);
                                string formattedDate = date.ToString("yyyy-MM-dd");
                                command.Parameters.AddWithValue("@date1", formattedDate);
                                command.Parameters.AddWithValue("@sum", txt_sum.Text.Replace(",", "."));
                                command.Parameters.AddWithValue("@personal_account", id);
                                command.Parameters.AddWithValue("@description", txt_description.Text);
                                command.ExecuteNonQuery();
                                refresh_table1();
                            }
                            id_t = null;
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
                id = Convert.ToString(selectedRow["Лиц счет"]);
                name = Convert.ToString(selectedRow["ФИО"]);
                query1 = "SELECT complite as 'Проведено', id as '№ транзакции', strftime('%d.%m.%Y', date_transaction) as 'Дата',  CAST(sum  as text) AS 'Сумма руб', description AS 'Назначение' FROM  transactions where personal_account = " + id;
                refresh_table1();
            }
        }

        private void ShowTransactionSum()
        {
            string sqlQuery = "SELECT SUM(sum) FROM transactions where complite = 'Да' AND personal_account = " + id;
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

            txt_balance.Text = "Остаток: " + sum.ToString("F2") + " руб.";
            if (sum > 0)
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

        private void Clientbtn_Click(object sender, RoutedEventArgs e)
        {
            ClientsWindow clientsWindow = new ClientsWindow();
            clientsWindow.ShowDialog();
        }

        private void dolgBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Должники.xlsx";

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
                    using (var command = new SQLiteCommand("select clients.id as i, clients.name as n, status_clients.status as s, printf(\"%.2f\", -1*SUM(sum)) as sm from transactions JOIN clients on clients.id = personal_account JOIN status_clients ON status_clients.id = clients.status   GROUP BY transactions.personal_account having -1*SUM(sum) > 0", connection))
                    {
                        var adapter = new SQLiteDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                    connection.Close();
                }
                // Создаем Excel приложение
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                // Заполняем лист данными
                Worksheet worksheet = excelApp.ActiveSheet;
                worksheet.Name = "Clients";
                worksheet.Name = "Услуги";


                worksheet.Cells[1, 1] = "=СЕГОДНЯ()";
                worksheet.Range["A3:D3"].Merge();
                worksheet.Cells[5, 1] = "№";
                worksheet.Cells[5, 2] = "ФИО клиента";
                worksheet.Cells[5, 3] = "Статус";
                worksheet.Cells[5, 4] = "Задолженность, руб.";
                worksheet.Cells[1, 1].Font.Bold = true;
                worksheet.Range["A3:D3"].Font.Bold = true;
                worksheet.Range["A6:D30"].EntireColumn.AutoFit();
                worksheet.Columns[2].ColumnWidth = 225;
                worksheet.Columns[2].ColumnWidth = 90;
                worksheet.Range["D6:D30"].NumberFormat = "0.00";
                // Заполнение ячеек листа из объекта DataTable.
                int row = 6;
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    worksheet.Cells[row, 1] = dataRow["i"];
                    worksheet.Cells[row, 2] = dataRow["n"];
                    worksheet.Cells[row, 3] = dataRow["s"];
                    worksheet.Cells[row, 4] = dataRow["sm"];
                    row++;
                }
                var range = worksheet.Range["A5:D" + --row];
                var borders = range.Borders;
                borders.LineStyle = XlLineStyle.xlContinuous;
                borders.Weight = 2d;

                worksheet.Cells[3, 1].Font.Bold = true;
                worksheet.Cells[3, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                worksheet.Cells[3, 1] = "Должники";
                // Сохраняем файл
                workbook.SaveAs(saveFileDialog.FileName);
                workbook.Close();
                excelApp.Quit();
                Marshal.ReleaseComObject(excelApp);

                // Открываем файл на просмотр
                Process.Start("Excel.exe", saveFileDialog.FileName);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка сохранения.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

        }

        private void print_Click(object sender, RoutedEventArgs e)
        {
            if (id != null)
            {
                // Создаем Excel приложение
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                try
                {

                    var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Движение_по_счету_" + id + ".xlsx";

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
                        using (var command = new SQLiteCommand(query1, connection))
                        {
                            var adapter = new SQLiteDataAdapter(command);
                            adapter.Fill(dataTable);
                        }
                        connection.Close();
                    }
                    excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                    // Заполняем лист данными
                    Worksheet worksheet = excelApp.ActiveSheet;
                    worksheet.Name = "Движение_по_счету_" + id;


                    worksheet.Cells[1, 1] = DateTime.Now.ToString("dd.MM.yyyy");
                    worksheet.Cells[1, 1].Font.Bold = true;
                    worksheet.Range["A1:C1"].Merge();

                    worksheet.Range["A3:E3"].Merge();
                    worksheet.Cells[3, 1].Font.Bold = true;
                    worksheet.Cells[3, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    worksheet.Cells[3, 1] = "Движение по по счету " + id + " клиента: " + name;

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cells[5, i + 1] = dataTable.Columns[i].ColumnName;
                    }
                    worksheet.Range["A5:Z5"].Font.Bold = true;
                    //worksheet.Range["A5:A25"].Font.Bold = true;

                    worksheet.Range["A6:I30"].EntireColumn.AutoFit();
                    worksheet.Columns[3].ColumnWidth = 12;
                    worksheet.Columns[4].ColumnWidth = 12;
                    worksheet.Columns[5].ColumnWidth = 80;
                    //worksheet.Range["F:G"].NumberFormat = "0.00";
                    worksheet.Range["D6:D50"].NumberFormat = "0.00";
                    worksheet.Range["A:Z"].Font.Name = "Times New Roman";

                    // Заполнение ячеек листа из объекта DataTable.
                    int row = 6;
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            worksheet.Cells[row, i + 1] = dataRow[dataTable.Columns[i].ColumnName];
                        }
                        row++;
                    }

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
            else MessageBox.Show("Сначала выбери клиента");
        }

        private void print_pay_Click(object sender, RoutedEventArgs e)
        {
            if (id != null)
            {
                MessggeBoks messgge = new MessggeBoks(id);
                messgge.ShowDialog();

            }
            else MessageBox.Show("Сначала выбери клиента");
        }
    }
}
