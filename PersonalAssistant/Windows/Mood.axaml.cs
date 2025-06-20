using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PersonalAssistant.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using PersonalAssistant.Helpers;
using PersonalAssistant.Context;

namespace PersonalAssistant;

public partial class Mood : Window
{
    public Mood()
    {
        InitializeComponent();
    }

    private DateTime currentDate;
    private readonly int userID;

    public Mood(int currentUserId)
    {
        InitializeComponent();
        userID = currentUserId;
        currentDate = DateTime.Today;

        PrevMonthBtn.Click += (_, _) => { currentDate = currentDate.AddMonths(-1); DrawCalendar(); };
        NextMonthBtn.Click += (_, _) => { currentDate = currentDate.AddMonths(1); DrawCalendar(); };

        Load();
    }
    private void AddTaskButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        AddEditTask addEditTask = new AddEditTask(userID);
        addEditTask.Show();
        Close();
    }
    private void MatrixButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        EisenhowerMatrixWindow matrixWindow = new EisenhowerMatrixWindow(userID);
        matrixWindow.Show();
        Close();
    }
    private void Load()
    {
        DrawCalendar();

        Statistics.CalculateAll();
        MostFrequentMoodCategoryTextBlock.Text = Statistics.MostFrequentMoodCategory;
        MostFrequentMoodCategoryCountTextBlock.Text = Statistics.MostFrequentMoodCategoryCount.ToString();
    }
    private void DrawCalendar()
    {
        CurrentYear.Text = currentDate.ToString("yyyy");
        MonthText.Text = currentDate.ToString("MMMM");
        CalendarGrid.Children.Clear();

        var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        int daysInCurrentMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
        int startOffset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

        // Предыдущий месяц
        var prevMonth = currentDate.AddMonths(-1);
        int daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

        var nextMonth = currentDate.AddMonths(1);

        // Получаем список настроений за текущий месяц
        List<Feeling> feelings;
        using (var db = new User8Context())
        {
            feelings = db.Feelings
                .Where(f => f.Users.Any(u => u.Id == userID)
                         && f.Date.Month == currentDate.Month
                         && f.Date.Year == currentDate.Year)
                .ToList();
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly tomorrow = today.AddDays(1);

        // Всего 42 ячейки
        for (int i = 0; i < 42; i++)
        {
            DateOnly date;
            bool isCurrentMonth;

            if (i < startOffset)
            {
                // Дни предыдущего месяца
                int day = daysInPrevMonth - (startOffset - 1 - i);
                date = new DateOnly(prevMonth.Year, prevMonth.Month, day);
                isCurrentMonth = false;
            }
            else if (i < startOffset + daysInCurrentMonth)
            {
                // Дни текущего месяца
                int day = i - startOffset + 1;
                date = new DateOnly(currentDate.Year, currentDate.Month, day);
                isCurrentMonth = true;
            }
            else
            {
                // Дни следующего месяца
                int day = i - (startOffset + daysInCurrentMonth) + 1;
                date = new DateOnly(nextMonth.Year, nextMonth.Month, day);
                isCurrentMonth = false;
            }

            int? level = null;
            if (isCurrentMonth)
            {
                level = feelings.FirstOrDefault(f => f.Date.Day == date.Day)?.Level;
            }

            // Здесь проверяем, если дата >= завтрашнего — фон прозрачный и без клика
            bool isClickable = isCurrentMonth && date < tomorrow;

            CalendarGrid.Children.Add(CreateDayCell(date.Day, isCurrentMonth, date, level, isClickable));
        }

        UpdateWeekStat();
    }

    private Border CreateDayCell(int day, bool isCurrentMonth, DateOnly? date = null, int? level = null, bool isClickable = true)
    {
        var textBlock = new TextBlock
        {
            Text = day.ToString(),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Foreground = isCurrentMonth ? Brushes.Black : Brushes.Gray
        };

        var backgroundBrush = isCurrentMonth
            ? (isClickable ? GetBrushByFeeling(level) : Brushes.Transparent)
            : Brushes.Transparent;

        bool isToday = date == DateOnly.FromDateTime(DateTime.Today);

        var border = new Border
        {
            Background = backgroundBrush,
            Margin = new Thickness(2),
            CornerRadius = new CornerRadius(4),
            BorderThickness = isToday ? new Thickness(4) : (isCurrentMonth ? new Thickness(1) : new Thickness(0)),
            BorderBrush = isToday ? new SolidColorBrush(Color.Parse("#DEB464")) : new SolidColorBrush(Color.Parse("#373E46")),
            Height = 50,
            Width = 50,
            Child = textBlock
        };

        if (isClickable && isCurrentMonth && date != null)
        {
            border.DoubleTapped += async (_, _) =>
            {
                var dialog = new AddEditMoodWindow(userID, date.Value);
                var result = await dialog.ShowDialog<bool?>(this);
                if (result == true)
                {
                    Load();
                }
            };
        }

        return border;
    }


    private void UpdateWeekStat()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
        DateOnly thisMonday = today.AddDays(-daysSinceMonday);
        DateOnly lastMonday = thisMonday.AddDays(-7);
        DateOnly lastSunday = thisMonday.AddDays(-1);

        using var context = new User8Context();
        var lastWeekFeelings = context.Feelings
            .Where(f => f.Users.Any(u => u.Id == userID)
                        && f.Date >= lastMonday
                        && f.Date <= lastSunday)
            .ToList();

        if (lastWeekFeelings.Count == 0)
        {
            WeekStatText.Text = "Нет";
            return;
        }

        // Группировка по диапазонам уровня (уровень -> категория)
        string GetCategoryByLevel(int level)
        {
            if (level < 15) return "Очень неприятно";
            if (level < 30) return "Неприятно";
            if (level < 45) return "Отчасти неприятно";
            if (level < 55) return "Нейтрально";
            if (level < 70) return "Отчасти приятно";
            if (level < 85) return "Приятно";
            return "Очень приятно";
        }

        var groups = lastWeekFeelings
            .GroupBy(f => GetCategoryByLevel(f.Level))
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToList();

        int maxCount = groups.Max(g => g.Count);
        var mostFrequent = groups.Where(g => g.Count == maxCount).ToList();

        if (mostFrequent.Count > 1)
        {
            WeekStatText.Text = "Нет частого";
            return;
        }

        WeekStatText.Text = mostFrequent[0].Category;
    }


    private static string GetEmotionNameByLevel(int level)
    {
        int emotionId = level switch
        {
            < 15 => 7,
            < 30 => 6,
            < 45 => 5,
            < 55 => 4,
            < 70 => 3,
            < 85 => 2,
            <= 100 => 1
        };

        using var context = new User8Context();
        var emotion = context.Emotions.FirstOrDefault(e => e.Id == emotionId);
        return emotion?.Name ?? "Неизвестно";
    }

    private static IBrush GetBrushByFeeling(int? level)
    {
        if (level == null)
            return new SolidColorBrush(Color.Parse("#F9F0EB"));

        if (level < 15) return new SolidColorBrush(Color.Parse("#E98080"));
        if (level < 30) return new SolidColorBrush(Color.Parse("#F5B1B1"));
        if (level < 45) return new SolidColorBrush(Color.Parse("#FBE0E0"));
        if (level < 55) return new SolidColorBrush(Color.Parse("#EBF6FA"));
        if (level < 70) return new SolidColorBrush(Color.Parse("#E0F7DD"));
        if (level < 85) return new SolidColorBrush(Color.Parse("#D5EDBA"));
        return new SolidColorBrush(Color.Parse("#A8E6A1")); ;
    }

    private void GoBackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TasksWindow taskWindow = new(userID);
        taskWindow.Show();
        Close();
    }
    private void GoToStatisticsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        StatisticsWindow statisticsWindow = new StatisticsWindow();
        statisticsWindow.Show();
        Close();
    }
}