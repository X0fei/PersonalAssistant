using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PersonalAssistant.Helpers;
using PersonalAssistant.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PersonalAssistant;

public partial class EisenhowerMatrixWindow : Window
{
    private int userID;
    private List<Models.Task> displayAllTasks = DBContext.Tasks;
    // Коллекция задач на выбранную дату
    private ObservableCollection<Models.Task> _tasksOnSelectedDate = new ObservableCollection<Models.Task>();

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
        if (DBContext.PreviousWindowType != null)
        {
            var previousWindow = (Window)Activator.CreateInstance(DBContext.PreviousWindowType, userID);
            previousWindow.Show();
            Close();
        }
    }
    private void MoodButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        Mood moodWindow = new Mood(userID);
        moodWindow.Show();
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
        DBContext.PreviousWindowType = typeof(EisenhowerMatrixWindow);
        if ((sender as ListBox).SelectedItem is Models.Task selectedTask)
        {
            AddEditTask addEditTask = new AddEditTask(userID, selectedTask.Id);
            addEditTask.Show();
            Close();
        }
    }
    private void TasksCalendar_SelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        Calendar calendar = sender as Calendar;
        var date = calendar.SelectedDate;
        if (date != null)
        {
            var tasks = displayAllTasks.Where(t =>
                t.Users.Any(u => u.Id == userID) &&
                ((t.StartDate?.Date <= date && t.EndDate?.Date >= date) ||
                (t.StartDate?.Date == null && t.EndDate?.Date == date) ||
                (t.StartDate?.Date == date && t.EndDate?.Date == null) ||
                (t.Deadline?.Date == date))).ToList();

            _tasksOnSelectedDate.Clear();
            foreach (var task in tasks)
                _tasksOnSelectedDate.Add(task);

            TasksByDateList.ItemsSource = _tasksOnSelectedDate;
        }
        CurrentDateTextBlock.Text = char.ToUpper((char)(date?.Date.ToString("dddd", new System.Globalization.CultureInfo("ru-RU"))[0])) +
                   date?.Date.ToString("dddd, d MMMM", new System.Globalization.CultureInfo("ru-RU")).Substring(1);
    }
}