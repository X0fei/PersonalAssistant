using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Context;
using PersonalAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAssistant.Helpers
{
    public static class DBContext
    {
        public static User8Context User8Context { get; set; } = new();
        public static List<User> Users { get; set; } = [.. User8Context.Users];
        public static List<Models.Task> Tasks { get; set; } = [.. User8Context.Tasks
            .Include(task => task.StatusNavigation)
            .Include(task => task.PriorityNavigation)
            .Include(task => task.Users)];
        public static List<Status> Statuses { get; set; } = [.. User8Context.Statuses];
        public static List<Models.List> Lists { get; set; } = [.. User8Context.Lists
            .Include(list => list.Tasks)
            .Include(task => task.Users)];
        public static List<Pfp> Pfps { get; set; } = [.. User8Context.Pfps];

        public static List<EisenhowerMatrix> EisenhowerMatrices { get; set; } = [.. User8Context.EisenhowerMatrices];

        public static Type PreviousWindowType { get; set; } = typeof(MainWindow);
        public static User? CurrentUser { get; set; } = null;

        public static User GetUserByLogin(string login)
        {
            return Users.FirstOrDefault(u => u.Login == login)
                ?? throw new InvalidOperationException($"Пользователь с логином '{login}' не найден.");
        }
    }
}
