using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Models.Common;
using Seemon.Todo.Models.Settings;
using Seemon.Todo.Views.Pages;

using Windows.System;

namespace Seemon.Todo.ViewModels.Pages;

public class ShellViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly ISystemService _systemService;
    private readonly ITaskService _taskService;
    private readonly IRecentFilesService _recentFilesService;
    private readonly ILocalSettingsService _localSettingsService;

    private readonly ViewSettings _viewSettings;
    private readonly TodoSettings _todoSettings;

    private bool _isBackEnabled;
    private bool _isMenuVisible;
    private bool _isRecentEnabled;
    private bool _isQuickSearchFocused;

    private ICommand? _goBackCommand;

    private ICommand? _showSettingsCommand;
    private ICommand? _showAboutCommand;
    private ICommand? _quickSearchFocusedChangedCommand;

    private ICommand? _newTodoCommand;
    private ICommand? _openTodoCommand;
    private ICommand? _reloadTodoFileCommand;
    private ICommand? _archiveCompletedTasksCommand;
    private ICommand? _openRecentCommand;
    private ICommand? _clearRecentCommand;
    private ICommand? _applicationExitCommand;

    private ICommand? _addNewTaskCommand;
    private ICommand? _addMultipleNewTaskCommand;
    private ICommand? _updateTaskCommand;
    private ICommand? _deleteTaskCommand;
    private ICommand? _toggleCompletedCommand;
    private ICommand? _toggleHiddenCommand;
    private ICommand? _setPriorityCommand;
    private ICommand? _clearPriorityCommand;
    private ICommand? _increasePriorityCommand;
    private ICommand? _decreasePriorityCommand;
    private ICommand? _setDateCommand;
    private ICommand? _postponeDateCommand;
    private ICommand? _clearDateCommand;
    private ICommand? _increaseDateCommand;
    private ICommand? _decreaseDateCommand;

    private ICommand? _featureNotImplementedCommand;

    public bool IsBackEnabled
    {
        get => _isBackEnabled; set => SetProperty(ref _isBackEnabled, value);
    }

    public bool IsMenuVisible
    {
        get => _isMenuVisible; set => SetProperty(ref _isMenuVisible, value);
    }

    public bool IsRecentEnabled
    {
        get => _isRecentEnabled; set => SetProperty(ref _isRecentEnabled, value);
    }

    public bool IsQuickSearchFocused
    {
        get => _isQuickSearchFocused; set => SetProperty(ref _isQuickSearchFocused, value);
    }

    public INavigationService NavigationService { get; }

    public ViewSettings ViewSettings => _viewSettings;

    public ObservableCollection<RecentFile> RecentFiles => _recentFilesService.RecentFiles;

    public ICommand GoBackCommand => _goBackCommand ??= RegisterCommand(OnGoBack);

    public ICommand ShowSettingsCommand => _showSettingsCommand ??= RegisterCommand(OnShowSettings);
    public ICommand ShowAboutCommand => _showAboutCommand ??= RegisterCommand(OnShowAbout);
    public ICommand QuickSearchFocusedChangedCommand => _quickSearchFocusedChangedCommand ??= RegisterCommand<string>(OnQuickSearchFocusedChanged);

    public ICommand NewTodoCommand => _newTodoCommand ??= RegisterCommand(OnNewTodo);
    public ICommand OpenTodoCommad => _openTodoCommand ??= RegisterCommand(OnOpenTodo);
    public ICommand ReloadTodoFileCommand => _reloadTodoFileCommand ??= RegisterCommand(OnReloadTodoFile, CanReloadTodoFile);
    public ICommand ArchiveCompletedTasksCommand => _archiveCompletedTasksCommand ??= RegisterCommand(OnArchiveCompletedTasks, CanArchiveCompletedTasks);
    public ICommand OpenRecentCommand => _openRecentCommand ??= RegisterCommand<string>(OnOpenRecent);
    public ICommand ClearRecentCommand => _clearRecentCommand ??= RegisterCommand(OnClearRecent, CanFileClearRecent);
    public ICommand ApplicationExitCommand => _applicationExitCommand ??= RegisterCommand(OnApplicationExit);

    public ICommand AddNewTaskCommand => _addNewTaskCommand ??= RegisterCommand(OnAddNewTask, CanAddNewTasks);
    public ICommand AddMultipleNewTasksCommand => _addMultipleNewTaskCommand ??= RegisterCommand(OnAddMultipleNewTasks, CanAddNewTasks);
    public ICommand UpdateTaskCommand => _updateTaskCommand ??= RegisterCommand(OnUpdateTask, CanUpdateTask);
    public ICommand DeleteTaskCommand => _deleteTaskCommand ??= RegisterCommand(OnDeleteTask, CanDeleteTask);
    public ICommand ToggleCompletedCommand => _toggleCompletedCommand ??= RegisterCommand(OnToggleCompleted, CanToggleCompleted);
    public ICommand ToggleHiddenCommand => _toggleHiddenCommand ??= RegisterCommand(OnToggleHidden, CanToggleHidden);
    public ICommand SetPriorityCommand => _setPriorityCommand ??= RegisterCommand(OnSetPriority, CanSetPriority);
    public ICommand ClearPriorityCommand => _clearPriorityCommand ??= RegisterCommand(OnClearPriority, CanClearPriority);
    public ICommand IncreasePriorityCommand => _increasePriorityCommand ??= RegisterCommand(OnIncreasePriority, CanIncreasePriority);
    public ICommand DecreasePriorityCommand => _decreasePriorityCommand ??= RegisterCommand(OnDecreasePriority, CanDecreasePriority);
    public ICommand SetDateCommand => _setDateCommand ??= RegisterCommand<string>(OnSetDate, CanSetDate);
    public ICommand PostponeDateCommand => _postponeDateCommand ??= RegisterCommand<string>(OnPostponeDate, CanPostponeDate);
    public ICommand ClearDateCommand => _clearDateCommand ??= RegisterCommand<string>(OnClearDate, CanClearDate);
    public ICommand IncreaseDateCommad => _increaseDateCommand ??= RegisterCommand<string>(OnIncreaseDate, CanIncreaseDate);
    public ICommand DecreaseDateCommad => _decreaseDateCommand ??= RegisterCommand<string>(OnDecreaseDate, CanDecreaseDate);

    public ICommand FeatureNotImplementedCommand => _featureNotImplementedCommand ??= RegisterCommand<string>(OnFeatureNotImplemented);

    public ShellViewModel(INavigationService navigationService, IDialogService dialogService, ISystemService systemService, ITaskService taskService, IRecentFilesService recentFilesService, ILocalSettingsService localSettingsService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;

        _dialogService = dialogService;
        _systemService = systemService;

        _taskService = taskService;
        _taskService.ActiveTasks.CollectionChanged += OnActiveTasksCollectionChanged;

        _recentFilesService = recentFilesService;
        _recentFilesService.RecentFiles.CollectionChanged += OnRecentFilesCollectionChanged;

        _localSettingsService = localSettingsService;
        _viewSettings = Task.Run(() => _localSettingsService.ReadSettingAsync<ViewSettings>(Constants.SETTING_VIEW)).Result ?? ViewSettings.Default;
        _todoSettings = Task.Run(() => _localSettingsService.ReadSettingAsync<TodoSettings>(Constants.SETTING_TODO)).Result ?? TodoSettings.Default;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;
        IsMenuVisible = NavigationService.Frame?.Content.GetType() == typeof(MainPage);
        IsRecentEnabled = _recentFilesService.RecentFiles.Count > 0;
    }

    private void OnGoBack() => NavigationService.GoBack();

    private void OnShowSettings() => NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);

    private void OnShowAbout() => NavigationService.NavigateTo(typeof(AboutViewModel).FullName!);

    private void OnQuickSearchFocusedChanged(string value) => IsQuickSearchFocused = bool.Parse(value);

    private async void OnNewTodo()
    {
        var path = await _systemService.OpenSaveDialogAsync();
        if (string.IsNullOrEmpty(path)) return;

        if (!File.Exists(path))
        {
            using var stream = File.Create(path);
        }

        OpenTodo(path);
    }

    private async void OnOpenTodo()
    {
        var path = await _systemService.OpenFileDialogAsync();
        if (string.IsNullOrEmpty(path)) return;

        OpenTodo(path);
    }

    private bool CanReloadTodoFile() => _taskService.IsLoaded;

    private void OnReloadTodoFile() => _taskService.ReloadTasks();

    private bool CanArchiveCompletedTasks() => _taskService.ActiveTasks.Any(t => t.IsCompleted);

    private void OnArchiveCompletedTasks() => _taskService.ArchiveCompletedTasks();

    private async void OnOpenRecent(string path)
    {
        if (!File.Exists(path))
        {
            await _dialogService.ShowMessageAsync("Open todo file", $"The todo file you are trying to open does not exisits.\n\n{path}");
            return;
        }
        OpenTodo(path);
    }

    private bool CanFileClearRecent() => _recentFilesService.RecentFiles.Count > 0;

    private void OnClearRecent() => _recentFilesService.Clear();

    private void OnApplicationExit() => Application.Current.Exit();

    private bool CanAddNewTasks() => _taskService.IsLoaded;

    private async void OnAddNewTask()
    {
        var response = await _dialogService.ShowDialogAsync<TaskPage>("Add new task");
        if (response != null)
        {
            _taskService.AddTask(response.BindableString.Trim());
        }
    }

    private async void OnAddMultipleNewTasks()
    {
        var response = await _dialogService.ShowDialogAsync<MultipleTaskPage>("Add multiple new task");
        if (response != null)
        {
            var tasks = response.BindableString.ReplaceLineEndings().Split(Environment.NewLine);
            foreach (var task in tasks)
            {
                _taskService.AddTask(task.Trim());
            }
        }
    }

    private bool CanUpdateTask() => _taskService.SelectedTasks.Count == 1;

    private async void OnUpdateTask()
    {
        var model = new Models.Common.BindableModel
        {
            BindableString = _taskService.SelectedTasks.First().Raw,
        };
        var response = await _dialogService.ShowDialogAsync<TaskPage>("Update Task", model);
        if (response != null)
        {
            _taskService.UpdateTask(_taskService.SelectedTasks.First(), response.BindableString);
        }
    }

    private bool CanDeleteTask() => _taskService.SelectedTasks.Count > 0;

    private async void OnDeleteTask()
    {
        var confirmed = true;

        if (_todoSettings.ConfirmBeleteDelete)
        {
            confirmed = await _dialogService.ShowConfirmationAsync("Delete tasks", "Are you sure you want to delete the selected tasks?");
        }
        if (confirmed)
        {
            foreach (var task in _taskService.SelectedTasks.ToList())
            {
                _taskService.DeleteTask(task);
            }
        }
    }

    private bool CanToggleCompleted() => _taskService.SelectedTasks.Count > 0;

    private void OnToggleCompleted()
    {
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.ToggleCompleted(task);
        }
    }

    private bool CanToggleHidden() => _taskService.SelectedTasks.Count > 0;

    private void OnToggleHidden()
    {
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.ToggleHidden(task);
        }
    }

    private bool CanSetPriority() => _taskService.SelectedTasks.Count > 0;

    private async void OnSetPriority()
    {
        var lastSelectedTask = _taskService.SelectedTasks.LastOrDefault();
        var model = new BindableModel
        {
            BindableString = !string.IsNullOrEmpty(lastSelectedTask?.Priority) 
                ? lastSelectedTask.Priority 
                : !string.IsNullOrEmpty(_todoSettings.DefaultPriority) ? _todoSettings.DefaultPriority : "A",
        };
        var response = await _dialogService.ShowDialogAsync<PriorityPage>("Set Task Priority", model);
        if (response != null)
        {
            var priority = response.BindableString.ToUpper().Trim();
            priority = !string.IsNullOrWhiteSpace(priority) && char.IsLetter(priority[0]) ? priority : string.Empty;

            foreach (var task in _taskService.SelectedTasks.ToList())
            {
                _taskService.SetPriority(task, priority);
            }
        }
    }

    private bool CanClearPriority() => _taskService.SelectedTasks.Count > 0;

    private void OnClearPriority()
    {
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.SetPriority(task, string.Empty);
        }
    }

    private bool CanIncreasePriority()
    {
        return _taskService.SelectedTasks.Count switch
        {
            0 => false,
            > 1 => true,
            _ => _taskService.SelectedTasks[0].Priority != "A"
        };
    }

    private void OnIncreasePriority()
    {
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            ChangePriority(task, -1);
        }
    }

    private bool CanDecreasePriority()
    {
        return _taskService.SelectedTasks.Count switch
        {
            0 => false,
            > 1 => true,
            _ => _taskService.SelectedTasks[0].Priority != "Z"
        };
    }

    private void OnDecreasePriority()
    {
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            ChangePriority(task, 1);
        }
    }

    private bool CanSetDate(string parameter) => _taskService.SelectedTasks.Count > 0;

    private async void OnSetDate(string parameter)
    {
        DateTypes type = (DateTypes)Enum.Parse(typeof(DateTypes), parameter);
        var title = type == DateTypes.Due ? "Set Due Date" : "Set Threshold Date";

        var lastSelectedTask = _taskService.SelectedTasks.LastOrDefault();
        var model = new BindableModel
        {
            BindableDateTime = !string.IsNullOrEmpty(type == DateTypes.Due ? lastSelectedTask?.DueDate : lastSelectedTask?.ThresholdDate)
                ? Convert.ToDateTime(type == DateTypes.Due ? lastSelectedTask?.DueDate : lastSelectedTask?.ThresholdDate)
                : DateTime.Today,
        };
        var response = await _dialogService.ShowDialogAsync<DatePage>(title, model);
        if (response != null && response?.BindableDateTime != null)
        {
            var date = response.BindableDateTime.Value.Date.ToTodoDate();

            foreach (var task in _taskService.SelectedTasks.ToList())
            {
                _taskService.SetDate(task, date, type);
            }
        }
    }

    private bool CanPostponeDate(string parameter) => _taskService.SelectedTasks?.Count > 0;

    private async void OnPostponeDate(string parameter)
    {
        DateTypes type = (DateTypes)Enum.Parse(typeof(DateTypes), parameter);
        var title = type == DateTypes.Due ? "Postpone Task Due Date" : "Postpone Task Threshold Date";

        var model = new BindableModel();
        var response = await _dialogService.ShowDialogAsync<PostponePage>(title, model);

        if (response != null && !string.IsNullOrEmpty(response?.BindableString))
        {
            foreach (var task in _taskService.SelectedTasks.ToList())
            {
                _taskService.PostponeDate(task, response.BindableString, type);
            }
        }
    }

    private bool CanIncreaseDate(string parameter) => _taskService.SelectedTasks.Count > 0;

    private void OnIncreaseDate(string parameter)
    {
        DateTypes type = (DateTypes)Enum.Parse(typeof(DateTypes), parameter);
        
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.PostponeDate(task, "1", type);
        }
    }

    private bool CanDecreaseDate(string parameter) => _taskService.SelectedTasks.Count > 0;

    private void OnDecreaseDate(string parameter)
    {
        DateTypes type = (DateTypes)Enum.Parse(typeof(DateTypes), parameter);
        
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.PostponeDate(task, "-1", type);
        }
    }

    private bool CanClearDate(string parameter) => _taskService.SelectedTasks?.Count > 0;

    private void OnClearDate(string parameter)
    {
        DateTypes type = (DateTypes)Enum.Parse(typeof(DateTypes), parameter);
        
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.SetDate(task, string.Empty, type);
        }
    }

    private async void OnFeatureNotImplemented(string feature) => await _dialogService.ShowFeatureNotImpletmented(feature);

    private void OnActiveTasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => RaiseCommandCanExecute();

    private void OnRecentFilesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => IsRecentEnabled = _recentFilesService.RecentFiles.Count > 0;

    private void ChangePriority(Models.Task task, int shift)
    {
        if (string.IsNullOrEmpty(task.Priority))
        {
            _taskService.SetPriority(task, "A");
        }
        else
        {
            var current = task.Priority[0];
            var priority = (char)(current + shift);

            if (char.IsLetter(priority))
            {
                _taskService.SetPriority(task, priority.ToString());
            }
        }
    }

    private void OpenTodo(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        _taskService.LoadTasks(path);
        _viewSettings.QuickSearchString = string.Empty;
    }

    public override bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args)
    {
        if (NavigationService.Frame?.GetPageViewModel() is not IViewModel vm) return false;

        if (vm.ShellKeyEventTriggered(args)) return true;

        switch (args.KeyboardAccelerator.Key)
        {
            case VirtualKey.F1:
                OnShowAbout();
                return true;
            case VirtualKey.F3:
                OnQuickSearchFocusedChanged(true.ToString());
                return true;
            case VirtualKey.F10:
                OnShowSettings();
                return true;
            default:
                return false;
        }
    }
}