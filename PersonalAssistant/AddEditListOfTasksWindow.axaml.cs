using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Models;
using PersonalAssistant.Utils.Context;
using System.Collections.Generic;
using System.Linq;

namespace PersonalAssistant;

public partial class AddEditListOfTasksWindow : Window
{
    private int userID;
    private User currentUser;
    public AddEditListOfTasksWindow()
    {
        InitializeComponent();
    }

    public string? CategoryName { get; private set; }

    public AddEditListOfTasksWindow(int userID)
    {
        InitializeComponent();
        this.userID = userID;
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        var name = ListOfTasksName.Text?.Trim();

        if (!string.IsNullOrWhiteSpace(name))
        {
            using (var context = new User8Context())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userID);
                Models.List newList = new Models.List()
                {
                    Name = name,
                    Users = new List<User> { user }
                };
                
                context.Lists.Add(newList);
                context.SaveChanges();
            }

            Utils.DbContext.User8Context = new();
            Utils.DbContext.Lists = Utils.DbContext.User8Context.Lists
                .Include(list => list.Tasks)
                .Include(task => task.Users)
                .ToList();

            Close();
        }
    }
}