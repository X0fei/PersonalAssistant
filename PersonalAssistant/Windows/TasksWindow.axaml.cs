using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using PersonalAssistant.Helpers;
using PersonalAssistant.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalAssistant;

public partial class TasksWindow : Window
{
    private int userID;
    private User currentUser;
    private List<Models.Task> displayAllTasks;
    public TasksWindow()
    {
        InitializeComponent();
    }

    public TasksWindow(int userID)
    {
        InitializeComponent();

        this.userID = userID;
        currentUser = DBContext.Users.First(u => u.Id == userID);

        LoadData();
    }

    private void GoBackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.CurrentUser = null;
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }

    private void AddTaskButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        AddEditTask addEditTask = new AddEditTask(userID);
        addEditTask.Show();
        Close();
    }

    private void Item_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (AllTasksList.SelectedItem is Models.Task selectedTask)
        {
            DBContext.PreviousWindowType = typeof(TasksWindow);
            AddEditTask addEditTask = new AddEditTask(userID, selectedTask.Id);
            addEditTask.Show();
            Close();
        }
    }

    //private void FilterComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    //{
    //    string selectedPriority = FilterComboBox.SelectedItem?.ToString() ?? "���";

    //    if (selectedPriority == "���")
    //    {
    //        AllTasksList.ItemsSource = displayAllTasks;
    //    }
    //    else
    //    {
    //        AllTasksList.ItemsSource = displayAllTasks
    //            .Where(t => t.EisenhowerMatrixNavigation?.Name == selectedPriority)
    //            .ToList();
    //    }
    //}

    private void ProfileButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        ProfileWindow profileWindow = new ProfileWindow(currentUser);
        profileWindow.Show();
        Close();
    }

    private void AddListOfTasksButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        OpenListOfTasksWindow();
    }

    private async System.Threading.Tasks.Task OpenListOfTasksWindow()
    {
        AddEditListOfTasksWindow addEditListOfTasksWindow = new AddEditListOfTasksWindow(userID);
        await addEditListOfTasksWindow.ShowDialog(this);

        // ���������� ������ �����
        var lists = DBContext.Lists
            .Where(l => l.Users.Any(u => u.Id == currentUser.Id))
            .ToList();

        // ��������� ������� "���" � ������ ������
        lists.Insert(0, new List { Id = 0, Name = "���" });

        ListsOfTasks.ItemsSource = lists;
    }
    private void ListsOfTasks_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        displayAllTasks = DBContext.Tasks
            .Where(t => t.Users.Any(u => u.Id == currentUser.Id))
            .ToList();

        if (ListsOfTasks.SelectedItem is List selectedList && selectedList.Id != 0)
        {
            // ��������� ������, ��������� � ��������� �������
            AllTasksList.ItemsSource = displayAllTasks
                .Where(t => t.Lists.Any(l => l.Id == selectedList.Id))
                .ToList();
        }
        else
        {
            // ���� ������ "���", ���������� ��� ������
            AllTasksList.ItemsSource = displayAllTasks;
        }
    }

    private void LoadProfileImage()
    {
        var pfp = currentUser.MainPfpNavigation;
        if (pfp != null && File.Exists(pfp.Path))
        {
            ProfileImage.Source = new Bitmap(pfp.Path);
        }
        else
        {
            ProfileImage.Source = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Images/PFPs/blank_profile.png");
        }
    }

    private void LoadData()
    {
        LoadProfileImage();

        Statistics.CalculateAll();
        ActiveTasksTextBlock.Text = $"{Statistics.AvticeTasks}";
        CompletedTasksTextBlock.Text = $"{Statistics.CompletedTasks}";
        OverdueTasksTextBlock.Text = $"{Statistics.OverdueTasks}";
        CompletedTasksPercentTextBlock.Text = $"{Statistics.CompletedTasksPercent}";

        GreetingTextBlock.Text = "������, " + currentUser.Name + "!";

        // ���������� ��� ������������
        UserName.Text = currentUser.Name;

        // ���������� ��� ������
        displayAllTasks = DBContext.Tasks
            .Where(t => t.Users.Any(u => u.Id == currentUser.Id))
            .ToList();
        AllTasksList.ItemsSource = displayAllTasks;

        // ���������� ������ �����
        var lists = DBContext.Lists
            .Where(l => l.Users.Any(u => u.Id == currentUser.Id))
            .ToList();

        // ��������� ������� "���" � ������ ������
        lists.Insert(0, new List { Id = 0, Name = "���" });

        ListsOfTasks.ItemsSource = lists;

        // ������������� "���" ��� ��������� �������
        ListsOfTasks.SelectedIndex = 0;

        //// ���������� ������� ����� � ����������
        //FilterComboBox.ItemsSource = DBContext.EisenhowerMatrices
        //    .Select(s => s.Name)
        //    .Prepend("���")
        //    .ToList();
        //FilterComboBox.SelectedIndex = 0;
    }
    private async void ListsOfTasks_ItemDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (ListsOfTasks.SelectedItem is List selectedList && selectedList.Id != 0)
        {
            // ��������� ���� �������������� ������
            AddEditListOfTasksWindow addEditListOfTasksWindow = new AddEditListOfTasksWindow(userID, selectedList.Id);
            await addEditListOfTasksWindow.ShowDialog(this);
        }
        else
        {
            // ���� ������ "���", ������ �� ������
            return;
        }

        // ��������� ������ ����� �������� ����
        LoadData();
    }

    private void MoodButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        Mood moodWindow = new Mood(userID);
        moodWindow.Show();
        Close();
    }

    private void MatrixButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        EisenhowerMatrixWindow matrixWindow = new EisenhowerMatrixWindow(userID);
        matrixWindow.Show();
        Close();
    }

    private void HorizontalScrollViewerWithoutShift(object? sender, Avalonia.Input.PointerWheelEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            var scrollViewer = listBox.GetVisualDescendants()
                .OfType<ScrollViewer>()
                .FirstOrDefault();

            if (scrollViewer != null)
            {
                // e.Delta.Y > 0 � ��������� �����, < 0 � ����
                scrollViewer.Offset = scrollViewer.Offset.WithX(
                    scrollViewer.Offset.X - e.Delta.Y * 40 // 40 � �������� ���������, ����� ��������
                );
                e.Handled = true;
            }
        }
    }

    private void GoToStatisticsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        StatisticsWindow statisticsWindow = new StatisticsWindow();
        statisticsWindow.Show();
        Close();
    }
}