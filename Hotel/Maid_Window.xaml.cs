﻿using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    /// <summary>
    /// Логика взаимодействия для Maid_Window.xaml
    /// </summary>
    public partial class Maid_Window : Window
    {
        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=.\\hotel.db");
        string query = "SELECT name AS \"ФИО\", position AS \"Должность\", employe_mode AS \"Режим работы\" FROM  maids";
        string name = null;

        public Maid_Window()
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
            dataGrid.ItemsSource = dataTable.DefaultView;
            ClearTxt();
            sqlConnection.Close();
        }

        public void ClearTxt()
        {
            txt_name.Text = "";
            txt_position.Text = "";
            txt_employe_mode.Text = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWin mainWin = new MainWin();
            mainWin.Show();
            Close();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_name.Text.Length == 0 || txt_position.Text.Length == 0 || txt_employe_mode.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "INSERT INTO maids (name, position, employe_mode) VALUES (\"" + txt_name.Text + "\",  \"" + txt_position.Text + "\",  \"" + txt_employe_mode.Text + "\")";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                    command.ExecuteNonQuery();

                    sqlConnection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                refresh_table();
            }
        }

        private void delButton_Click(object sender, RoutedEventArgs e)
        {

            if (name == null)
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (MessageBox.Show($"Удалить запись {name}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                        {
                            connection.Open();
                            SQLiteCommand command = new SQLiteCommand($"DELETE FROM maids WHERE name = \"{name}\"", connection);
                            command.ExecuteNonQuery();
                            refresh_table();
                            MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        name = null;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

                }
            }

        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_name.Text.Length == 0 || txt_position.Text.Length == 0 || txt_employe_mode.Text.Length == 0)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (name == null)
                    MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    if (MessageBox.Show($"Редактировать запись {name}?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(sqlConnection))
                            {
                                connection.Open();
                                SQLiteCommand command = new SQLiteCommand("UPDATE maids SET name = \"" + txt_name.Text + "\", position = \"" + txt_position.Text + "\", employe_mode = \"" + txt_employe_mode.Text + "\" WHERE name = @name", connection);
                                command.Parameters.AddWithValue("@name", name);
                                command.ExecuteNonQuery();

                                MessageBox.Show("Запись отредактирована", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                refresh_table();
                            }
                            name = null;
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
                txt_name.Text = Convert.ToString(selectedRow["ФИО"]);
                txt_position.Text = Convert.ToString(selectedRow["Должность"]);
                txt_employe_mode.Text = Convert.ToString(selectedRow["Режим работы"]);

                name = Convert.ToString(selectedRow["ФИО"]);
            }
        }
    }
}
