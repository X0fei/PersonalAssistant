using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PersonalAssistant.Models;
using System.Collections.Generic;
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
        currentUser = Utils.DbContext.Users.First(u => u.Id == userID);

        LoadData();
    }

    private void GoBackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }

    private void AddTaskButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AddEditTask addEditTask = new AddEditTask(userID);
        addEditTask.Show();
        Close();
    }

    private void Item_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (AllTasksList.SelectedItem is Models.Task selectedTask)
        {
            AddEditTask addEditTask = new AddEditTask(userID, selectedTask.Id);
            addEditTask.Show();
            Close();
        }
    }

    private void FilterComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        string selectedPriority = FilterComboBox.SelectedItem?.ToString() ?? "���";

        if (selectedPriority == "���")
        {
            AllTasksList.ItemsSource = displayAllTasks;
        }
        else
        {
            AllTasksList.ItemsSource = displayAllTasks
                .Where(t => t.PriorityTableNavigation?.Name == selectedPriority)
                .ToList();
        }
    }

    private void ProfileButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
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
        ListsOfTasks.ItemsSource = Utils.DbContext.Lists
            .Where(l => l.Users.Any(u => u.Id == currentUser.Id))
            .ToList();
    }
    private void ListsOfTasks_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        displayAllTasks = Utils.DbContext.Tasks
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

    private void LoadData()
    {
        // ���������� ��� ������������ � ������ �������
        ProfileButton.Content = currentUser.Name;

        // ���������� ��� ������
        displayAllTasks = Utils.DbContext.Tasks
            .Where(t => t.Users.Any(u => u.Id == currentUser.Id))
            .ToList();
        AllTasksList.ItemsSource = displayAllTasks;

        // ���������� ������ �����
        var lists = Utils.DbContext.Lists
            .Where(l => l.Users.Any(u => u.Id == currentUser.Id))
            .ToList();

        // ��������� ������� "���" � ������ ������
        lists.Insert(0, new List { Id = 0, Name = "���" });

        ListsOfTasks.ItemsSource = lists;

        // ������������� "���" ��� ��������� �������
        ListsOfTasks.SelectedIndex = 0;

        // ���������� ������� ����� � ����������
        FilterComboBox.ItemsSource = Utils.DbContext.PriorityTables
            .Select(s => s.Name)
            .Prepend("���")
            .ToList();
        FilterComboBox.SelectedIndex = 0;
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
}