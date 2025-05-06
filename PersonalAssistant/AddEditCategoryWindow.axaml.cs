using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace PersonalAssistant;

public partial class AddEditCategoryWindow : Window
{
    public AddEditCategoryWindow()
    {
        InitializeComponent();
    }

    public string? CategoryName { get; private set; }

    public AddEditCategoryWindow(string? initialName = null)
    {
        InitializeComponent();
        if (initialName != null)
            CategoryNameBox.Text = initialName;
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        var name = CategoryNameBox.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(name))
        {
            CategoryName = name;
            Close(CategoryName);
        }
    }
}