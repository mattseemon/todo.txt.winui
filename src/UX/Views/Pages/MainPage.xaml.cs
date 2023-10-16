using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.ViewModels.Pages;

namespace Seemon.Todo.Views.Pages;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
