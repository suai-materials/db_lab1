﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using Microsoft.VisualBasic;

namespace DataBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string QueryGetMalesWithAge =
            "SELECT *, DateDiff(\"yyyy\", Datarogd, Date()) AS [Возраст]  FROM Students WHERE Students.Pol = 'М' AND Familia LIKE ?";
        private string baseQuery = "SELECT * FROM Students WHERE Familia LIKE ?";
        private string GroupQuery =
            "SELECT Familia + \" \" + Left([Imya], 1) + \". \" + Left([Otchestvo], 1) + \".\" AS [Фамилия и инициалы] FROM Students WHERE Familia LIKE ? AND №gr =  @group";

        private string YearQuery = "SELECT Familia, [№gr]  FROM Students WHERE Year([Datarogd]) = @year AND Familia LIKE ?";
        private string OutOfTownQuery = "SELECT Familia, [№gr] FROM Students WHERE Gorod <> \"\" AND Familia LIKE ?";
        private string queryNow;
        private const string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\Desktop\labs\04.01\db_lab1\DataBase\db\College.mdb;Persist Security Info=False";
        private int year;
        OleDbConnection connection = new OleDbConnection(ConnectionString);
        public MainWindow()
        {
            InitializeComponent();
            
            ReloadTablebyCommand(Query: baseQuery);
        }

        private void OpenConnection()
        {
            try
            {
                connection.Open();
            }
            // catch (OleDbException)
            // {
            //     // Студент уже создан
            // }
            catch (Exception e)
            {
                MessageBox.Show("База данных не найдена или что-то пошло не так.");
                Close();
            }
        }

        public void ReloadTablebyCommand(string? Query = null, string Familia = "%")
        {
            OpenConnection();
            queryNow = Query ?? queryNow;
            var command = new OleDbCommand(Query ?? queryNow, connection);
            command.Parameters.Add( "?", OleDbType.VarChar, 80 ).Value = Familia;
            ReloadTable(command);
        }
        
        public void ReloadTablebyCommand(string groupName, string? Query = null, string Familia = "%")
        {
            OpenConnection();
            queryNow = Query ?? queryNow;
            var command = new OleDbCommand(queryNow, connection);
            command.Parameters.Add( "?", OleDbType.VarChar, 80 ).Value = Familia;
            command.Parameters.Add( "@group", OleDbType.VarChar, 6).Value = groupName;
            ReloadTable(command);
        }
        
        public void ReloadTablebyCommand(int year, string? Query = null, string Familia = "%")
        {
            OpenConnection();
            queryNow = Query ?? queryNow;
            var command = new OleDbCommand(queryNow, connection);
            command.Parameters.AddWithValue("@year", year);
            command.Parameters.Add( "?", OleDbType.VarChar, 80 ).Value = Familia;
            
            ReloadTable(command);
        }

        private void ReloadTable(OleDbCommand command)
        {
            var table = new DataTable();
            new OleDbDataAdapter(command).Fill(table);
            data.ItemsSource = table.DefaultView;
            connection.Close();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            if (Group.Visibility is Visibility.Visible)
                ReloadTablebyCommand(groupName: Group.Text, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
            else if (Year.Visibility is Visibility.Visible && int.TryParse(Year.Text, out year))
                ReloadTablebyCommand(year, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
            else 
                ReloadTablebyCommand(Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            ReloadTablebyCommand(Query: QueryGetMalesWithAge, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }


        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ReloadTablebyCommand(Query: baseQuery, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void SearchByGroup(object sender, RoutedEventArgs e)
        {
            ReloadTablebyCommand(Group.Text, GroupQuery, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void GroupSearch_OnChecked(object sender, RoutedEventArgs e)
        {
            GroupText.Visibility = Visibility.Visible;
            Group.Visibility = Visibility.Visible;
            GroupButton.Visibility = Visibility.Visible;
            HideCheckBoxes();
            ReloadTablebyCommand(Group.Text, GroupQuery, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void GroupSearch_OnUnchecked(object sender, RoutedEventArgs e)
        {
            GroupText.Visibility = Visibility.Collapsed;
            Group.Visibility = Visibility.Collapsed;
            GroupButton.Visibility = Visibility.Collapsed;
            ShowCheckBoxes();
            ReloadTablebyCommand(baseQuery, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        void HideCheckBoxes()
        {
            YoungMen.IsChecked = false;
            YoungMen.Visibility = Visibility.Collapsed;
            OutOfTown.IsChecked = false;
            OutOfTown.Visibility = Visibility.Collapsed;
        }
        
        void ShowCheckBoxes()
        {
            YoungMen.Visibility = Visibility.Visible;
            OutOfTown.Visibility = Visibility.Visible;
        }

        private void YearSearch_OnChecked(object sender, RoutedEventArgs e)
        {
            HideCheckBoxes();
            GroupSearch.IsChecked = false;
            GroupSearch.IsEnabled = false;
            YearText.Visibility = Visibility.Visible;
            Year.Visibility = Visibility.Visible;
            YearButton.Visibility = Visibility.Visible;
            ReloadTablebyCommand(0, YearQuery, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void YearButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Year.Text, out year))
                ReloadTablebyCommand(year, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void YearSearch_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ShowCheckBoxes();
            GroupSearch.IsEnabled = true;
            YearText.Visibility = Visibility.Collapsed;
            Year.Visibility = Visibility.Collapsed;
            YearButton.Visibility = Visibility.Collapsed;
            ReloadTablebyCommand(Query: OutOfTownQuery, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void OutOfTown_OnChecked(object sender, RoutedEventArgs e)
        {
            ReloadTablebyCommand(Query: OutOfTownQuery, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void OutOfTown_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ReloadTablebyCommand(Query: baseQuery, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void Reform1(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("База данных не найдена или что-то пошло не так.");
                Close();
            }
            new OleDbCommand("UPDATE Students SET Gorod = \'г. Колпино\', Budget = 1 WHERE POL = \'М\' AND Budget = 0", connection).ExecuteNonQuery();
            connection.Close();
            ReloadTablebyCommand(Query: baseQuery, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void Reform2(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("База данных не найдена или что-то пошло не так.");
                Close();
            }
            new OleDbCommand("DELETE FROM Students WHERE Budget = 0", connection).ExecuteNonQuery();
            connection.Close();
            ReloadTablebyCommand(Query: baseQuery, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }
    }
}