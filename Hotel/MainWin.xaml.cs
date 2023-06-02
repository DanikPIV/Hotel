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
            Reservation_Window reservation_Window = new Reservation_Window();
            reservation_Window.Show();
            Close();
        }

        private void MenuItem_Clients_Click(object sender, RoutedEventArgs e)
        {
            ClientsWindow clientsWindow = new ClientsWindow();
            clientsWindow.Show();
            Close();
        }
        private void MenuItem_Status_Clients_Click(object sender, RoutedEventArgs e)
        {
            Status_Clients_Window sclientsWindow = new Status_Clients_Window();
            sclientsWindow.Show();
            Close();
        }

        private void Button_chess_Click(object sender, RoutedEventArgs e)
        {
            Chess_Window chess_Window = new Chess_Window();
            chess_Window.ShowDialog();
        }

        private void Button_personal_account_Click(object sender, RoutedEventArgs e)
        {
            Persolal_Account_Window persolal_Account_Window = new Persolal_Account_Window();
            persolal_Account_Window.Show();
            Close();
        }

        private void Button_maids_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_price_list_Click(object sender, RoutedEventArgs e)
        {
            Price_List_Window price_List_Window = new Price_List_Window();
            price_List_Window.Show();
            Close();
        }

        private void Button_servise_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_room_types_Click(object sender, RoutedEventArgs e)
        {
            Type_Room_Window type_Room_Window = new Type_Room_Window();
            type_Room_Window.Show();
            Close();
        }

        private void MenuItem_type_of_food_Click(object sender, RoutedEventArgs e)
        {
            type_of_food_Window type_Of_Food_Window = new type_of_food_Window();
            type_Of_Food_Window.Show();
            Close();
        }

        private void MenuItem_servises_room_Click(object sender, RoutedEventArgs e)
        {
            Servises_of_room_Window servises_Of_Room_Window = new Servises_of_room_Window();
            servises_Of_Room_Window.Show();
            Close();
        }

        private void MenuItem_maid_Click(object sender, RoutedEventArgs e)
        {
            Maid_Window maid_Window = new Maid_Window();
            maid_Window.Show();
            Close();
        }

        private void MenuItem_servise_Click(object sender, RoutedEventArgs e)
        {
            Services_Window servises_Window = new Services_Window();
            servises_Window.Show();
            Close();
        }

        private void MenuItem_Rooms_Click(object sender, RoutedEventArgs e)
        {
            Rooms_Window rooms_Window = new Rooms_Window();
            rooms_Window.Show();
            Close();
        }
    }
}
