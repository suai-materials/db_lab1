using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Data.OleDb;
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
        private string queryNow;
        private const string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pankSU\Desktop\db_lab1\DataBase\db\College.mdb;Persist Security Info=False";
        OleDbConnection connection = new OleDbConnection(ConnectionString);
        public MainWindow()
        {
            InitializeComponent();
            reloadTablebyCommand(baseQuery);
        }

        public void reloadTablebyCommand(string? Query = null, string Familia = "%")
        {
            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("База данных не найдена или что-то пошло не так.");
                Close();
                return;
            }
            
            var table = new DataTable();
            var command = new OleDbCommand(Query ?? queryNow, connection);
            command.Parameters.Add( "?", OleDbType.VarChar, 80 ).Value = Familia;
            new OleDbDataAdapter(command).Fill(table);
            data.ItemsSource = table.DefaultView;
            connection.Close();
            queryNow = Query ?? queryNow;
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            reloadTablebyCommand(Familia: Familia.Text);
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            reloadTablebyCommand(QueryGetMalesWithAge);
        }


        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            reloadTablebyCommand(baseQuery);
        }
    }
}