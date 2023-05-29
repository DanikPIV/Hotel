using System.Windows;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для MainWin.xaml
    /// </summary>
    public partial class MainWin : Window
    {
        public MainWin()
        {
            InitializeComponent();
        }

        private void Button_reservation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Clients_Click(object sender, RoutedEventArgs e)
        {
            ClientsWindow clientsWindow = new ClientsWindow();
            clientsWindow.Show();
            Close();
        }

        private void Button_chess_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_personal_account_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_maids_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_price_list_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_servise_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
