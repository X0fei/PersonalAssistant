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
        public static int FailedTasksPercent;
        public static int VeryBadMoodPercent;
        public static int BadMoodPercent;
        public static int SlightlyBadMoodPercent;
        public static int NeutralMoodPercent;
        public static int SlightlyGoodMoodPercent;
        public static int GoodMoodPercent;
        public static int VeryGoodMoodPercent;
        public static void CalculateAll()
        {
            AvticeTasks = CalculateActiveTasks();
            CompletedTasks = CalculateCompletedTasks();
            OverdueTasks = CalculateOverdueTasks();
            CompletedTasksPercent = CalculateCompletedTasksPercent();
            MostFrequentMoodCategory = GetMostFrequentMoodCategory();
            MostFrequentMoodCategoryCount = GetMostFrequentMoodCategoryCount();
            FailedTasksPercent = CalculateFailedTasksPercent();
            VeryBadMoodPercent = CalculateVeryBadMoodPercent();
            BadMoodPercent = CalculateBadMoodPercent();
            SlightlyBadMoodPercent = CalculateSlightlyBadMoodPercent();
            NeutralMoodPercent = CalculateNeutralMoodPercent();
            SlightlyGoodMoodPercent = CalculateSlightlyGoodMoodPercent();
            GoodMoodPercent = CalculateGoodMoodPercent();
            VeryGoodMoodPercent = CalculateVeryGoodMoodPercent();
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
        public static int CalculateFailedTasksPercent()
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

            int completed = userTasks.Count(task => task.Status == 4);

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
        public static int CalculateVeryBadMoodPercent()
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
            {
                return 0;
            }

            // "Очень неприятно" — это уровень < 15
            int veryBadCount = feelings.Count(f => f.Level < 15);

            return (int)Math.Round((double)veryBadCount / feelings.Count * 100);
        }
        public static int CalculateBadMoodPercent()
        {
            var user = DBContext.CurrentUser;
            if (user == null) return 0;

            using var context = new User8Context();
            var feelings = context.Feelings
                .Where(f => f.Users.Any(u => u.Id == user.Id))
                .ToList();

            if (feelings.Count == 0) return 0;

            int count = feelings.Count(f => f.Level >= 15 && f.Level < 30);
            return (int)Math.Round((double)count / feelings.Count * 100);
        }
        public static int CalculateSlightlyBadMoodPercent()
        {
            var user = DBContext.CurrentUser;
            if (user == null) return 0;

            using var context = new User8Context();
            var feelings = context.Feelings
                .Where(f => f.Users.Any(u => u.Id == user.Id))
                .ToList();

            if (feelings.Count == 0) return 0;

            int count = feelings.Count(f => f.Level >= 30 && f.Level < 45);
            return (int)Math.Round((double)count / feelings.Count * 100);
        }
        public static int CalculateNeutralMoodPercent()
        {
            var user = DBContext.CurrentUser;
            if (user == null) return 0;

            using var context = new User8Context();
            var feelings = context.Feelings
                .Where(f => f.Users.Any(u => u.Id == user.Id))
                .ToList();

            if (feelings.Count == 0) return 0;

            int count = feelings.Count(f => f.Level >= 45 && f.Level < 55);
            return (int)Math.Round((double)count / feelings.Count * 100);
        }
        public static int CalculateSlightlyGoodMoodPercent()
        {
            var user = DBContext.CurrentUser;
            if (user == null) return 0;

            using var context = new User8Context();
            var feelings = context.Feelings
                .Where(f => f.Users.Any(u => u.Id == user.Id))
                .ToList();

            if (feelings.Count == 0) return 0;

            int count = feelings.Count(f => f.Level >= 55 && f.Level < 70);
            return (int)Math.Round((double)count / feelings.Count * 100);
        }
        public static int CalculateGoodMoodPercent()
        {
            var user = DBContext.CurrentUser;
            if (user == null) return 0;

            using var context = new User8Context();
            var feelings = context.Feelings
                .Where(f => f.Users.Any(u => u.Id == user.Id))
                .ToList();

            if (feelings.Count == 0) return 0;

            int count = feelings.Count(f => f.Level >= 70 && f.Level < 85);
            return (int)Math.Round((double)count / feelings.Count * 100);
        }
        public static int CalculateVeryGoodMoodPercent()
        {
            var user = DBContext.CurrentUser;
            if (user == null) return 0;

            using var context = new User8Context();
            var feelings = context.Feelings
                .Where(f => f.Users.Any(u => u.Id == user.Id))
                .ToList();

            if (feelings.Count == 0) return 0;

            int count = feelings.Count(f => f.Level >= 85);
            return (int)Math.Round((double)count / feelings.Count * 100);
        }

    }
}
