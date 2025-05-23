using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PersonalAssistant.Helpers;
using PersonalAssistant.Models;
using System.Linq;

namespace PersonalAssistant;

public partial class EisenhowerMatrixWindow : Window
{
    private int userID;

    public EisenhowerMatrixWindow()
    {
        InitializeComponent();
    }

    public EisenhowerMatrixWindow(int userID)
    {
        InitializeComponent();
        this.userID = userID;
        Load();
    }

    private void Load()
    {
        UrgentImportantListBox.ItemsSource = DBContext.Tasks
            .Where(t => t.EisenhowerMatrix == 5).Where(t => t.Users.Any(u => u.Id == userID))
            .ToList();
        UrgentNotImportantListBox.ItemsSource = DBContext.Tasks
            .Where(t => t.EisenhowerMatrix == 4).Where(t => t.Users.Any(u => u.Id == userID))
            .ToList();
        NotUrgentImportantListBox.ItemsSource = DBContext.Tasks
            .Where(t => t.EisenhowerMatrix == 3).Where(t => t.Users.Any(u => u.Id == userID))
            .ToList();
        NotUrgentNotImportantListBox.ItemsSource = DBContext.Tasks
            .Where(t => t.EisenhowerMatrix == 2).Where(t => t.Users.Any(u => u.Id == userID))
            .ToList();
    }

    private void GoBackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TasksWindow tasksWindow = new(userID);
        tasksWindow.Show();
        Close();
    }

    private void Item_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(EisenhowerMatrixWindow);
        if ((sender as ListBox).SelectedItem is Models.Task selectedTask)
        {
            AddEditTask addEditTask = new AddEditTask(userID, selectedTask.Id);
            addEditTask.Show();
            Close();
        }
    }
}