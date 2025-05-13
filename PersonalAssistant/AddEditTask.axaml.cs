using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Models;
using PersonalAssistant.Utils.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalAssistant;

public partial class AddEditTask : Window
{
    private int userID;
    private int? taskID = null;
    public AddEditTask()
    {
        InitializeComponent();
    }
    public AddEditTask(int userID)
    {
        InitializeComponent();
        this.userID = userID;
        ListOfTasksBox.ItemsSource = Utils.DbContext.Lists
            .Where(t => t.Users.Any(u => u.Id == userID))
            .ToList();
    }
    public AddEditTask(int userID, int taskID)
    {
        InitializeComponent();
        DeleteButton.IsVisible = true;
        this.userID = userID;
        this.taskID = taskID;
        ListOfTasksBox.ItemsSource = Utils.DbContext.Lists
            .Where(t => t.Users.Any(u => u.Id == userID))
            .ToList();

        using (var context = new User8Context())
        {
            var task = context.Tasks
                .Include(t => t.Lists)
                .FirstOrDefault(t => t.Id == taskID);

            NameBox.Text = task.Name;
            DescriptionBox.Text = task.Description;
            if (task.Priority != null)
            {
                PriorityBox.SelectedIndex = (int)(task.Priority - 1);
            }
            if (task.PriorityTable != null)
            {
                PriorityTableBox.SelectedIndex = (int)(task.PriorityTable - 1);
            }
            StatusBox.SelectedIndex = task.Status - 1;

            // Устанавливаем выбранный элемент в ListOfTasksBox  
            var taskList = task.Lists.FirstOrDefault();
            if (taskList != null)
            {
                ListOfTasksBox.SelectedItem = ListOfTasksBox.ItemsSource
                    .Cast<Models.List>()
                    .FirstOrDefault(l => l.Id == taskList.Id);
            }
        }
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (taskID == null)
        {
            using (var context = new User8Context())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userID);

                var selectedList = (Models.List)ListOfTasksBox.SelectedItem;
                var list = context.Lists.FirstOrDefault(l => l.Id == selectedList.Id);

                Models.Task newTask = new Models.Task()
                {
                    Name = NameBox.Text,
                    Description = DescriptionBox.Text,
                    Priority = PriorityBox.SelectedIndex + 1,
                    PriorityTable = PriorityTableBox.SelectedIndex + 1,
                    Status = StatusBox.SelectedIndex + 1,
                    CreationDate = DateTime.Now,
                    Users = new List<User> { user },
                    Lists = new List<Models.List> { list }
                };
                context.Add(newTask);
                context.SaveChanges();
            }
        }
        else
        {
            using (var context = new User8Context())
            {
                // Загружаем задачу с пользователями и списками  
                var task = context.Tasks
                    .Include(t => t.Users)
                    .Include(t => t.Lists)
                    .FirstOrDefault(t => t.Id == taskID);

                // Обновляем поля задачи  
                task.Name = NameBox.Text;
                task.Description = DescriptionBox.Text;
                task.Priority = PriorityBox.SelectedIndex + 1;
                task.PriorityTable = PriorityTableBox.SelectedIndex + 1;
                task.Status = StatusBox.SelectedIndex + 1;

                // Обновляем список, к которому относится задача
                var selectedList = (Models.List)ListOfTasksBox.SelectedItem;
                if (selectedList != null )
                {
                    var newList = context.Lists.FirstOrDefault(l => l.Id == selectedList.Id);
                    task.Lists.Clear();
                    task.Lists.Add(newList);
                }
                
                context.SaveChanges();
            }
        }

        Utils.DbContext.User8Context = new();
        Utils.DbContext.Tasks = [.. Utils.DbContext.User8Context.Tasks
           .Include(task => task.StatusNavigation)
           .Include(task => task.PriorityNavigation)
           .Include(task => task.Users)];

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

    private void DeleteButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using (var context = new User8Context())
        {
            var task = context.Tasks
                .Include(t => t.Users)
                .FirstOrDefault(t => t.Id == taskID);

            task.Users.Clear();
            context.SaveChanges();

            context.Tasks.Remove(task);
            context.SaveChanges();
        }

        Utils.DbContext.User8Context = new();
        Utils.DbContext.Tasks = [.. Utils.DbContext.User8Context.Tasks
            .Include(task => task.StatusNavigation)
            .Include(task => task.PriorityNavigation)
            .Include(task => task.Users)];

        TasksWindow tasksWindow = new TasksWindow(userID);
        tasksWindow.Show();
        Close();
    }
}