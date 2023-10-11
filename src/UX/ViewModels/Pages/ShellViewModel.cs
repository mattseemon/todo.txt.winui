using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;
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
        get => _isQuickSearchFocused;
        set => SetProperty(ref _isQuickSearchFocused, value);
    }

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

    public ICommand FeatureNotImplementedCommand => _featureNotImplementedCommand ??= RegisterCommand<string>(OnFeatureNotImplemented);

    public INavigationService NavigationService
    {
        get;
    }

    public ViewSettings ViewSettings => _viewSettings;

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

    private void OnClearRecent()
    {
        _recentFilesService.Clear();
    }

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
            var tasks = _taskService.SelectedTasks.ToList();
            foreach (var task in tasks)
            {
                _taskService.DeleteTask(task);
            }
        }
    }

    private bool CanToggleCompleted() => _taskService.SelectedTasks.Count > 0;

    private void OnToggleCompleted()
    {
        var tasks = _taskService.SelectedTasks.ToList();
        foreach(var task in tasks)
        {
            _taskService.ToggleCompleted(task);
        }
    }

    private async void OnFeatureNotImplemented(string feature) => await _dialogService.ShowFeatureNotImpletmented(feature);

    private void OnActiveTasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => RaiseCommandCanExecute();
    private void OnRecentFilesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => IsRecentEnabled = _recentFilesService.RecentFiles.Count > 0;

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