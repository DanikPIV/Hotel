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
    /// Логика взаимодействия для MainWin.xaml
    /// </summary>
    public partial class MainWin : System.Windows.Window
    {
        public MainWin()
        {
            InitializeComponent();
        }

        private void Button_reservation_Click(object sender, RoutedEventArgs e)
        {
            Reservation_Window reservation_Window = new Reservation_Window();
            reservation_Window.ShowDialog();
        }

        private void MenuItem_Clients_Click(object sender, RoutedEventArgs e)
        {
            ClientsWindow clientsWindow = new ClientsWindow();
            clientsWindow.ShowDialog();
        }
        private void MenuItem_Status_Clients_Click(object sender, RoutedEventArgs e)
        {
            Status_Clients_Window sclientsWindow = new Status_Clients_Window();
            sclientsWindow.ShowDialog();
        }


        private void Button_personal_account_Click(object sender, RoutedEventArgs e)
        {
            Persolal_Account_Window persolal_Account_Window = new Persolal_Account_Window();
            persolal_Account_Window.ShowDialog();
        }

        private void Button_maids_Click(object sender, RoutedEventArgs e)
        {
            Maid_Contlol_Window maid_Contlol = new Maid_Contlol_Window();
            maid_Contlol.ShowDialog();
        }

        private void Button_price_list_Click(object sender, RoutedEventArgs e)
        {
            Price_List_Window price_List_Window = new Price_List_Window();
            price_List_Window.ShowDialog();
        }

        private void Button_servise_Click(object sender, RoutedEventArgs e)
        {
            servisWindow servisWindow = new servisWindow();
            servisWindow.ShowDialog();
        }

        private void MenuItem_room_types_Click(object sender, RoutedEventArgs e)
        {
            Type_Room_Window type_Room_Window = new Type_Room_Window();
            type_Room_Window.ShowDialog();
        }

        private void MenuItem_type_of_food_Click(object sender, RoutedEventArgs e)
        {
            type_of_food_Window type_Of_Food_Window = new type_of_food_Window();
            type_Of_Food_Window.ShowDialog();
        }

        private void MenuItem_servises_room_Click(object sender, RoutedEventArgs e)
        {
            Servises_of_room_Window servises_Of_Room_Window = new Servises_of_room_Window();
            servises_Of_Room_Window.ShowDialog();
        }

        private void MenuItem_maid_Click(object sender, RoutedEventArgs e)
        {
            Maid_Window maid_Window = new Maid_Window();
            maid_Window.ShowDialog();
        }

        private void MenuItem_servise_Click(object sender, RoutedEventArgs e)
        {
            Services_Window servises_Window = new Services_Window();
            servises_Window.ShowDialog();
        }

        private void MenuItem_Rooms_Click(object sender, RoutedEventArgs e)
        {
            Rooms_Window rooms_Window = new Rooms_Window();
            rooms_Window.ShowDialog();
        }

        private void MenuItem_pass_Click(object sender, RoutedEventArgs e)
        {
            edit_pass_Window edit_Pass_Window = new edit_pass_Window();
            edit_Pass_Window.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=hotel.db");
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT root FROM users where current = 1", conn);
            string count = Convert.ToString(cmd.ExecuteScalar());

            if (count == "Администратор")
            {
                Control_btn.Visibility = Visibility.Visible;
                MenuItem_stat.IsEnabled = true;
            }
            conn.Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void acc_exit(object sender, RoutedEventArgs e)
        {


            AuthWindow authWindow = new AuthWindow();
            authWindow.Show();
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
                using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("UPDATE users SET current = 0 WHERE current = 1", connection);
                    command.ExecuteNonQuery();
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void Control_btn_Click(object sender, RoutedEventArgs e)
        {
            account_control_Window account_Control_Window = new account_control_Window();
            account_Control_Window.ShowDialog();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
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
                worksheet.Name = "Услуги";


                worksheet.Cells[1, 1] = DateTime.Now.ToString("dd.MM.yyyy");
                worksheet.Cells[1, 1].Font.Bold = true;

                worksheet.Range["A3:D3"].Merge();
                worksheet.Cells[3, 1].Font.Bold = true;
                worksheet.Cells[3, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                worksheet.Cells[3, 1] = "Должники";

                worksheet.Cells[5, 1] = "№";
                worksheet.Cells[5, 2] = "ФИО клиента";
                worksheet.Cells[5, 3] = "Статус";
                worksheet.Cells[5, 4] = "Задолженность, руб.";
                worksheet.Range["A3:D3"].Font.Bold = true;

                worksheet.Range["A6:D30"].EntireColumn.AutoFit();
                worksheet.Columns[2].ColumnWidth = 60;
                worksheet.Columns[3].ColumnWidth = 20;
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

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
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
                // Создаем Excel приложение
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();
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
                worksheet.Range["A:Z"].Font.Name = "Times New Roman";

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
            catch (Exception ex) { MessageBox.Show("Ошибка сохранения.\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}
