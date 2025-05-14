using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Context;
using PersonalAssistant.Models;
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
                ShowError("������������ � ����� ������� �� ������.");
            }
            else if (user.Password != password)
            {
                ShowError("�������� ����� ��� ������.");
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
                LoginErrorText.Text = "����� �� ����� ���� ������";
                LoginErrorText.IsVisible = true;
            }
            else if (login.Length < 3)
            {
                LoginErrorText.Text = "����� ������ ���� �� 3 �� 20 ��������";
                LoginErrorText.IsVisible = true;
            }
            else if (!loginRegex.IsMatch(login))
            {
                LoginErrorText.Text = "��������� ������ ��������� �����, �����, - � _";
                LoginErrorText.IsVisible = true;
            }
            else if (users.Any(u => u.Login == login))
            {
                LoginErrorText.Text = "����� ��� �����";
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
            string? password = RegisterPasswordBox.Text;
            string? confirmPassword = RegisterConfirmPasswordBox.Text;

            if (LoginErrorText.IsVisible)
                return;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                return;

            if (password != confirmPassword)
            {
                PasswordErrorText.Text = "������ �� ���������";
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
                Name = login,
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

        private void EnterToTab(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox)
            {
                var parent = textBox.Parent;
                if (parent is Panel panel)
                {
                    // �������� ��� �������� ���������� � ������������ ����������
                    var textBoxes = panel.Children.OfType<TextBox>().ToList();
                    int currentIndex = textBoxes.IndexOf(textBox);

                    // ���� ������� ������� �� ���������, ��������� � ����������
                    if (currentIndex >= 0 && currentIndex < textBoxes.Count - 1)
                    {
                        var nextControl = textBoxes[currentIndex + 1];
                        nextControl.Focus();
                    }
                    else
                    {
                        // ���� ���������, �������� ������
                        var controls = panel.Children.OfType<Control>().ToList();
                        var button = controls.OfType<Button>().FirstOrDefault();
                        button?.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
                    }
                }
            }
        }
    }
}