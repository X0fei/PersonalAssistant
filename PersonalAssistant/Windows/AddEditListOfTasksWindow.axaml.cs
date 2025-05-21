using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Context;
using PersonalAssistant.Helpers;
using PersonalAssistant.Models;
using System.Collections.Generic;
using System.Linq;

namespace PersonalAssistant;

public partial class AddEditListOfTasksWindow : Window
{
    public AddEditListOfTasksWindow()
    {
        InitializeComponent();
    }

    private int userID;
    private User currentUser;
    private int? listID;

    public string? CategoryName { get; private set; }

    public AddEditListOfTasksWindow(int userID)
    {
        InitializeComponent();

        this.userID = userID;

        DeleteButton.IsVisible = false;
    }

    public AddEditListOfTasksWindow(int userID, int? listID = null)
    {
        InitializeComponent();

        this.userID = userID;
        this.listID = listID;

        if (listID.HasValue)
        {
            // Загружаем данные списка из БД
            var list = DBContext.Lists.FirstOrDefault(l => l.Id == listID.Value);
            if (list != null)
            {
                ListOfTasksName.Text = list.Name;
            }
        }
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string? listName = ListOfTasksName.Text?.Trim();

        if (string.IsNullOrEmpty(listName))
        {
            return;
        }

        using (var context = new User8Context())
        {
            if (listID.HasValue)
            {
                // Редактирование существующего списка
                var list = context.Lists.FirstOrDefault(l => l.Id == listID.Value);
                if (list != null)
                {
                    list.Name = listName;
                    context.SaveChanges();
                }
            }
            else
            {
                // Создание нового списка
                var newList = new List
                {
                    Name = listName,
                    Users = new List<User> { context.Users.First(u => u.Id == userID) }
                };
                context.Lists.Add(newList);
                context.SaveChanges();
            }
        }

        DBContext.User8Context = new User8Context();
        DBContext.Lists = DBContext.User8Context.Lists
            .Include(list => list.Tasks)
            .Include(list => list.Users)
            .ToList();

        Close();
    }
    private void DeleteButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (listID.HasValue)
        {
            using (var context = new User8Context())
            {
                var list = context.Lists
                    .Include(l => l.Users)
                    .Include(l => l.Tasks)
                    .FirstOrDefault(l => l.Id == listID.Value);

                if (list != null)
                {
                    // Удаляем связи с пользователями  
                    list.Users.Clear();

                    // Удаляем связи с задачами  
                    list.Tasks.Clear();

                    // Удаляем сам список  
                    context.Lists.Remove(list);
                    context.SaveChanges();
                }
            }
        }

        DBContext.User8Context = new User8Context();
        DBContext.Lists = DBContext.User8Context.Lists
            .Include(list => list.Tasks)
            .Include(list => list.Users)
            .ToList();

        Close();
    }
}