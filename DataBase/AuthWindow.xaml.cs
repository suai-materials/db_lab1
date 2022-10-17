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
    private Mode _mode = Mode.Auth;

    private Dictionary<Mode, string> modeToQuery = new()
    {
        {Mode.Auth, "SELECT user_id, role FROM users WHERE login = @login AND password = @pass"},
        {Mode.Reg, "INSERT INTO users ([login], [password]) VALUES (@login, @pass)"}
    };


    public AuthWindow()
    {
        InitializeComponent();
    }

    private void Auth(object sender, RoutedEventArgs e)
    {
        try
        {
            var connection =
                new OleDbConnection(
                    @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\Desktop\labs\04.01\db_lab1\DataBase\db\college.mdb;Persist Security Info=False");

            connection.Open();
            using (var mySha256 = SHA256.Create())
            {
                if (Login.Text.Trim() == "")
                {
                    ErrorMessage.Text = "Логин не может быть пустым";
                    return;
                }

                if (Password.Password.Trim() == "")
                {
                    ErrorMessage.Text = "Пароль не должен быть пустым";
                    return;
                }

                if (_mode == Mode.Reg)
                {
                    if (Password.Password != SecondPassword.Password)
                    {
                        ErrorMessage.Text = "Пароли не совпадают";
                        return;
                    }

                    var checkThisLoginCommand =
                        new OleDbCommand("SELECT user_id FROM users WHERE login = @login", connection);
                    checkThisLoginCommand.Parameters.Add("@login", OleDbType.VarChar, 80).Value = Login.Text;
                    if (checkThisLoginCommand.ExecuteScalar() != null)
                    {
                        ErrorMessage.Text = "Такой логин уже существует";
                        return;
                    }
                }

                var passwordHash = string.Join("",
                    mySha256.ComputeHash(new UTF8Encoding().GetBytes(Password.Password)).Select(b => $"{b:X}")
                        .ToArray()).ToLowerInvariant();
                var command = new OleDbCommand(modeToQuery[_mode], connection);
                command.Parameters.Add("@login", OleDbType.VarChar, 80).Value = Login.Text;
                command.Parameters.Add("@pass", OleDbType.VarChar, 80).Value = passwordHash;
                if (_mode == Mode.Reg)
                {
                    command.ExecuteNonQuery();
                    ToReg(null, null);
                    return;
                }

                var user = command.ExecuteReader();
                if (!user.HasRows)
                {
                    ErrorMessage.Text = "Неверный логин или пароль";
                }
                else
                {
                    user.Read();
                    var userId = user.GetInt32(0);
                    var roleId = user.GetInt32(1);

                    var window = new MainWindow(userId, roleId);
                    window.Show();
                    Close();
                }
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show("База данных не найдена или что-то пошло не так.");
            Close();
        }
    }

    private void ToReg(object? sender, RoutedEventArgs? e)
    {
        ErrorMessage.Text = "";
        switch (_mode)
        {
            case Mode.Auth:
                _mode = Mode.Reg;
                SecPasswordPanel.Visibility = Visibility.Visible;
                SwitchBtn.Content = "Перейти к авторизации";
                AuthBtn.Content = "Зарегестрироваться";
                break;
            case Mode.Reg:
                _mode = Mode.Auth;
                SwitchBtn.Content = "Перейти к регистрации";
                AuthBtn.Content = "Вход";
                SecPasswordPanel.Visibility = Visibility.Collapsed;
                break;
        }
    }
}