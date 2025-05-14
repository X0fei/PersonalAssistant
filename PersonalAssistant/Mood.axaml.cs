using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PersonalAssistant.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using PersonalAssistant.Utils;
using PersonalAssistant.Context;

namespace PersonalAssistant;

public partial class Mood : Window
{
    public Mood()
    {
        InitializeComponent();
    }

    private DateTime _currentMonth;
    private readonly int _currentUserId; // Передайте сюда id текущего пользователя

    public Mood(int currentUserId)
    {
        InitializeComponent();
        _currentUserId = currentUserId;
        _currentMonth = DateTime.Today;

        PrevMonthBtn.Click += (_, _) => { _currentMonth = _currentMonth.AddMonths(-1); DrawCalendar(); };
        NextMonthBtn.Click += (_, _) => { _currentMonth = _currentMonth.AddMonths(1); DrawCalendar(); };

        DrawCalendar();
    }

    // ... (остальной код не меняется)

    private void DrawCalendar()
    {
        MonthText.Text = _currentMonth.ToString("MMMM yyyy");
        CalendarGrid.Children.Clear();

        var firstDay = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
        int startDayOfWeek = ((int)firstDay.DayOfWeek + 6) % 7; // Понедельник = 0

        List<Feeling> feelings;
        using (var context = new User8Context())
        {
            feelings = context.Feelings
                .Where(f => f.Users.Any(u => u.Id == _currentUserId)
                            && f.Date.Month == _currentMonth.Month
                            && f.Date.Year == _currentMonth.Year)
                .ToList();
        }

        for (int i = 0; i < startDayOfWeek; i++)
            CalendarGrid.Children.Add(new Border());

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
            var feeling = feelings.FirstOrDefault(f => f.Date.Date == date.Date);

            var border = new Border
            {
                Background = GetBrushByFeeling(feeling?.Level),
                Margin = new Thickness(2),
                CornerRadius = new CornerRadius(4),
                Height = 50,
                Width = 50,
                Child = new TextBlock
                {
                    Text = day.ToString(),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                }
            };

            border.DoubleTapped += async (_, _) =>
            {
                var dialog = new AddEditMoodWindow(_currentUserId, date);
                var result = await dialog.ShowDialog<bool?>(this);
                if (result == true)
                    DrawCalendar();
            };

            CalendarGrid.Children.Add(border);
        }
    }

    private IBrush GetBrushByFeeling(int? level)
    {
        if (level == null)
            return Brushes.LightGray;

        // Диапазоны по вашему ТЗ
        if (level < 15) return Brushes.Red;
        if (level < 30) return Brushes.OrangeRed;
        if (level < 45) return Brushes.Orange;
        if (level < 55) return Brushes.Yellow;
        if (level < 70) return Brushes.YellowGreen;
        if (level < 85) return Brushes.GreenYellow;
        return Brushes.Green;
    }

    private void GoBackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TasksWindow taskWindow = new TasksWindow(_currentUserId);
        taskWindow.Show();
        Close();
    }
}