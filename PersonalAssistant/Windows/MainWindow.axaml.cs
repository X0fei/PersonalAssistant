using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Context;
using PersonalAssistant.Models;
using PersonalAssistant.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Interactivity;

namespace PersonalAssistant
{
    public partial class MainWindow : Window
    {
        private List<User> users = [.. DBContext.Users];
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void LoginButton_Click(object? sender, RoutedEventArgs e)
        {
            string? login = LoginBox.Text?.Trim(); // Use null-conditional operator to safely access Text
            if (!InputIsValid(login, "�����"))
            {
                return;
            }

            string? password = LoginPasswordBox.Text;
            if (!InputIsValid(password, "������"))
            {
                return;
            }

            User user;

            try
            {
                user = DBContext.GetUserByLogin(login!); // Use null-forgiving operator since password is validated
            }
            catch (InvalidOperationException exception)
            {
                ShowError(exception.Message);
                return;
            }

            if (user.Password != password)
            {
                ShowError("�������� ����� ��� ������.");
                return;
            }

            LogIn(user);
        }

        private bool InputIsValid(string? input, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                ShowError($"{fieldName} �� ����� ���� ������.");
                return false;
            }
            return true;
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

        private void LogIn(User user)
        {
            new TasksWindow(user.Id).Show();
            Close();
        }

        private void Input_TextChanged(object? sender, TextChangedEventArgs e)
        {
            HideError();

            if (sender is TextBox textBox)
            {
                string fieldName = textBox.Name switch
                {
                    "LoginBox" or "RegisterLoginBox" => "�����",
                    "LoginPasswordBox" or "RegisterPasswordBox" => "������",
                    "RegisterConfirmPasswordBox" => "������������� ������",
                    _ => string.Empty,
                };
                InputIsValid(textBox.Text, fieldName);
            }
        }

        private void RegisterLoginBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            string? login = RegisterLoginBox.Text?.Trim();
            Regex loginRegex = new(@"^[a-zA-Z0-9_-]+$");

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

            User newUser = new()
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

            DBContext.Users = [.. DBContext.User8Context.Users];
            users = [.. DBContext.Users];

            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                TasksWindow tasksWindow = new(user.Id);
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