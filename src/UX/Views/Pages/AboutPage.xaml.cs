using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.ViewModels.Pages;

namespace Seemon.Todo.Views.Pages;

public sealed partial class AboutPage : Page
{
    public AboutViewModel ViewModel { get; }

    public AboutPage()
    {
        ViewModel = App.GetService<AboutViewModel>();
        InitializeComponent();
    }
}
