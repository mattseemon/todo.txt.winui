using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.ViewModels.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Seemon.Todo.Views.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PriorityPage : Page
{
    public PriorityViewModel ViewModel { get; }

    public PriorityPage()
    {
        ViewModel = App.GetService<PriorityViewModel>();
        this.InitializeComponent();
    }
}
