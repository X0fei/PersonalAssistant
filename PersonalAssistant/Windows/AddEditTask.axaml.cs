using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Context;
using PersonalAssistant.Helpers;
using PersonalAssistant.Models;
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

        // Подгружаем списки задач
        var lists = DBContext.Lists
            .Where(l => l.Users.Any(u => u.Id == this.userID))
            .ToList();

        // Добавляем элемент "Нет" в начало списка
        lists.Insert(0, new List { Id = 0, Name = "Нет" });

        ListOfTasksBox.ItemsSource = lists;
        ListOfTasksBox.SelectedIndex = 0;
    }
    public AddEditTask(int userID, int taskID)
    {
        InitializeComponent();
        DeleteButton.IsVisible = true;
        this.userID = userID;
        this.taskID = taskID;

        // Подгружаем списки задач
        var lists = DBContext.Lists
            .Where(l => l.Users.Any(u => u.Id == this.userID))
            .ToList();

        // Добавляем элемент "Нет" в начало списка
        lists.Insert(0, new List { Id = 0, Name = "Нет" });

        ListOfTasksBox.ItemsSource = lists;

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

                List selectedList = ListOfTasksBox.SelectedItem as List;
                List? list = null;
                if (selectedList != null)
                {
                    list = context.Lists.FirstOrDefault(l => l.Id == selectedList.Id);
                    // Proceed with logic
                }
                else
                {
                    // Handle the case where no list is selected
                }

                if (string.IsNullOrEmpty(NameBox.Text))
                {
                    NameBox.Text = "Задача";
                }

                Models.Task newTask;
                if (ListOfTasksBox.SelectedIndex == 0)
                {
                    newTask = new Models.Task()
                    {
                        Name = NameBox.Text,
                        Description = DescriptionBox.Text,
                        Priority = PriorityBox.SelectedIndex + 1,
                        PriorityTable = PriorityTableBox.SelectedIndex + 1,
                        Status = StatusBox.SelectedIndex + 1,
                        CreationDate = DateTime.Now,
                        Users = new List<User> { user }
                    };
                }
                else
                {
                    newTask = new Models.Task()
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
                }

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

                if (string.IsNullOrEmpty(NameBox.Text))
                {
                    NameBox.Text = "Задача";
                }

                // Обновляем поля задачи  
                task.Name = NameBox.Text;
                task.Description = DescriptionBox.Text;
                task.Priority = PriorityBox.SelectedIndex + 1;
                task.PriorityTable = PriorityTableBox.SelectedIndex + 1;
                task.Status = StatusBox.SelectedIndex + 1;

                if (ListOfTasksBox.SelectedIndex == 0)
                {
                    task.Lists.Clear();
                }
                else
                {
                    // Обновляем список, к которому относится задача
                    var selectedList = (Models.List)ListOfTasksBox.SelectedItem;
                    if (selectedList != null)
                    {
                        var newList = context.Lists.FirstOrDefault(l => l.Id == selectedList.Id);
                        task.Lists.Clear();
                        task.Lists.Add(newList);
                    }
                }
                
                context.SaveChanges();
            }
        }

        DBContext.User8Context = new();
        DBContext.Tasks = [.. DBContext.User8Context.Tasks
           .Include(task => task.StatusNavigation)
           .Include(task => task.PriorityNavigation)
           .Include(task => task.Users)];

        DBContext.Lists = [.. DBContext.User8Context.Lists
            .Include(list => list.Tasks)
            .Include(task => task.Users)];

        // Для создания окна того же типа, что и предыдущее, используйте сохранённый экземпляр или тип окна.
        // Если у вас есть только тип окна (например, DBContext.PreviousWindowType), создайте экземпляр через рефлексию:
        if (DBContext.PreviousWindowType != null)
        {
            var previousWindow = (Window)Activator.CreateInstance(DBContext.PreviousWindowType, userID);
            previousWindow.Show();
            Close();
        }
    }

    private void GoBackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DBContext.PreviousWindowType != null)
        {
            var previousWindow = (Window)Activator.CreateInstance(DBContext.PreviousWindowType, userID);
            previousWindow.Show();
            Close();
        }
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

        DBContext.User8Context = new();
        DBContext.Tasks = [.. DBContext.User8Context.Tasks
            .Include(task => task.StatusNavigation)
            .Include(task => task.PriorityNavigation)
            .Include(task => task.Users)];

        if (DBContext.PreviousWindowType != null)
        {
            var previousWindow = (Window)Activator.CreateInstance(DBContext.PreviousWindowType, userID);
            previousWindow.Show();
            Close();
        }
    }
}