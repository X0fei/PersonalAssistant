using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PersonalAssistant.Context;
using PersonalAssistant.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PersonalAssistant;

public partial class AddEditMoodWindow : Window
{
    public AddEditMoodWindow()
    {
        InitializeComponent();
    }

    private readonly int _userId;
    private readonly DateTime _date;
    private Feeling? _feeling;

    public string DateText => _date.ToString("dd MMMM yyyy");

    public int Level
    {
        get => (int)LevelSlider.Value;
        set => LevelSlider.Value = value;
    }

    public AddEditMoodWindow(int userId, DateTime date)
    {
        InitializeComponent();
        _userId = userId;
        _date = date;

        LevelSlider.PropertyChanged += (_, e) =>
        {
            if (e.Property == Avalonia.Controls.Slider.ValueProperty)
                UpdateEmotionText((int)LevelSlider.Value);
        };

        SaveBtn.Click += SaveBtn_Click;
        DeleteBtn.Click += DeleteBtn_Click;

        this.Opened += (_, _) => LoadFeeling();
    }

    private void LoadFeeling()
    {
        using var context = new User8Context();
        _feeling = context.Feelings
            .Where(f => f.Users.Any(u => u.Id == _userId) && f.Date.Date == _date.Date)
            .FirstOrDefault();

        if (_feeling != null)
        {
            Level = _feeling.Level;
            DeleteBtn.IsVisible = true;
        }
        else
        {
            Level = 50;
            DeleteBtn.IsVisible = false;
        }
        UpdateEmotionText(Level);
    }

    private void UpdateEmotionText(int level)
    {
        EmotionText.Text = GetEmotionNameByLevel(level);
    }

    private string GetEmotionNameByLevel(int level)
    {
        int emotionId = level switch
        {
            < 15 => 7,
            < 30 => 6,
            < 45 => 5,
            < 55 => 4,
            < 70 => 3,
            < 85 => 2,
            _ => 1
        };

        using var context = new User8Context();
        var emotion = context.Emotions.FirstOrDefault(e => e.Id == emotionId);
        return emotion?.Name ?? "Неизвестно";
    }

    private void SaveBtn_Click(object? sender, RoutedEventArgs e)
    {
        using var context = new User8Context();
        var user = context.Users.First(u => u.Id == _userId);

        var feeling = context.Feelings
            .FirstOrDefault(f => f.Users.Any(u => u.Id == _userId) && f.Date.Date == _date.Date);

        if (feeling == null)
        {
            feeling = new Feeling
            {
                Level = Level,
                Date = _date,
                Users = new List<User> { user }
            };
            context.Feelings.Add(feeling);
        }
        else
        {
            feeling.Level = Level;
        }
        context.SaveChanges();
        Close(true);
    }

    private void DeleteBtn_Click(object? sender, RoutedEventArgs e)
    {
        using var context = new User8Context();
        var feeling = context.Feelings
            .FirstOrDefault(f => f.Users.Any(u => u.Id == _userId) && f.Date.Date == _date.Date);

        if (feeling != null)
        {
            feeling.Users.ToList().ForEach(user => user.Feelings.Remove(feeling));
            feeling.Emotions.ToList().ForEach(emotion => emotion.Feelings.Remove(feeling));
            context.Feelings.Remove(feeling);
            context.SaveChanges();

            context.SaveChanges();
        }
        Close(true);
    }
}