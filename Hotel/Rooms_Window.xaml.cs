using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Rooms_Window.xaml
    /// </summary>
    public partial class Rooms_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT num AS \"№\", room_types.type AS \"Тип\", floor AS \"Этаж\", doplata AS \"Доплата\", telephone AS \"Телефон\",  rooms.description AS \"Описание\",jpg FROM rooms, room_types where room_types.id = rooms.type";
        string num = null;
        public Rooms_Window()
        {
            InitializeComponent();

            refresh_table();

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
            DataTable dataTable = new DataTable();
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
                                    "VALUES (\"" + txt_num.Text + "\", \"" + txt_floor.Text + "\", (select room_types.id from room_types where room_types.type like \"%" + comboBox_type.Text + "%\"), \"" + txt_doplata.Text + "\", \"" + txt_telephone.Text + "\", \"" + txt_description.Text + "\", @jpg)";


                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                    command.Parameters.AddWithValue("@jpg", imageBytes);
                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                    refresh_table();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

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
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM rooms WHERE num = \"{num}\"", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                            MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        num = null;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }


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
                                    // Получаем изображение из элемента управления Image
                                    Bitmap bmp = new Bitmap(ImageControl.Source.ToString());

                                    // Создаем MemoryStream
                                    MemoryStream ms = new MemoryStream();

                                    // Сохраняем изображение в MemoryStream в формате PNG
                                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                                    // Получаем массив байтов из MemoryStream
                                    imageBytes = ms.ToArray();

                                    // Закрываем MemoryStream
                                    ms.Close();
                                }
                                SQLiteCommand command = new SQLiteCommand("UPDATE rooms SET num =\"" + txt_num.Text + "\", floor = \"" + txt_floor.Text + "\", type = (select room_types.id from room_types where room_types.type like \"%" + comboBox_type.Text + "%\"), doplata = \"" + txt_doplata.Text + "\", telephone = \"" + txt_telephone.Text + "\", description = \"" + txt_description.Text + "\", jpg = @jpg WHERE num = @num", connection);
                                command.Parameters.AddWithValue("@num", num);
                                command.Parameters.AddWithValue("@jpg", imageBytes);
                                command.ExecuteNonQuery();
                                MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                refresh_table();
                            }
                            num = null;
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

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
                    ImageControl.Source = null;
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
    }
}
