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

        // ���������� ������ �����
        var lists = DBContext.Lists
            .Where(l => l.Users.Any(u => u.Id == this.userID))
            .ToList();

        // ��������� ������� "���" � ������ ������
        lists.Insert(0, new List { Id = 0, Name = "���" });

        ListOfTasksBox.ItemsSource = lists;
        ListOfTasksBox.SelectedIndex = 0;
    }
    public AddEditTask(int userID, int taskID)
    {
        InitializeComponent();
        DeleteButton.IsVisible = true;
        this.userID = userID;
        this.taskID = taskID;

        // ���������� ������ �����
        var lists = DBContext.Lists
            .Where(l => l.Users.Any(u => u.Id == this.userID))
            .ToList();

        // ��������� ������� "���" � ������ ������
        lists.Insert(0, new List { Id = 0, Name = "���" });

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
            if (task.EisenhowerMatrix != null)
            {
                PriorityTableBox.SelectedIndex = (int)(task.EisenhowerMatrix - 1);
            }
            StatusBox.SelectedIndex = task.Status - 1;

            // ������������� ��������� ������� � ListOfTasksBox  
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
        // �������� �������� �� DatePicker � TimePicker
        DateTime? startDateTime = null, endDateTime = null, deadlineDateTime = null;

        if (StartDatePicker.SelectedDate.HasValue && StartTimePicker.SelectedTime.HasValue)
            startDateTime = StartDatePicker.SelectedDate.Value.Date + StartTimePicker.SelectedTime.Value;

        if (EndDatePicker.SelectedDate.HasValue && EndTimePicker.SelectedTime.HasValue)
            endDateTime = EndDatePicker.SelectedDate.Value.Date + EndTimePicker.SelectedTime.Value;

        if (DeadlineDatePicker.SelectedDate.HasValue && DeadlineTimePicker.SelectedTime.HasValue)
            deadlineDateTime = DeadlineDatePicker.SelectedDate.Value.Date + DeadlineTimePicker.SelectedTime.Value;

        // ��������� ���
        if (startDateTime.HasValue && endDateTime.HasValue && startDateTime > endDateTime)
        {
            //await MessageBox.Show(this, "���� ������ �� ����� ���� ����� ���� ���������.", "������", MessageBox.MessageBoxButtons.Ok);
            return;
        }
        if (deadlineDateTime.HasValue)
        {
            if (startDateTime.HasValue && deadlineDateTime < startDateTime)
            {
                //await MessageBox.Show(this, "������� �� ����� ���� ������ ���� ������.", "������", MessageBox.MessageBoxButtons.Ok);
                return;
            }
            if (endDateTime.HasValue && deadlineDateTime < endDateTime)
            {
                //await MessageBox.Show(this, "������� �� ����� ���� ������ ���� ���������.", "������", MessageBox.MessageBoxButtons.Ok);
                return;
            }
        }


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
                    NameBox.Text = "������";
                }

                Models.Task newTask;
                if (ListOfTasksBox.SelectedIndex == 0)
                {
                    newTask = new Models.Task()
                    {
                        Name = NameBox.Text,
                        Description = DescriptionBox.Text,
                        Priority = PriorityBox.SelectedIndex + 1,
                        EisenhowerMatrix = PriorityTableBox.SelectedIndex + 1,
                        Status = StatusBox.SelectedIndex + 1,
                        CreationDate = DateTime.Now,
                        Users = new List<User> { user },
                        StartDate = startDateTime,
                        EndDate = endDateTime,
                        Deadline = deadlineDateTime
                    };
                }
                else
                {
                    newTask = new Models.Task()
                    {
                        Name = NameBox.Text,
                        Description = DescriptionBox.Text,
                        Priority = PriorityBox.SelectedIndex + 1,
                        EisenhowerMatrix = PriorityTableBox.SelectedIndex + 1,
                        Status = StatusBox.SelectedIndex + 1,
                        CreationDate = DateTime.Now,
                        Users = new List<User> { user },
                        Lists = new List<Models.List> { list },
                        StartDate = startDateTime,
                        EndDate = endDateTime,
                        Deadline = deadlineDateTime
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
                // ��������� ������ � �������������� � ��������  
                var task = context.Tasks
                    .Include(t => t.Users)
                    .Include(t => t.Lists)
                    .FirstOrDefault(t => t.Id == taskID);

                if (string.IsNullOrEmpty(NameBox.Text))
                {
                    NameBox.Text = "������";
                }

                // ��������� ���� ������  
                task.Name = NameBox.Text;
                task.Description = DescriptionBox.Text;
                task.Priority = PriorityBox.SelectedIndex + 1;
                task.EisenhowerMatrix = PriorityTableBox.SelectedIndex + 1;
                task.Status = StatusBox.SelectedIndex + 1;
                task.StartDate = startDateTime;
                task.EndDate = endDateTime;
                task.Deadline = deadlineDateTime;

                if (ListOfTasksBox.SelectedIndex == 0)
                {
                    task.Lists.Clear();
                }
                else
                {
                    // ��������� ������, � �������� ��������� ������
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

        // ��� �������� ���� ���� �� ����, ��� � ����������, ����������� ���������� ��������� ��� ��� ����.
        // ���� � ��� ���� ������ ��� ���� (��������, DBContext.PreviousWindowType), �������� ��������� ����� ���������:
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