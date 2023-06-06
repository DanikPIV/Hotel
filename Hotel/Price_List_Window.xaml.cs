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

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Price_List_Window.xaml
    /// </summary>
    public partial class Price_List_Window : System.Windows.Window
    {//
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=hotel.db");
        string query = "SELECT CAST(price_list.id AS CHAR(15)) AS 'Шифр', room_types.type AS 'Тип', strftime('%d.%m.%Y', date_from) AS 'Действует с', strftime('%d.%m.%Y', date_to) AS 'по', holyday AS 'Выходной', valid AS 'Действует', CAST( price as text) AS 'Цена в сутки', CAST(reservation_price as text) AS 'Цена брони в сутки' FROM price_list, room_types where price_list.room_type = room_types.id";
        string id = null;
        public Price_List_Window()
        {
            InitializeComponent();

            refresh_table();

            sqlConnection.Open();
            string query = "SELECT type FROM room_types";
            SQLiteCommand command = new SQLiteCommand(query, sqlConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string type = reader.GetString(0);
                comboBox_type.Items.Add(type);
            }
            sqlConnection.Close();

        }
        public void refresh_table()
        {
            sqlConnection.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, sqlConnection);
            System.Data.DataTable dataTable = new System.Data.DataTable();
            dataAdapter.Fill(dataTable);
            dataGrid.ItemsSource = dataTable.DefaultView;
            ClearTxt();
            sqlConnection.Close();
        }

        public void ClearTxt()
        {
            comboBox_type.Text = "";
            date_picker1.Text = "";
            date_picker2.Text = "";
            comboBox_holiday.Text = "";
            comboBox_valid.Text = "";
            txt_price.Text = "";
            txt_reservation_price.Text = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_type.Text.Length == 0 || date_picker1.Text.Length == 0 || date_picker2.Text.Length == 0 || comboBox_holiday.SelectedIndex == -1
            || comboBox_valid.Text.Length == 0 || txt_price.Text.Length == 0 || txt_reservation_price.Text.Length == 0)
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

                    string sql = "INSERT INTO price_list (room_type, date_from,  date_to, holyday, valid, price, reservation_price) " +
                                    "VALUES ((select id from room_types where type like @room_type), @date1, @date2, @holyday, @valid, @price, @reservation_price)";

                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                    command.Parameters.AddWithValue("@room_type", comboBox_type.Text);

                    DateTime date = DateTime.Parse(date_picker1.Text);
                    string formattedDate = date.ToString("yyyy-MM-dd");
                    command.Parameters.AddWithValue("@date1", formattedDate);

                    date = DateTime.Parse(date_picker2.Text);
                    formattedDate = date.ToString("yyyy-MM-dd");
                    command.Parameters.AddWithValue("@date2", formattedDate);

                    command.Parameters.AddWithValue("@holyday", comboBox_holiday.Text);
                    command.Parameters.AddWithValue("@valid", comboBox_valid.Text);
                    command.Parameters.AddWithValue("@price", txt_price.Text.Replace(",", "."));
                    command.Parameters.AddWithValue("@reservation_price", txt_reservation_price.Text.Replace(",", "."));
                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                    refresh_table();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); sqlConnection.Close(); }

            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var isValid = Regex.IsMatch(txt_price.Text + e.Text, @"\A[0-9]+(?:[.,])?(?:[0-9]{1,2})?\z");
            if (!isValid)
            {
                e.Handled = true;
            }
        }

        private void TextBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var isValid = Regex.IsMatch(txt_reservation_price.Text + e.Text, @"\A[0-9]+(?:[.,])?(?:[0-9]{1,2})?\z");
            if (!isValid)
            {
                e.Handled = true;
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
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM price_list WHERE id = '{id}'", connection);
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
            if (comboBox_type.Text.Length == 0 || date_picker1.Text.Length == 0 || date_picker2.Text.Length == 0 || comboBox_holiday.SelectedIndex == -1
            || comboBox_valid.Text.Length == 0 || txt_price.Text.Length == 0 || txt_reservation_price.Text.Length == 0)
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
                                SQLiteCommand command = new SQLiteCommand("UPDATE price_list SET room_type = (select id from room_types where type like @room_type), date_from =   @date1, date_to = @date2, holyday = @holyday, valid = @valid, price = @price, reservation_price = @reservation_price WHERE id = @id", connection);
                                command.Parameters.AddWithValue("@id", id);
                                command.Parameters.AddWithValue("@room_type", comboBox_type.Text);

                                DateTime date = DateTime.Parse(date_picker1.Text);
                                string formattedDate = date.ToString("yyyy-MM-dd");
                                command.Parameters.AddWithValue("@date1", formattedDate);

                                date = DateTime.Parse(date_picker2.Text);
                                formattedDate = date.ToString("yyyy-MM-dd");
                                command.Parameters.AddWithValue("@date2", formattedDate);

                                command.Parameters.AddWithValue("@holyday", comboBox_holiday.Text);
                                command.Parameters.AddWithValue("@valid", comboBox_valid.Text);
                                command.Parameters.AddWithValue("@price", txt_price.Text.Replace(",", "."));
                                command.Parameters.AddWithValue("@reservation_price", txt_reservation_price.Text.Replace(",", "."));
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
                comboBox_type.Text = Convert.ToString(selectedRow["Тип"]);
                date_picker1.Text = Convert.ToString(selectedRow["Действует с"]);
                date_picker2.Text = Convert.ToString(selectedRow["По"]);
                comboBox_holiday.Text = Convert.ToString(selectedRow["Выходной"]);
                comboBox_valid.Text = Convert.ToString(selectedRow["Действует"]);
                txt_price.Text = Convert.ToString(selectedRow["Цена в сутки"]);
                txt_reservation_price.Text = Convert.ToString(selectedRow["Цена брони в сутки"]);

                id = Convert.ToString(selectedRow["Шифр"]);
            }
        }

        private void CheckBox1_Checked(object sender, RoutedEventArgs e)
        {

            query = "SELECT CAST(price_list.id AS CHAR(15)) AS 'Шифр', room_types.type AS 'Тип', strftime('%d.%m.%Y', date_from) AS 'Действует с', strftime('%d.%m.%Y', date_to) AS 'по', holyday AS 'Выходной', valid AS 'Действует', CAST( price as text) AS 'Цена в сутки', CAST(reservation_price as text) AS 'Цена брони в сутки' FROM price_list, room_types where price_list.room_type = room_types.id AND valid = 'Да'";
            refresh_table();

        }

        private void CheckBox1_UnChecked(object sender, RoutedEventArgs e)
        {
            query = "SELECT CAST(price_list.id AS CHAR(15)) AS 'Шифр', room_types.type AS 'Тип', strftime('%d.%m.%Y', date_from) AS 'Действует с', strftime('%d.%m.%Y', date_to) AS 'по', holyday AS 'Выходной', valid AS 'Действует', CAST( price as text) AS 'Цена в сутки', CAST(reservation_price as text) AS 'Цена брони в сутки' FROM price_list, room_types where price_list.room_type = room_types.id";
            refresh_table();
        }

        private void date_picker_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void print_Click(object sender, RoutedEventArgs e)
        {
            // Создаем Excel приложение
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            try
            {

                var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Прайс-лист.xlsx";

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
                    using (var command = new SQLiteCommand("SELECT CAST(price_list.id AS CHAR(15)) AS 'Шифр', room_types.type AS 'Тип', strftime('%d.%m.%Y', date_from) AS 'Действует с', strftime('%d.%m.%Y', date_to) AS 'по', holyday AS 'Выходной', valid AS 'Действует', CAST( price as text) AS 'Цена в сутки', CAST(reservation_price as text) AS 'Цена брони в сутки' FROM price_list, room_types where price_list.room_type = room_types.id", connection))
                    {
                        var adapter = new SQLiteDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                    connection.Close();
                }
                excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                // Заполняем лист данными
                Worksheet worksheet = excelApp.ActiveSheet;
                worksheet.Name = "Прайс-лист";


                worksheet.Cells[1, 1] = DateTime.Now.ToString("dd.MM.yyyy");
                worksheet.Cells[1, 1].Font.Bold = true;
                worksheet.Range["A1:C1"].Merge();

                worksheet.Range["A3:H3"].Merge();
                worksheet.Cells[3, 1].Font.Bold = true;
                worksheet.Cells[3, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                worksheet.Cells[3, 1] = "Прайс-лист";

                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    worksheet.Cells[5, i + 1] = dataTable.Columns[i].ColumnName;
                }
                worksheet.Range["A5:H5"].Font.Bold = true;
                worksheet.Range["A5:A25"].Font.Bold = true;

                worksheet.Range["A6:H30"].EntireColumn.AutoFit();
                worksheet.Columns[2].ColumnWidth = 23;
                worksheet.Columns[3].ColumnWidth = 12;
                worksheet.Columns[4].ColumnWidth = 12;
                worksheet.Range["G:H"].NumberFormat = "0.00";
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
