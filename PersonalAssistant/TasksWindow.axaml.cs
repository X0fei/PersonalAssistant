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
        var border = sender as Border;
        int taskID = (int)border.Tag;
        AddEditTask addEditTask = new AddEditTask(userID, taskID);
        addEditTask.Show();
        Close();
    }
}