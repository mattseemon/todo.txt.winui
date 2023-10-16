using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.ViewModels.Pages;

namespace Seemon.Todo.Views.Pages;

public sealed partial class TaskPage : Page
{
    public TaskViewModel ViewModel { get; }

    public TaskPage()
    {
        ViewModel = App.GetService<TaskViewModel>();
        InitializeComponent();
    }
}
