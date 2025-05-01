using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PersonalAssistant.Models;
using PersonalAssistant.Utils.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalAssistant;

public partial class AddEditTask : Window
{
    int userID;
    public AddEditTask()
    {
        InitializeComponent();
    }
    public AddEditTask(int userID)
    {
        InitializeComponent();
        this.userID = userID;
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        

        //newTask.Users.Add(user);

        using (var context = new User8Context())
        {
            var user = context.Users.FirstOrDefault(u => u.Id == userID);
            Models.Task newTask = new Models.Task()
            {
                Name = NameBox.Text,
                Description = DescriptionBox.Text,
                Priority = PriorityBox.SelectedIndex + 1,
                Status = StatusBox.SelectedIndex + 1,
                CreationDate = DateTime.Now,
                Users = new List<User> { user }
            };
            context.Add(newTask);
            context.SaveChanges();
        }

        Utils.DbContext.Tasks = [.. Utils.DbContext.User8Context.Tasks];

        TasksWindow tasksWindow = new TasksWindow(userID);
        tasksWindow.Show();
        Close();
    }

    private void GoBackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TasksWindow tasksWindow = new TasksWindow(userID);
        tasksWindow.Show();
        Close();
    }
}