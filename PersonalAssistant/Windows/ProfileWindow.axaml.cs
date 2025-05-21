using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PersonalAssistant.Models;
using System.IO;
using System.Linq;

namespace PersonalAssistant;

public partial class ProfileWindow : Window
{
    private User user;
    private string? selectedImagePath;

    public ProfileWindow()
    {
        InitializeComponent();
    }

    public ProfileWindow(User user)
    {
        InitializeComponent();
        this.user = user;

        NameBox.Text = user.Name;
        BioBox.Text = user.Bio ?? "";
        LoginBox.Text = user.Login;
        // пароль не подставляем по соображениям безопасности
        LoadProfileImage();
    }

    private void LoadProfileImage()
    {
        var pfp = user.MainPfpNavigation;
        if (pfp != null && File.Exists(pfp.Path))
        {
            ProfileImage.Source = new Avalonia.Media.Imaging.Bitmap(pfp.Path);
        }
    }

    private async void AddPhotoButton_Click(object? sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filters.Add(new FileDialogFilter { Name = "Изображения", Extensions = { "jpg", "png", "jpeg" } });
        dialog.AllowMultiple = false;
        var result = await dialog.ShowAsync(this);
        if (result != null && result.Length > 0)
        {
            selectedImagePath = result[0];
            ProfileImage.Source = new Avalonia.Media.Imaging.Bitmap(selectedImagePath);
        }
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        var tasksWindow = new TasksWindow(user.Id);
        tasksWindow.Show();
        Close();
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        ErrorText.Text = "";

        string name = NameBox.Text?.Trim() ?? "";
        string bio = BioBox.Text?.Trim();
        string login = LoginBox.Text?.Trim() ?? "";
        string password = PasswordBox.Text ?? "";
        string confirm = ConfirmBox.Text ?? "";

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(login))
        {
            ErrorText.Text = "Имя и логин обязательны.";
            return;
        }

        if (login.Length < 4 || login.Length > 20 || !login.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
        {
            ErrorText.Text = "Логин должен быть от 4 до 20 символов и содержать только латиницу, цифры, -, _";
            return;
        }

        if (Utils.DBContext.Users.Any(u => u.Login == login && u.Id != user.Id))
        {
            ErrorText.Text = "Этот логин уже занят.";
            return;
        }

        if (!string.IsNullOrEmpty(password))
        {
            if (password.Length > 20)
            {
                ErrorText.Text = "Пароль не должен превышать 20 символов.";
                return;
            }
            if (password != confirm)
            {
                ErrorText.Text = "Пароли не совпадают.";
                return;
            }
            user.Password = password;
        }

        user.Name = name;
        user.Bio = bio;
        user.Login = login;

        if (selectedImagePath != null)
        {
            string fileName = Path.GetFileName(selectedImagePath);
            string newPath = Path.Combine("Images", fileName);
            Directory.CreateDirectory("Images");
            File.Copy(selectedImagePath, newPath, true);

            var pfp = new Pfp { Path = newPath };
            Utils.DBContext.User8Context.Pfps.Add(pfp);
            Utils.DBContext.User8Context.SaveChanges();

            user.MainPfp = pfp.Id;
        }

        Utils.DBContext.User8Context.SaveChanges();

        var tasksWindow = new TasksWindow(user.Id);
        tasksWindow.Show();
        Close();
    }
}