using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PersonalAssistant.Helpers;
using PersonalAssistant.Models;
using System;

namespace PersonalAssistant;

public partial class StatisticsWindow : Window
{
    public StatisticsWindow()
    {
        InitializeComponent();
    }
    private void GoBackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DBContext.PreviousWindowType != null)
        {
            var previousWindow = (Window)Activator.CreateInstance(DBContext.PreviousWindowType, DBContext.CurrentUser.Id);
            previousWindow.Show();
            Close();
        }
    }
    private void AddTaskButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        AddEditTask addEditTask = new AddEditTask(DBContext.CurrentUser.Id);
        addEditTask.Show();
        Close();
    }
    private void MoodButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        Mood moodWindow = new Mood(DBContext.CurrentUser.Id);
        moodWindow.Show();
        Close();
    }
    private void MatrixButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        EisenhowerMatrixWindow matrixWindow = new EisenhowerMatrixWindow(DBContext.CurrentUser.Id);
        matrixWindow.Show();
        Close();
    }
}