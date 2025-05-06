using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalAssistant;

public partial class TasksWindow : Window
{
    private int userID;
    private List<Models.Task> displayAllTasks;
    public TasksWindow()
    {
        InitializeComponent();
    }

    public TasksWindow(int userID)
    {
        InitializeComponent();
        this.userID = userID;
        displayAllTasks = Utils.DbContext.Tasks
            .Where(task => task.Users.Any(user => user.Id == this.userID))
            .ToList();
        AllTasksList.ItemsSource = displayAllTasks;
        FilterComboBox.ItemsSource = Utils.DbContext.Statuses
            .Select(s => s.Name)
            .Prepend("Все")
            .ToList();
        FilterComboBox.SelectedIndex = 0;
        FilterComboBox.SelectionChanged += FilterComboBox_SelectionChanged;
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

    private void FilterComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        string selectedStatus = FilterComboBox.SelectedItem?.ToString() ?? "Все";

        if (selectedStatus == "Все")
        {
            AllTasksList.ItemsSource = displayAllTasks;
        }
        else
        {
            AllTasksList.ItemsSource = displayAllTasks
                .Where(t => t.StatusNavigation?.Name == selectedStatus)
                .ToList();
        }
    }

}