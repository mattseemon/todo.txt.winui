using Seemon.Todo.Helpers.ViewModels;

namespace Seemon.Todo.ViewModels.Pages;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
    }

    public List<string> Cats = new()
    {
        "Abyssinian",
        "Aegean",
        "American Bobtail",
    };
}
