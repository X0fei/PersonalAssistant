using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalAssistant;

public partial class TasksWindow : Window
{
    int userID;
    private List<Models.Task> displayAllTasks = Utils.DbContext.tasks.ToList();
    public TasksWindow()
    {
        InitializeComponent();
    }

    public TasksWindow(int userID)
    {
        InitializeComponent();
        this.userID = userID;
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
}