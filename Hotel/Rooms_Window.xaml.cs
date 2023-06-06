using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Rooms_Window.xaml
    /// </summary>
    public partial class Rooms_Window : System.Windows.Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT num AS '№', room_types.type AS 'Тип', floor AS 'Этаж', doplata AS 'Доплата', telephone AS 'Телефон',  rooms.description AS 'Описание',jpg FROM rooms, room_types where room_types.id = rooms.type ORDER BY num";
        string num = null;
        public Rooms_Window()
        {
            InitializeComponent();

            refresh_table();
            string imagePath = "/resouse/default_JPG.jpg";
            var imageUri = new Uri(imagePath, UriKind.Relative);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = imageUri;
            bitmap.EndInit();
            ImageControl.Source = bitmap;

            sqlConnection.Open();
            string query = "SELECT type FROM room_types";
            SQLiteCommand command = new SQLiteCommand(query, sqlConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string num = reader.GetString(0);
                comboBox_type.Items.Add(num);
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
            txt_num.Text = "";
            comboBox_type.Text = "";
            txt_floor.Text = "";
            txt_doplata.Text = "";
            txt_telephone.Text = "";
            txt_description.Text = "";
            ImageControl.Source = null;
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var isValid = Regex.IsMatch(txt_doplata.Text + e.Text, @"\A[0-9]+(?:[.,])?(?:[0-9]{1,2})?\z");
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
            if (txt_num.Text.Length == 0 || comboBox_type.SelectedIndex == -1 || txt_floor.Text.Length == 0 || txt_telephone.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                byte[] imageBytes = null;
                if (ImageControl.Source != null)
                {
                    BitmapSource bitmapSource = (BitmapSource)ImageControl.Source;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                        encoder.Save(ms);
                        imageBytes = ms.ToArray();
                    }
                }
                try
                {
                    sqlConnection.Open();
                    string sql = "INSERT INTO rooms (num,  floor, type, doplata, telephone, description, jpg) " +
                                    "VALUES (@num, @floor, (select room_types.id from room_types where room_types.type = @type), @doplata, @telephone, @description, @jpg)";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                    command.Parameters.AddWithValue("@num", txt_num.Text);
                    command.Parameters.AddWithValue("@floor", txt_floor.Text);
                    command.Parameters.AddWithValue("@type", comboBox_type.Text);
                    command.Parameters.AddWithValue("@doplata", txt_doplata.Text.Replace(",", "."));
                    command.Parameters.AddWithValue("@telephone", txt_telephone.Text);
                    command.Parameters.AddWithValue("@description", txt_description.Text);
                    command.Parameters.AddWithValue("@jpg", imageBytes);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();

                    refresh_table();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); sqlConnection.Close(); }

            }
        }

        private void delButton_Click(object sender, RoutedEventArgs e)
        {

            if (num == null)
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Удалить запись {num}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM rooms WHERE num = '{num}'", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                        }
                        num = null;
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка базы данных.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }


                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_num.Text.Length == 0 || comboBox_type.SelectedIndex == -1 || txt_floor.Text.Length == 0 || txt_telephone.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (num == null)
                    MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    if (MessageBox.Show($"Редактировать запись {num}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                            {
                                connection.Open();
                                byte[] imageBytes = null;
                                if (ImageControl.Source != null)
                                {
                                    BitmapSource bitmapSource = (BitmapSource)ImageControl.Source;
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        BitmapEncoder encoder = new PngBitmapEncoder();
                                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                                        encoder.Save(ms);
                                        imageBytes = ms.ToArray();
                                    }
                                }
                                SQLiteCommand command = new SQLiteCommand("UPDATE rooms SET num = @num1, floor = @floor, type = (select room_types.id from room_types where room_types.type = @type), doplata = @doplata, telephone = @telephone, description = @description, jpg = @jpg WHERE num = @num", connection);
                                command.Parameters.AddWithValue("@num", num);
                                command.Parameters.AddWithValue("@num1", txt_num.Text);
                                command.Parameters.AddWithValue("@floor", txt_floor.Text);
                                command.Parameters.AddWithValue("@type", comboBox_type.Text);
                                command.Parameters.AddWithValue("@doplata", txt_doplata.Text.Replace(",", "."));
                                command.Parameters.AddWithValue("@telephone", txt_telephone.Text);
                                command.Parameters.AddWithValue("@description", txt_description.Text);
                                command.Parameters.AddWithValue("@jpg", imageBytes);
                                command.ExecuteNonQuery();
                                refresh_table();
                            }
                            num = null;
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
                txt_num.Text = Convert.ToString(selectedRow["№"]);
                comboBox_type.Text = Convert.ToString(selectedRow["Тип"]);
                txt_floor.Text = Convert.ToString(selectedRow["Этаж"]);
                txt_doplata.Text = Convert.ToString(selectedRow["Доплата"]);
                txt_telephone.Text = Convert.ToString(selectedRow["Телефон"]);
                txt_description.Text = Convert.ToString(selectedRow["Описание"]);

                num = Convert.ToString(selectedRow["№"]);
                if (!selectedRow.Row.IsNull("jpg"))
                {
                    byte[] imageBytes = (byte[])selectedRow["jpg"];
                    BitmapImage bitmapImage = new BitmapImage();
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = stream;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                    }
                    ImageControl.Source = bitmapImage;

                }
                else
                {
                    string imagePath = "/resouse/default_JPG.jpg";
                    var imageUri = new Uri(imagePath, UriKind.Relative);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = imageUri;
                    bitmap.EndInit();
                    ImageControl.Source = bitmap;
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Создаем диалоговое окно для выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Указываем, что можно выбирать только картинки
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            // Открываем диалоговое окно
            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем путь к выбранной картинке
                string imagePath = openFileDialog.FileName;

                // Делаем что-то с выбранной картинкой
                // Например, отображаем ее в Image элементе управления
                ImageControl.Source = new BitmapImage(new Uri(imagePath));
            }
        }

        private void Button_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Указываем, что можно выбирать только картинки
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            // Открываем диалоговое окно
            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем путь к выбранной картинке
                string imagePath = openFileDialog.FileName;

                // Делаем что-то с выбранной картинкой
                // Например, отображаем ее в Image элементе управления
                ImageControl.Source = new BitmapImage(new Uri(imagePath));
            }
        }

        private void free_print_Click(object sender, RoutedEventArgs e)
        {
            // Создаем Excel приложение
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            try
            {

                var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Свободные_номера.xlsx";

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
                    using (var command = new SQLiteCommand("SELECT num AS n, room_types.type AS ty, floor AS f, doplata AS d, telephone AS te,  rooms.description AS desc FROM rooms, room_types JOIN reservation ON rooms.id = reservation.room where room_types.id = rooms.type AND (date_from > date('now') OR date_to < date('now')) ORDER BY num", connection))
                    {
                        var adapter = new SQLiteDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                    connection.Close();
                }
                excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                // Заполняем лист данными
                Worksheet worksheet = excelApp.ActiveSheet;
                worksheet.Name = "Свободные номера";


                worksheet.Cells[1, 1] = DateTime.Now.ToString("dd.MM.yyyy");
                worksheet.Cells[1, 1].Font.Bold = true;
                worksheet.Range["A1:C1"].Merge();

                worksheet.Range["A3:F3"].Merge();
                worksheet.Cells[3, 1].Font.Bold = true;
                worksheet.Cells[3, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;

                worksheet.Cells[3, 1] = "Свободные номера";
                worksheet.Cells[5, 1] = "Номер";
                worksheet.Cells[5, 2] = "Тип номера";
                worksheet.Cells[5, 3] = "Этаж";
                worksheet.Cells[5, 4] = "Доплата";
                worksheet.Cells[5, 5] = "Телефон";
                worksheet.Cells[5, 6] = "Описание номера";
                worksheet.Range["A5:F5"].Font.Bold = true;
                worksheet.Range["A5:A25"].Font.Bold = true;

                worksheet.Range["A6:F30"].EntireColumn.AutoFit();
                worksheet.Columns[2].ColumnWidth = 20;
                worksheet.Columns[6].ColumnWidth = 50;
                worksheet.Range["D6:D30"].NumberFormat = "0.00";

                // Заполнение ячеек листа из объекта DataTable.
                int row = 6;
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    worksheet.Cells[row, 1] = dataRow["n"];
                    worksheet.Cells[row, 2] = dataRow["ty"];
                    worksheet.Cells[row, 3] = dataRow["f"];
                    worksheet.Cells[row, 4] = dataRow["d"];
                    worksheet.Cells[row, 5] = dataRow["te"];
                    worksheet.Cells[row, 6] = dataRow["desc"];
                    row++;
                }
                var range = worksheet.Range["A5:F" + --row];
                var borders = range.Borders;
                borders.LineStyle = XlLineStyle.xlContinuous;
                borders.Weight = 2d;

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

        private void print_Click(object sender, RoutedEventArgs e)
        {
            // Создаем Excel приложение
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            try
            {

                var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Номерной_фонд.xlsx";

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
                    using (var command = new SQLiteCommand("SELECT num AS n, room_types.type AS ty, floor AS f, doplata AS d, telephone AS te,  rooms.description AS desc,jpg FROM rooms, room_types where room_types.id = rooms.type ORDER BY num", connection))
                    {
                        var adapter = new SQLiteDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                    connection.Close();
                }
                excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                // Заполняем лист данными
                Worksheet worksheet = excelApp.ActiveSheet;
                worksheet.Name = "Номерной фонд";


                worksheet.Cells[1, 1] = DateTime.Now.ToString("dd.MM.yyyy");
                worksheet.Cells[1, 1].Font.Bold = true;
                worksheet.Range["A1:C1"].Merge();

                worksheet.Range["A3:F3"].Merge();
                worksheet.Cells[3, 1].Font.Bold = true;
                worksheet.Cells[3, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;

                worksheet.Cells[3, 1] = "Номера";
                worksheet.Cells[5, 1] = "Номер";
                worksheet.Cells[5, 2] = "Тип номера";
                worksheet.Cells[5, 3] = "Этаж";
                worksheet.Cells[5, 4] = "Доплата";
                worksheet.Cells[5, 5] = "Телефон";
                worksheet.Cells[5, 6] = "Описание номера";
                worksheet.Range["A5:F5"].Font.Bold = true;
                worksheet.Range["A5:A25"].Font.Bold = true;

                worksheet.Range["A6:F30"].EntireColumn.AutoFit();
                worksheet.Columns[2].ColumnWidth = 20;
                worksheet.Columns[6].ColumnWidth = 50;
                worksheet.Range["D6:D30"].NumberFormat = "0.00";

                // Заполнение ячеек листа из объекта DataTable.
                int row = 6;
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    worksheet.Cells[row, 1] = dataRow["n"];
                    worksheet.Cells[row, 2] = dataRow["ty"];
                    worksheet.Cells[row, 3] = dataRow["f"];
                    worksheet.Cells[row, 4] = dataRow["d"];
                    worksheet.Cells[row, 5] = dataRow["te"];
                    worksheet.Cells[row, 6] = dataRow["desc"];
                    row++;
                }
                var range = worksheet.Range["A5:F" + --row];
                var borders = range.Borders;
                borders.LineStyle = XlLineStyle.xlContinuous;
                borders.Weight = 2d;

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
