using PersonalAssistant.Context;
using PersonalAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAssistant.Helpers
{
    public static class Statistics
    {
        public static int AvticeTasks;
        public static int CompletedTasks;
        public static int OverdueTasks;
        public static int CompletedTasksPercent;
        public static string MostFrequentMoodCategory;
        public static int MostFrequentMoodCategoryCount;
        public static void CalculateAll()
        {
            AvticeTasks = CalculateActiveTasks();
            CompletedTasks = CalculateCompletedTasks();
            OverdueTasks = CalculateOverdueTasks();
            CompletedTasksPercent = CalculateCompletedTasksPercent();
            MostFrequentMoodCategory = GetMostFrequentMoodCategory();
            MostFrequentMoodCategoryCount = GetMostFrequentMoodCategoryCount();
        }
        public static int CalculateActiveTasks()
        {
            var user = DBContext.CurrentUser;
            if (user == null)
            {
                return 0;
            }

            return DBContext.Tasks
                .Where(task =>
                    task.Users.Any(u => u.Id == user.Id) &&
                    task.Status != 3 &&
                    task.Status != 4)
                .Count();
        }
        public static int CalculateCompletedTasks()
        {
            var user = DBContext.CurrentUser;
            if (user == null)
            {
                return 0;
            }

            return DBContext.Tasks
                .Where(task =>
                    task.Users.Any(u => u.Id == user.Id) &&
                    task.Status == 3)
                .Count();
        }
        public static int CalculateOverdueTasks()
        {
            var user = DBContext.CurrentUser;
            if (user == null)
            {
                return 0;
            }

            var now = DateTime.Now;

            return DBContext.Tasks
                .Where(task =>
                    task.Users.Any(u => u.Id == user.Id) &&
                    (
                        ((task.EndDate.HasValue && now > task.EndDate.Value) ||
                        (task.Deadline.HasValue && now > task.Deadline.Value)) &&
                        task.Status != 3 &&
                        task.Status != 4
                    )
                )
                .Count();
        }
        public static int CalculateCompletedTasksPercent()
        {
            var user = DBContext.CurrentUser;
            if (user == null)
            {
                return 0;
            }

            var userTasks = DBContext.Tasks
                .Where(task => task.Users.Any(u => u.Id == user.Id));

            int total = userTasks.Count();
            if (total == 0)
            {
                return 0;
            }

            int completed = userTasks.Count(task => task.Status == 3);

            return (int)Math.Round((double)completed / total * 100);
        }
        public static string GetMostFrequentMoodCategory()
        {
            var user = DBContext.CurrentUser;
            if (user == null)
            {
                return "Нет";
            }

            using var context = new User8Context();
            var feelings = context.Feelings
                .Where(f => f.Users.Any(u => u.Id == user.Id))
                .ToList();

            if (feelings.Count == 0)
                return "Нет";

            // Локальная функция для определения категории по уровню настроения
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

            var groups = feelings
                .GroupBy(f => GetCategoryByLevel(f.Level))
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();

            int maxCount = groups.Max(g => g.Count);
            var mostFrequent = groups.Where(g => g.Count == maxCount).ToList();

            if (mostFrequent.Count > 1)
                return "Нет явного лидера";

            return mostFrequent[0].Category;
        }
        public static int GetMostFrequentMoodCategoryCount()
        {
            var user = DBContext.CurrentUser;
            if (user == null)
            {
                return 0;
            }

            using var context = new User8Context();
            var feelings = context.Feelings
                .Where(f => f.Users.Any(u => u.Id == user.Id))
                .ToList();

            if (feelings.Count == 0)
                return 0;

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

            var groups = feelings
                .GroupBy(f => GetCategoryByLevel(f.Level))
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();

            int maxCount = groups.Max(g => g.Count);
            return maxCount;
        }
    }
}
