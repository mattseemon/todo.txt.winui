using CommunityToolkit.WinUI.UI;
using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.ViewModels;


namespace Seemon.Todo.ViewModels.Pages;

public class MainViewModel : ViewModelBase, INavigationAware
{
    private readonly ITaskService _taskService;

    public AdvancedCollectionView Tasks
    {
        get; private set;
    }

    public MainViewModel(ITaskService taskService)
    {
        _taskService = taskService;
        _taskService.Loaded += OnTasksLoaded;
    }

    private void OnTasksLoaded(object? sender, string e)
    {
        Tasks.Refresh();
    }

    public void OnNavigatedTo(object parameter)
    {
        Tasks = new AdvancedCollectionView(_taskService.ActiveTasks, true);
    }

    public void OnNavigatedFrom()
    {
    }

    public List<string> Cats = new()
    {
        "Abyssinian",
        "Aegean",
        "American Bobtail",
    };
}
