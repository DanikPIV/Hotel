using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для MessggeBoks.xaml
    /// </summary>
    public partial class MessggeBoks : System.Windows.Window
    {
        public MessggeBoks()
        {
            InitializeComponent();
        }
        public MessggeBoks(string id)
        {
            InitializeComponent();
            this.id = id;
        }
        string id;

        private void Cencel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            if (date_picker1.Text.Length == 0 || date_picker2.Text.Length == 0)
                MessageBox.Show("Выберите");
            else
            {
                // Создаем Excel приложение
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                try
                {

                    var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Счет_на_оплату.xlsx";

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
                        using (var command = new SQLiteCommand("select id as №, date_transaction as Дата, description as Услуга , sum as Сумма from transactions where date_transaction > @date1 AND date_transaction < @date2 AND complite = 'Нет' AND personal_account =" + id, connection))
                        {
                            DateTime date = DateTime.Parse(date_picker1.Text);
                            string formattedDate = date.ToString("yyyy-MM-dd");
                            command.Parameters.AddWithValue("@date1", formattedDate);
                            date = DateTime.Parse(date_picker2.Text);
                            formattedDate = date.ToString("yyyy-MM-dd");
                            command.Parameters.AddWithValue("@date2", formattedDate);

                            var adapter = new SQLiteDataAdapter(command);
                            adapter.Fill(dataTable);
                        }
                        connection.Close();
                    }
                    excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                    // Заполняем лист данными
                    Worksheet worksheet = excelApp.ActiveSheet;
                    worksheet.Name = "Счет_на_оплату";


                    worksheet.Cells[1, 1] = DateTime.Now.ToString("dd.MM.yyyy");
                    worksheet.Cells[1, 1].Font.Bold = true;
                    worksheet.Range["A1:C1"].Merge();

                    worksheet.Range["A3:E3"].Merge();
                    worksheet.Cells[3, 1].Font.Bold = true;
                    worksheet.Cells[3, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    worksheet.Cells[3, 1] = "Счет №000185215 от" + DateTime.Now.ToString("dd.MM.yyyy"); ;

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cells[5, i + 1] = dataTable.Columns[i].ColumnName;
                    }
                    worksheet.Range["A5:Z5"].Font.Bold = true;
                    //worksheet.Range["A5:A25"].Font.Bold = true;

                    worksheet.Range["A6:I30"].EntireColumn.AutoFit();
                    worksheet.Columns[2].ColumnWidth = 12;
                    worksheet.Columns[4].ColumnWidth = 12;
                    worksheet.Columns[3].ColumnWidth = 100;
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

                    worksheet.Cells[row, 3].Font.Bold = true;
                    worksheet.Cells[row, 4].Font.Bold = true;
                    worksheet.Cells[row, 4] = "=СУММ(D6:D" + row + ")";
                    worksheet.Cells[row, 3].HorizontalAlignment = XlHAlign.xlHAlignRight;
                    worksheet.Cells[row, 3] = "Итого:";

                    var range = worksheet.Range["A5:D" + --row];
                    var borders = range.Borders;
                    borders.LineStyle = XlLineStyle.xlContinuous;
                    borders.Weight = 2d;
                    range = worksheet.Range["D" + (row - 1) + ":D" + row];
                    borders = range.Borders;
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
}
