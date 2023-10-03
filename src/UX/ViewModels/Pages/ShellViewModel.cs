using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
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

    private bool _isBackEnabled;
    private bool _isMenuVisible;
    private bool _isRecentEnabled;

    private ICommand? _goBackCommand;

    private ICommand? _showSettingsCommand;
    private ICommand? _showAboutCommand;

    private ICommand? _newTodoCommand;
    private ICommand? _openTodoCommand;
    private ICommand? _reloadTodoFileCommand;
    private ICommand? _archiveCompletedTasksCommand;
    private ICommand? _openRecentCommand;
    private ICommand? _clearRecentCommand;
    private ICommand? _applicationExitCommand;

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

    public ObservableCollection<RecentFile> RecentFiles => _recentFilesService.RecentFiles;

    public ICommand GoBackCommand => _goBackCommand ??= RegisterCommand(OnGoBack);

    public ICommand ShowSettingsCommand => _showSettingsCommand ??= RegisterCommand(OnShowSettings);
    public ICommand ShowAboutCommand => _showAboutCommand ??= RegisterCommand(OnShowAbout);

    public ICommand NewTodoCommand => _newTodoCommand ??= RegisterCommand(OnNewTodo);
    public ICommand OpenTodoCommad => _openTodoCommand ??= RegisterCommand(OnOpenTodo);
    public ICommand ReloadTodoFileCommand => _reloadTodoFileCommand ?? RegisterCommand(OnReloadTodoFile, CanReloadTodoFile);
    public ICommand ArchiveCompletedTasksCommand => _archiveCompletedTasksCommand ??= RegisterCommand(OnArchiveCompletedTasks, CanArchiveCompletedTasks);
    public ICommand OpenRecentCommand => _openRecentCommand ??= RegisterCommand<string>(OnOpenRecent);
    public ICommand ClearRecentCommand => _clearRecentCommand ??= RegisterCommand(OnClearRecent, CanFileClearRecent);
    public ICommand ApplicationExitCommand => _applicationExitCommand ??= RegisterCommand(OnApplicationExit);
    public ICommand FeatureNotImplementedCommand => _featureNotImplementedCommand ??= RegisterCommand<string>(OnFeatureNotImplemented);

    public INavigationService NavigationService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService, IDialogService dialogService, ISystemService systemService, ITaskService taskService, IRecentFilesService recentFilesService)
    {
        _dialogService = dialogService;
        NavigationService = navigationService;
        _systemService = systemService;
        _taskService = taskService;
        _recentFilesService = recentFilesService;

        NavigationService.Navigated += OnNavigated;
        _taskService.ActiveTasks.CollectionChanged += OnActiveTasksCollectionChanged;
        _recentFilesService.RecentFiles.CollectionChanged += OnRecentFilesCollectionChanged;
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
        OpenTodo((path));
    }

    private bool CanFileClearRecent() => _recentFilesService.RecentFiles.Count > 0;

    private void OnClearRecent()
    {
        _recentFilesService.Clear();
    }

    private void OnApplicationExit() => Application.Current.Exit();

    private async void OnFeatureNotImplemented(string feature) => await _dialogService.ShowFeatureNotImpletmented(feature);

    private void OnActiveTasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => RaiseCommandCanExecute();
    private void OnRecentFilesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => IsRecentEnabled = _recentFilesService.RecentFiles.Count > 0;

    private void OpenTodo(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        _taskService.LoadTasks(path);
        _recentFilesService.Add(path);
    }

    public override bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args)
    {
        var vm = NavigationService.Frame?.GetPageViewModel() as IViewModel;

        if (vm.ShellKeyEventTriggered(args)) return true;

        switch (args.KeyboardAccelerator.Key)
        {
            case VirtualKey.F1:
                OnShowAbout();
                return true;
            case VirtualKey.F10:
                OnShowSettings();
                return true;
            default:
                return false;
        }
    }
}