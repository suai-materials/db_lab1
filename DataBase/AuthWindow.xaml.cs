using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace DataBase;

public partial class AuthWindow : Window
{
    private Modes mode = Modes.Auth;

    private Dictionary<Modes, String> modeToQuery = new Dictionary<Modes, string>()
    {
        {Modes.Auth, "SELECT user_id, role FROM users WHERE login = @login"}
    };

    enum Modes
    {
        Auth,
        Reg
    }

    public AuthWindow()
    {
        InitializeComponent();
    }

    private void Auth(object sender, RoutedEventArgs e)
    {
        try
        {
            OleDbConnection connection =
                new OleDbConnection(
                    @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\Desktop\labs\04.01\db_lab1\DataBase\db\college.mdb;Persist Security Info=False");

            connection.Open();
            using (SHA256 mySha256 = SHA256.Create())
            {
                var passwordHash = String.Join("",
                    mySha256.ComputeHash(new UTF8Encoding().GetBytes(Password.Password)).Select(b => $"{b:X}")
                        .ToArray()).ToLowerInvariant();
                OleDbCommand command = new OleDbCommand(modeToQuery[mode], connection);
                command.Parameters.Add("@login", OleDbType.VarChar, 80).Value = Login.Text;
                command.Parameters.Add("@pass", OleDbType.VarChar, 80).Value = passwordHash;
                var user = command.ExecuteScalar();
                if (user is null) ;
                else
                {
                    var window = new MainWindow();
                }
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show("База данных не найдена или что-то пошло не так.");
            Close();
        }
    }
}