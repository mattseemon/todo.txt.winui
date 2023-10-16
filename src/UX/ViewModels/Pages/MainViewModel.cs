using System.ComponentModel;
using System.Windows.Input;

using CommunityToolkit.WinUI.UI;

using Microsoft.UI.Xaml.Input;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.ViewModels.Pages;

public class MainViewModel : ViewModelBase, INavigationAware
{
    private readonly ITaskService _taskService;
    private readonly IRecentFilesService _recentFilesService;
    private readonly ILocalSettingsService _localSettingsService;

    private AppSettings _appSettings;
    private readonly ViewSettings _viewSettings;

    private ICommand? _selectionChangedCommand;
    private ICommand? _doubleTappedCommand;

    public ICommand SelectionChangedCommand => _selectionChangedCommand ??= RegisterCommand(OnSelectionChanged);
    public ICommand DoubleTappedCommand => _doubleTappedCommand ??= RegisterCommand(OnDoubleTapped);

    public AdvancedCollectionView Tasks { get; private set; }

    public MainViewModel(ITaskService taskService, IRecentFilesService recentFilesService, ILocalSettingsService localSettingsService)
    {
        _taskService = taskService;
        _recentFilesService = recentFilesService;
        _localSettingsService = localSettingsService;

        _taskService.Loaded += OnTasksLoaded;
        _taskService.CollectionChanged += OnCollectionChanged;

        Tasks = new AdvancedCollectionView(_taskService.ActiveTasks)
        {
            Filter = QuickSearch
        };
        Tasks.SortDescriptions.Clear();

        _viewSettings = Task.Run(() => _localSettingsService.ReadSettingAsync<ViewSettings>(Constants.SETTING_VIEW)).Result ?? ViewSettings.Default;
        _viewSettings.PropertyChanged += OnViewSettingsPropertyChanged;

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

    private void OnSelectionChanged()
        => App.GetService<ShellViewModel>().RaiseCommandCanExecute();

    private void OnDoubleTapped()
        => App.GetService<ShellViewModel>().UpdateTaskCommand?.Execute(null);

    private void OnViewSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewSettings.QuickSearchString))
        {
            Tasks.Refresh();
        }
    }

    private bool QuickSearch(object item)
    {
        if (string.IsNullOrEmpty(_viewSettings.QuickSearchString)) return true;

        var task = (Models.Task)item;
        var today = DateTime.Today;
        var stringComparison = _viewSettings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        return _viewSettings.QuickSearchString switch
        {
            "DONE" => task.IsCompleted,
            "-DONE" => !task.IsCompleted,
            "due:today" => task.DueDate.IsDateEqualTo(today),
            "-due:today" => !task.DueDate.IsDateEqualTo(today),
            "due:tomorrow" => task.DueDate.IsDateEqualTo(today.AddDays(1)),
            "due:past" => task.DueDate.IsDateLesserThan(today),
            "-due:past" => !task.DueDate.IsDateLesserThan(today),
            "due:future" => task.DueDate.IsDateGreaterThan(today),
            "-due:future" => !task.DueDate.IsDateGreaterThan(today),
            "due:active" => (!task.DueDate.IsNullOrEmpty() && !task.DueDate.IsDateGreaterThan(today)),
            "-due:active" => !(!task.DueDate.IsNullOrEmpty() && !task.DueDate.IsDateGreaterThan(today)),
            _ => task.Raw.Contains(_viewSettings.QuickSearchString, stringComparison),
        };
    }

    private void OnCollectionChanged(object? sender, EventArgs e) => Tasks.Refresh();

    private void OnTasksLoaded(object? sender, string e) => Tasks.Refresh();

    public void OnNavigatedTo(object parameter)
    {
        _appSettings = Task.Run(() => _localSettingsService?.ReadSettingAsync<AppSettings>(Constants.SETTING_APPLICATION)).Result ?? AppSettings.Default;
        Tasks.Refresh();
    }

    public void OnNavigatedFrom() { }

    public override bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args)
        => base.ShellKeyEventTriggered(args);
}
