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
        Load();
    }
    private void Load()
    {
        Statistics.CalculateAll();
        CompletedTasksCountPercentTextBlock.Text = Statistics.CompletedTasksPercent.ToString() + "%";
        CompletedTasksDiagram.Height = Statistics.CompletedTasksPercent * 300 / 100;
        FailedTasksCountPercentTextBlock.Text = Statistics.FailedTasksPercent.ToString() + "%";
        FailedTasksDiagram.Height = Statistics.FailedTasksPercent * 300 / 100;
        VeryBadMoodPercentTextBlock.Text = Statistics.VeryBadMoodPercent.ToString() + "%";
        VeryBadMoodDiagram.Height = Statistics.VeryBadMoodPercent * 300 / 100;
        BadMoodPercentTextBlock.Text = Statistics.BadMoodPercent.ToString() + "%";
        BadMoodDiagram.Height = Statistics.BadMoodPercent * 300 / 100;
        SlightlyBadMoodPercentTextBlock.Text = Statistics.SlightlyBadMoodPercent.ToString() + "%";
        SlightlyBadMoodDiagram.Height = Statistics.SlightlyBadMoodPercent * 300 / 100;
        NeutralMoodPercentTextBlock.Text = Statistics.NeutralMoodPercent.ToString() + "%";
        NeutralMoodDiagram.Height = Statistics.NeutralMoodPercent * 300 / 100;
        SlightlyGoodMoodPercentTextBlock.Text = Statistics.SlightlyGoodMoodPercent.ToString() + "%";
        SlightlyGoodMoodDiagram.Height = Statistics.SlightlyGoodMoodPercent * 300 / 100;
        GoodMoodPercentTextBlock.Text = Statistics.GoodMoodPercent.ToString() + "%";
        GoodMoodDiagram.Height = Statistics.GoodMoodPercent * 300 / 100;
        VeryGoodMoodPercentTextBlock.Text = Statistics.VeryGoodMoodPercent.ToString() + "%";
        VeryGoodMoodDiagram.Height = Statistics.VeryGoodMoodPercent * 300 / 100;
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