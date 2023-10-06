using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.ViewModels.Pages;

namespace Seemon.Todo.Views.Pages;

public sealed partial class MultipleTaskPage : Page
{
    public MultipleTaskViewModel ViewModel { get; }

    public MultipleTaskPage()
    {
        ViewModel = App.GetService<MultipleTaskViewModel>();
        InitializeComponent();
    }
}
