using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Models;
using PersonalAssistant.Utils.Context;
using System.Collections.Generic;
using System.Linq;

namespace PersonalAssistant
{
    public partial class MainWindow : Window
    {
        private List<User> users = Utils.DbContext.users.ToList();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = LoginPasswordBox.Text;

            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                TasksWindow tasksWindow = new TasksWindow(user.Id);
                tasksWindow.Show();
                Close();
            }
            else
            {
                
            }
        }

        private void RegisterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string login = RegisterLoginBox.Text;
            string name = RegisterUsernameBox.Text;
            string password = RegisterPasswordBox.Text;
            string confirmPassword = RegisterConfirmPasswordBox.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            if (password != confirmPassword)
            {
                return;
            }

            if (users.Any(u => u.Login == login))
            {
                return;
            }

            User newUser = new User()
            {
                Login = login,
                Name = name,
                Password = password
            };

            using (var context = new User8Context())
            {
                context.Add(newUser);
                context.SaveChanges();
            }

            Utils.DbContext.users = [.. Utils.DbContext.User8Context.Users];
            users = Utils.DbContext.users.ToList();

            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                TasksWindow tasksWindow = new TasksWindow(user.Id);
                tasksWindow.Show();
                Close();
            }
            else
            {

            }
        }
    }
}