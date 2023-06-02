using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Chess_Window.xaml
    /// </summary>
    public partial class Chess_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT service AS \"Услуга\", price AS \"Цена\", description AS \"Описание\" FROM  services";
        public Chess_Window()
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
            DataGrid.ItemsSource = dataTable.DefaultView;
            sqlConnection.Close();
        }
    }
}