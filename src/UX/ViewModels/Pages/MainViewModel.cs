using System.Collections.Specialized;

using CommunityToolkit.WinUI.UI;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.ViewModels.Pages;

public class MainViewModel : ViewModelBase, INavigationAware
{
    private readonly ITaskService _taskService;
    private readonly IRecentFilesService _recentFilesService;
    private readonly ILocalSettingsService _localSettingsService;

    private AppSettings _appSettings;

    public AdvancedCollectionView Tasks
    {
        get; private set;
    }

    public MainViewModel(ITaskService taskService, IRecentFilesService recentFilesService, ILocalSettingsService localSettingsService)
    {
        _taskService = taskService;
        _recentFilesService = recentFilesService;
        _localSettingsService = localSettingsService;

        _taskService.Loaded += OnTasksLoaded;
        _taskService.ActiveTasks.CollectionChanged += OnActiveTasksCollectionChanged;

        Tasks = new AdvancedCollectionView(_taskService.ActiveTasks, true);

        _appSettings = Task.Run(() => _localSettingsService?.ReadSettingAsync<AppSettings>(Constants.SETTING_APPLICATION)).Result ?? AppSettings.Default;
        if (_appSettings.OpenRecentOnStartup && !_taskService.IsLoaded)
        {
            var recent = _recentFilesService.RecentFiles.FirstOrDefault();
            if (recent != null)
            {
                if (File.Exists(recent.Path))
                {
                    _taskService.LoadTasks(recent.Path);
                }
                else
                {
                    _recentFilesService.Remove(recent.Path);
                }
            }
        }
    }

    private void OnActiveTasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Tasks?.Refresh();

    private void OnTasksLoaded(object? sender, string e)
    {
        Tasks.Refresh();
    }

    public void OnNavigatedTo(object parameter)
    {
        _appSettings = Task.Run(() => _localSettingsService?.ReadSettingAsync<AppSettings>(Constants.SETTING_APPLICATION)).Result ?? AppSettings.Default;
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
