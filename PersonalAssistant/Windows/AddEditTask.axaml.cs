using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Context;
using PersonalAssistant.Helpers;
using PersonalAssistant.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PersonalAssistant;

public partial class AddEditTask : Window
{
    private int userID;
    private int? taskID = null;
    private List<Models.Task> displayAllTasks = DBContext.Tasks;
    // Коллекция задач на выбранную дату
    private ObservableCollection<Models.Task> _tasksOnSelectedDate = new ObservableCollection<Models.Task>();
    public AddEditTask()
    {
        InitializeComponent();
    }
    public AddEditTask(int userID)
    {
        InitializeComponent();
        this.userID = userID;
        GreetingTitle.Text = "Добавление задачи";
        // Подгружаем списки задач
        var lists = DBContext.Lists
            .Where(l => l.Users.Any(u => u.Id == this.userID))
            .ToList();

        // Добавляем элемент "Нет" в начало списка
        lists.Insert(0, new List { Id = 0, Name = "Нет" });

        ListOfTasksBox.ItemsSource = lists;
        ListOfTasksBox.SelectedIndex = 0;
        CalendarOfTasks.SelectedDate = DateTime.Today;
    }
    public AddEditTask(int userID, int taskID)
    {
        InitializeComponent();
        DeleteButton.IsVisible = true;
        this.userID = userID;
        this.taskID = taskID;

        GreetingTitle.Text = "Редактирование задачи";
        // Подгружаем списки задач
        var lists = DBContext.Lists
            .Where(l => l.Users.Any(u => u.Id == this.userID))
            .ToList();

        // Добавляем элемент "Нет" в начало списка
        lists.Insert(0, new List { Id = 0, Name = "Нет" });

        ListOfTasksBox.ItemsSource = lists;

        using (var context = new User8Context())
        {
            var task = context.Tasks
                .Include(t => t.Lists)
                .FirstOrDefault(t => t.Id == taskID);

            NameBox.Text = task.Name;
            DescriptionBox.Text = task.Description;
            if (task.Priority != null)
            {
                PriorityBox.SelectedIndex = (int)(task.Priority - 1);
            }
            if (task.EisenhowerMatrix != null)
            {
                PriorityTableBox.SelectedIndex = (int)(task.EisenhowerMatrix - 1);
            }
            StatusBox.SelectedIndex = task.Status - 1;

            // Устанавливаем выбранный элемент в ListOfTasksBox  
            var taskList = task.Lists.FirstOrDefault();
            if (taskList != null)
            {
                ListOfTasksBox.SelectedItem = ListOfTasksBox.ItemsSource
                    .Cast<Models.List>()
                    .FirstOrDefault(l => l.Id == taskList.Id);
            }
        }
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Получаем значения из DatePicker и TimePicker
        DateTime? startDateTime = null, endDateTime = null, deadlineDateTime = null;

        if (StartDatePicker.SelectedDate.HasValue && StartTimePicker.SelectedTime.HasValue)
            startDateTime = StartDatePicker.SelectedDate.Value.Date + StartTimePicker.SelectedTime.Value;

        if (EndDatePicker.SelectedDate.HasValue && EndTimePicker.SelectedTime.HasValue)
            endDateTime = EndDatePicker.SelectedDate.Value.Date + EndTimePicker.SelectedTime.Value;

        if (DeadlineDatePicker.SelectedDate.HasValue && DeadlineTimePicker.SelectedTime.HasValue)
            deadlineDateTime = DeadlineDatePicker.SelectedDate.Value.Date + DeadlineTimePicker.SelectedTime.Value;

        // Валидация дат
        if (startDateTime.HasValue && endDateTime.HasValue && startDateTime > endDateTime)
        {
            //await MessageBox.Show(this, "Дата начала не может быть позже даты окончания.", "Ошибка", MessageBox.MessageBoxButtons.Ok);
            return;
        }
        if (deadlineDateTime.HasValue)
        {
            if (startDateTime.HasValue && deadlineDateTime < startDateTime)
            {
                //await MessageBox.Show(this, "Дедлайн не может быть раньше даты начала.", "Ошибка", MessageBox.MessageBoxButtons.Ok);
                return;
            }
            if (endDateTime.HasValue && deadlineDateTime < endDateTime)
            {
                //await MessageBox.Show(this, "Дедлайн не может быть раньше даты окончания.", "Ошибка", MessageBox.MessageBoxButtons.Ok);
                return;
            }
        }


        if (taskID == null)
        {
            using (var context = new User8Context())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userID);

                List selectedList = ListOfTasksBox.SelectedItem as List;
                List? list = null;
                if (selectedList != null)
                {
                    list = context.Lists.FirstOrDefault(l => l.Id == selectedList.Id);
                    // Proceed with logic
                }
                else
                {
                    // Handle the case where no list is selected
                }

                if (string.IsNullOrEmpty(NameBox.Text))
                {
                    NameBox.Text = "Задача";
                }

                Models.Task newTask;
                if (ListOfTasksBox.SelectedIndex == 0)
                {
                    newTask = new Models.Task()
                    {
                        Name = NameBox.Text,
                        Description = DescriptionBox.Text,
                        Priority = PriorityBox.SelectedIndex + 1,
                        EisenhowerMatrix = PriorityTableBox.SelectedIndex + 1,
                        Status = StatusBox.SelectedIndex + 1,
                        CreationDate = DateTime.Now,
                        Users = new List<User> { user },
                        StartDate = startDateTime,
                        EndDate = endDateTime,
                        Deadline = deadlineDateTime
                    };
                }
                else
                {
                    newTask = new Models.Task()
                    {
                        Name = NameBox.Text,
                        Description = DescriptionBox.Text,
                        Priority = PriorityBox.SelectedIndex + 1,
                        EisenhowerMatrix = PriorityTableBox.SelectedIndex + 1,
                        Status = StatusBox.SelectedIndex + 1,
                        CreationDate = DateTime.Now,
                        Users = new List<User> { user },
                        Lists = new List<Models.List> { list },
                        StartDate = startDateTime,
                        EndDate = endDateTime,
                        Deadline = deadlineDateTime
                    };
                }

                context.Add(newTask);
                context.SaveChanges();
            }
        }
        else
        {
            using (var context = new User8Context())
            {
                // Загружаем задачу с пользователями и списками  
                var task = context.Tasks
                    .Include(t => t.Users)
                    .Include(t => t.Lists)
                    .FirstOrDefault(t => t.Id == taskID);

                if (string.IsNullOrEmpty(NameBox.Text))
                {
                    NameBox.Text = "Задача";
                }

                // Обновляем поля задачи  
                task.Name = NameBox.Text;
                task.Description = DescriptionBox.Text;
                task.Priority = PriorityBox.SelectedIndex + 1;
                task.EisenhowerMatrix = PriorityTableBox.SelectedIndex + 1;
                task.Status = StatusBox.SelectedIndex + 1;
                task.StartDate = startDateTime;
                task.EndDate = endDateTime;
                task.Deadline = deadlineDateTime;

                if (ListOfTasksBox.SelectedIndex == 0)
                {
                    task.Lists.Clear();
                }
                else
                {
                    // Обновляем список, к которому относится задача
                    var selectedList = (Models.List)ListOfTasksBox.SelectedItem;
                    if (selectedList != null)
                    {
                        var newList = context.Lists.FirstOrDefault(l => l.Id == selectedList.Id);
                        task.Lists.Clear();
                        task.Lists.Add(newList);
                    }
                }
                
                context.SaveChanges();
            }
        }

        DBContext.User8Context = new();
        DBContext.Tasks = [.. DBContext.User8Context.Tasks
           .Include(task => task.StatusNavigation)
           .Include(task => task.PriorityNavigation)
           .Include(task => task.Users)];

        DBContext.Lists = [.. DBContext.User8Context.Lists
            .Include(list => list.Tasks)
            .Include(task => task.Users)];

        // Для создания окна того же типа, что и предыдущее, используйте сохранённый экземпляр или тип окна.
        // Если у вас есть только тип окна (например, DBContext.PreviousWindowType), создайте экземпляр через рефлексию:
        if (DBContext.PreviousWindowType != null)
        {
            var previousWindow = (Window)Activator.CreateInstance(DBContext.PreviousWindowType, userID);
            previousWindow.Show();
            Close();
        }
    }

    private void GoBackButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DBContext.PreviousWindowType != null)
        {
            var previousWindow = (Window)Activator.CreateInstance(DBContext.PreviousWindowType, userID);
            previousWindow.Show();
            Close();
        }
    }

    private void DeleteButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using (var context = new User8Context())
        {
            var task = context.Tasks
                .Include(t => t.Users)
                .FirstOrDefault(t => t.Id == taskID);

            task.Users.Clear();
            context.SaveChanges();

            context.Tasks.Remove(task);
            context.SaveChanges();
        }

        DBContext.User8Context = new();
        DBContext.Tasks = [.. DBContext.User8Context.Tasks
            .Include(task => task.StatusNavigation)
            .Include(task => task.PriorityNavigation)
            .Include(task => task.Users)];

        if (DBContext.PreviousWindowType != null)
        {
            var previousWindow = (Window)Activator.CreateInstance(DBContext.PreviousWindowType, userID);
            previousWindow.Show();
            Close();
        }
    }
    private void MoodButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        Mood moodWindow = new Mood(userID);
        moodWindow.Show();
        Close();
    }
    private void MatrixButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        EisenhowerMatrixWindow matrixWindow = new EisenhowerMatrixWindow(userID);
        matrixWindow.Show();
        Close();
    }
    private void Item_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (TasksByDateList.SelectedItem is Models.Task selectedTask)
        {
            DBContext.PreviousWindowType = typeof(TasksWindow);
            AddEditTask addEditTask = new AddEditTask(userID, selectedTask.Id);
            addEditTask.Show();
            Close();
        }
    }
    private void TasksCalendar_SelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        Calendar calendar = sender as Calendar;
        var date = calendar.SelectedDate;
        if (date != null)
        {
            var tasks = displayAllTasks.Where(t =>
                t.Users.Any(u => u.Id == userID) &&
                ((t.StartDate?.Date <= date && t.EndDate?.Date >= date) ||
                (t.StartDate?.Date == null && t.EndDate?.Date == date) ||
                (t.StartDate?.Date == date && t.EndDate?.Date == null) ||
                (t.Deadline?.Date == date))).ToList();

            _tasksOnSelectedDate.Clear();
            foreach (var task in tasks)
                _tasksOnSelectedDate.Add(task);

            TasksByDateList.ItemsSource = _tasksOnSelectedDate;
        }
        CurrentDateTextBlock.Text = char.ToUpper((char)(date?.Date.ToString("dddd", new System.Globalization.CultureInfo("ru-RU"))[0])) +
                   date?.Date.ToString("dddd, d MMMM", new System.Globalization.CultureInfo("ru-RU")).Substring(1);
    }
    private void GoToStatisticsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DBContext.PreviousWindowType = typeof(TasksWindow);
        StatisticsWindow statisticsWindow = new StatisticsWindow();
        statisticsWindow.Show();
        Close();
    }
}