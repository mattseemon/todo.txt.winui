using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.ViewModels.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Seemon.Todo.Views.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PostponePage : Page
{
    public PostponeViewModel ViewModel { get; }

    public PostponePage()
    {
        ViewModel = App.GetService<PostponeViewModel>();
        InitializeComponent();
    }
}
