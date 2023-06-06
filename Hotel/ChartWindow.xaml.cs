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
    /// Логика взаимодействия для ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : System.Windows.Window
    {
        public ChartWindow()
        {
            InitializeComponent();
        }

        private void print4_Click(object sender, RoutedEventArgs e)
        {
            if (date_picker1.Text.Length == 0 || date_picker2.Text.Length == 0)
                MessageBox.Show("Выберите");
            else
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();

                try
                {

                    var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Доходность_по_клиентам.xlsx";

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
                        using (var command = new SQLiteCommand("select name as Клиенты, SUM(sum) as 'Доходность по клиентам' from transactions JOIN clients on clients.id = personal_account where sum > 0 AND date_transaction > @date1 AND date_transaction < @date2 group by personal_account", connection))
                        {
                            DateTime date = DateTime.Parse(date_picker1.Text);
                            string formattedDate = date.ToString("yyyy-MM-dd");
                            command.Parameters.AddWithValue("@date1", formattedDate);
                            date = DateTime.Parse(date_picker2.Text);
                            formattedDate = date.ToString("yyyy-MM-dd");
                            command.Parameters.AddWithValue("@date2", formattedDate);

                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                            adapter.Fill(dataTable);
                        }
                        connection.Close();
                    }
                    excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                    // Заполняем лист данными
                    Worksheet worksheet = excelApp.ActiveSheet;
                    worksheet.Name = "Доходность_по_клиентам";


                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1] = dataTable.Columns[i].ColumnName;
                    }
                    worksheet.Range["A1:B1"].Font.Bold = true;

                    worksheet.Range["A1:I30"].EntireColumn.AutoFit();
                    worksheet.Range["A:Z"].Font.Name = "Times New Roman";

                    // Заполнение ячеек листа из объекта DataTable.
                    int row = 2;
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            worksheet.Cells[row, i + 1] = dataRow[dataTable.Columns[i].ColumnName];
                        }
                        row++;
                    }


                    ChartObjects chartObjects = (ChartObjects)worksheet.ChartObjects(Type.Missing);
                    ChartObject chartObject = chartObjects.Add(250, 20, 400, 300);
                    Chart chart = chartObject.Chart;

                    // Добавьте данные в диаграмму
                    Range range = worksheet.get_Range("A1:B" + (row - 1));
                    chart.SetSourceData(range);

                    // Установите тип диаграммы
                    chart.ChartType = XlChartType.xlPieExploded;

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
        private void print_Click(object sender, RoutedEventArgs e)
        {
            if (date_picker1.Text.Length == 0 || date_picker2.Text.Length == 0)
                MessageBox.Show("Выберите");
            else
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();

                try
                {

                    var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Популярность_номеров.xlsx";

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
                        using (var command = new SQLiteCommand("select num as Номер, julianday(date_to)- julianday(date_from) as 'Кол-во дней сдачи/брони' from reservation JOIN rooms ON rooms.id = room where (date_to > @date1 AND date_from < @date1) OR (date_to > @date2 AND date_from < @date2) ORDER BY num", connection))
                        {
                            DateTime date = DateTime.Parse(date_picker1.Text);
                            string formattedDate = date.ToString("yyyy-MM-dd");
                            command.Parameters.AddWithValue("@date1", formattedDate);
                            date = DateTime.Parse(date_picker2.Text);
                            formattedDate = date.ToString("yyyy-MM-dd");
                            command.Parameters.AddWithValue("@date2", formattedDate);

                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                            adapter.Fill(dataTable);
                        }
                        connection.Close();
                    }
                    excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                    // Заполняем лист данными
                    Worksheet worksheet = excelApp.ActiveSheet;



                    worksheet.Cells[1, 2] = dataTable.Columns[1].ColumnName;

                    worksheet.Range["A1:B1"].Font.Bold = true;

                    worksheet.Range["A1:I30"].EntireColumn.AutoFit();
                    worksheet.Range["A:Z"].Font.Name = "Times New Roman";

                    // Заполнение ячеек листа из объекта DataTable.
                    int row = 2;
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            worksheet.Cells[row, i + 1] = dataRow[dataTable.Columns[i].ColumnName];
                        }
                        row++;
                    }


                    ChartObjects chartObjects = (ChartObjects)worksheet.ChartObjects(Type.Missing);
                    ChartObject chartObject = chartObjects.Add(250, 20, 400, 300);
                    Chart chart = chartObject.Chart;

                    // Добавьте данные в диаграмму
                    Range range = worksheet.get_Range("A1:B" + (row - 1));
                    chart.SetSourceData(range);

                    // Установите тип диаграммы
                    chart.ChartType = XlChartType.xlColumnClustered;

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

        private void print1_Click(object sender, RoutedEventArgs e)
        {
            if (date_picker1.Text.Length == 0 || date_picker2.Text.Length == 0)
                MessageBox.Show("Выберите");
            else
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();

                try
                {

                    var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Доходность_номеров.xlsx";

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
                        using (var command = new SQLiteCommand("select num , (julianday(reservation.date_to)- julianday(reservation.date_from))* price as r from reservation JOIN rooms ON rooms.id = room JOIN room_types ON rooms.type = room_types.id JOIN price_list ON room_type = room_types.id  where ((reservation.date_to > @date1 AND reservation.date_to < @date2) OR (reservation.date_From > @date1 AND reservation.date_from < @date2)) AND price_list.holyday = 'Да' ORDER BY r", connection))
                        {
                            DateTime date = DateTime.Parse(date_picker1.Text);
                            string formattedDate = date.ToString("yyyy-MM-dd");
                            command.Parameters.AddWithValue("@date1", formattedDate);
                            date = DateTime.Parse(date_picker2.Text);
                            formattedDate = date.ToString("yyyy-MM-dd");
                            command.Parameters.AddWithValue("@date2", formattedDate);

                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                            adapter.Fill(dataTable);
                        }
                        connection.Close();
                    }
                    excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                    // Заполняем лист данными
                    Worksheet worksheet = excelApp.ActiveSheet;



                    worksheet.Cells[1, 2] = "Доходность номеров";

                    worksheet.Range["A1:B1"].Font.Bold = true;

                    worksheet.Range["A1:I30"].EntireColumn.AutoFit();
                    worksheet.Range["A:Z"].Font.Name = "Times New Roman";

                    // Заполнение ячеек листа из объекта DataTable.
                    int row = 2;
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            worksheet.Cells[row, i + 1] = dataRow[dataTable.Columns[i].ColumnName];
                        }
                        row++;
                    }


                    ChartObjects chartObjects = (ChartObjects)worksheet.ChartObjects(Type.Missing);
                    ChartObject chartObject = chartObjects.Add(250, 20, 400, 300);
                    Chart chart = chartObject.Chart;

                    // Добавьте данные в диаграмму
                    Range range = worksheet.get_Range("A1:B" + (row - 1));
                    chart.SetSourceData(range);

                    // Установите тип диаграммы
                    chart.ChartType = XlChartType.xlColumnClustered;

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

        private void print3_Click(object sender, RoutedEventArgs e)
        {
            if (date_picker1.Text.Length == 0 || date_picker2.Text.Length == 0)
                MessageBox.Show("Выберите");
            else
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();

                try
                {

                    var filename = DateTime.Now.ToString("dd-MM-yyyy") + "-Динамика_доходности.xlsx";

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
                        using (var command = new SQLiteCommand("SELECT strftime('%m-%Y', date_transaction), SUM(sum) FROM transactions where sum > 0 GROUP BY strftime('%m-%Y', date_transaction)", connection))
                        {
                            DateTime date = DateTime.Parse(date_picker1.Text);
                            string formattedDate = date.ToString("yyyy-MM-dd");
                            command.Parameters.AddWithValue("@date1", formattedDate);
                            date = DateTime.Parse(date_picker2.Text);
                            formattedDate = date.ToString("yyyy-MM-dd");
                            command.Parameters.AddWithValue("@date2", formattedDate);

                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                            adapter.Fill(dataTable);
                        }
                        connection.Close();
                    }
                    excelApp.DisplayAlerts = false; // выключить диалоги предупреждений.
                                                    // Заполняем лист данными
                    Worksheet worksheet = excelApp.ActiveSheet;



                    worksheet.Cells[1, 2] = "Динамика выручки";

                    worksheet.Range["A1:B1"].Font.Bold = true;

                    worksheet.Range["A1:I30"].EntireColumn.AutoFit();
                    worksheet.Range["A:Z"].Font.Name = "Times New Roman";

                    // Заполнение ячеек листа из объекта DataTable.
                    int row = 2;
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            worksheet.Cells[row, i + 1] = dataRow[dataTable.Columns[i].ColumnName];
                        }
                        row++;
                    }


                    ChartObjects chartObjects = (ChartObjects)worksheet.ChartObjects(Type.Missing);
                    ChartObject chartObject = chartObjects.Add(250, 20, 400, 300);
                    Chart chart = chartObject.Chart;

                    // Добавьте данные в диаграмму
                    Range range = worksheet.get_Range("A1:B" + (row - 1));
                    chart.SetSourceData(range);

                    // Установите тип диаграммы
                    chart.ChartType = XlChartType.xlLine;

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
