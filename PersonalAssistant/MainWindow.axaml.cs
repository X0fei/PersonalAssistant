using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Models;
using PersonalAssistant.Utils.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PersonalAssistant
{
    public partial class MainWindow : Window
    {
        private List<User> users = Utils.DbContext.Users.ToList();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.IsVisible = true;
        }

        private void HideError()
        {
            ErrorTextBlock.Text = string.Empty;
            ErrorTextBlock.IsVisible = false;
        }

        private void Input_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            HideError();
        }

        private void LoginButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string? login = LoginBox.Text?.Trim();
            string? password = LoginPasswordBox.Text;

            var user = users.FirstOrDefault(u => u.Login == login);

            if (user == null)
            {
                ShowError("Пользователь с таким логином не найден.");
            }
            else if (user.Password != password)
            {
                ShowError("Неверный логин или пароль.");
            }
            else
            {
                TasksWindow tasksWindow = new TasksWindow(user.Id);
                tasksWindow.Show();
                Close();
            }
        }

        private void RegisterLoginBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            string? login = RegisterLoginBox.Text?.Trim();
            Regex loginRegex = new Regex(@"^[a-zA-Z0-9_-]+$");

            if (string.IsNullOrWhiteSpace(login))
            {
                LoginErrorText.Text = "Логин не может быть пустым";
                LoginErrorText.IsVisible = true;
            }
            else if (login.Length < 3 || login.Length > 20)
            {
                LoginErrorText.Text = "Логин должен быть от 3 до 20 символов";
                LoginErrorText.IsVisible = true;
            }
            else if (!loginRegex.IsMatch(login))
            {
                LoginErrorText.Text = "Допустимы только латинские буквы, цифры, - и _";
                LoginErrorText.IsVisible = true;
            }
            else if (users.Any(u => u.Login == login))
            {
                LoginErrorText.Text = "Логин уже занят";
                LoginErrorText.IsVisible = true;
            }
            else
            {
                LoginErrorText.IsVisible = false;
            }
        }


        private void RegisterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string? login = RegisterLoginBox.Text?.Trim();
            string? name = RegisterUsernameBox.Text?.Trim();
            string? password = RegisterPasswordBox.Text;
            string? confirmPassword = RegisterConfirmPasswordBox.Text;

            if (LoginErrorText.IsVisible)
                return;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                return;

            if (password != confirmPassword)
            {
                PasswordErrorText.Text = "Пароли не совпадают";
                PasswordErrorText.IsVisible = true;
                return;
            }
            else
            {
                PasswordErrorText.IsVisible = false;
            }

            User newUser = new User()
            {
                Login = login,
                Name = name,
                Password = password
            };

            using (var context = new User8Context())
            {
                context.Users.Add(newUser);
                context.SaveChanges();
            }

            Utils.DbContext.Users = [.. Utils.DbContext.User8Context.Users];
            users = Utils.DbContext.Users.ToList();

            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                TasksWindow tasksWindow = new TasksWindow(user.Id);
                tasksWindow.Show();
                Close();
            }
        }
    }
}