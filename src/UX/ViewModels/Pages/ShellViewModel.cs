using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

using Seemon.Todo.Contracts.Services;
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

    private ICommand? _fileNewTodoCommand;
    private ICommand? _fileOpenTodoCommand;
    private ICommand? _fileOpenRecentCommand;
    private ICommand? _fileClearRecentCommand;
    private ICommand? _fileExitCommand;

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

    public ICommand FileNewTodoCommand => _fileNewTodoCommand ??= RegisterCommand(OnFileNewTodo);
    public ICommand FileOpenTodoCommad => _fileOpenTodoCommand ??= RegisterCommand(OnFileOpenTodo);
    public ICommand FileOpenRecentCommand => _fileOpenRecentCommand ??= RegisterCommand<string>(OnFileOpenRecent);
    public ICommand FileClearRecentCommad => _fileClearRecentCommand ??= RegisterCommand(OnFileClearRecent, CanFileClearRecent);

    public ICommand FileExitCommand => _fileExitCommand ??= RegisterCommand(OnFileExit);
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

    private async void OnFileNewTodo()
    {
        var path = await _systemService.OpenSaveDialogAsync();
        if (string.IsNullOrEmpty(path)) return;

        if (!File.Exists(path))
        {
            using var stream = File.Create(path);
        }

        OpenTodo(path);
    }

    private async void OnFileOpenTodo()
    {
        var path = await _systemService.OpenFileDialogAsync();
        if (string.IsNullOrEmpty(path)) return;

        OpenTodo(path);
    }

    private void OnFileExit() => Application.Current.Exit();

    private async void OnFileOpenRecent(string path)
    {
        if (!File.Exists(path))
        {
            await _dialogService.ShowMessageAsync("Open todo file", $"The todo file you are trying to open does not exisits.\n\n{path}");
            return;
        }

        OpenTodo((path));
    }

    private bool CanFileClearRecent() => _recentFilesService.RecentFiles.Count > 0;

    private void OnFileClearRecent()
    {
        _recentFilesService.Clear();
    }

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