using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Models;
using PersonalAssistant.Utils.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAssistant.Utils
{
    public static class DbContext
    {
        public static User8Context User8Context { get; set; } = new();
        public static List<User> Users { get; set; } = [.. User8Context.Users];
        public static List<Models.Task> Tasks { get; set; } = [.. User8Context.Tasks
            .Include(task => task.StatusNavigation)
            .Include(task => task.PriorityNavigation)
            .Include(task => task.Users)];
    }
}
