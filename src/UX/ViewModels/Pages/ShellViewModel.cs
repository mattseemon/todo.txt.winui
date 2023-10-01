using System.Collections.Specialized;
using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Views.Pages;
using Windows.System;

namespace Seemon.Todo.ViewModels.Pages;

public class ShellViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly ISystemService _systemService;
    private readonly ITaskService _taskService;

    private bool _isBackEnabled;
    private bool _isMenuVisible;

    private ICommand? _goBackCommand;

    private ICommand? _showSettingsCommand;
    private ICommand? _showAboutCommand;

    private ICommand? _fileNewTodoCommand;
    private ICommand? _fileOpenTodoCommand;
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

    public ICommand GoBackCommand => _goBackCommand ??= RegisterCommand(OnGoBack);

    public ICommand ShowSettingsCommand => _showSettingsCommand ??= RegisterCommand(OnShowSettings);
    public ICommand ShowAboutCommand => _showAboutCommand ??= RegisterCommand(OnShowAbout);

    public ICommand FileNewTodoCommand => _fileNewTodoCommand ??= RegisterCommand(OnFileNewTodo);
    public ICommand FileOpenTodoCommad => _fileOpenTodoCommand ??= RegisterCommand(OnFileOpenTodo);
    public ICommand FileExitCommand => _fileExitCommand ??= RegisterCommand(OnFileExit);
    public ICommand FeatureNotImplementedCommand => _featureNotImplementedCommand ??= RegisterCommand(OnFeatureNotImplemented);

    public INavigationService NavigationService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService, IDialogService dialogService, ISystemService systemService, ITaskService taskService)
    {
        _dialogService = dialogService;
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        _systemService = systemService;
        _taskService = taskService;
        _taskService.ActiveTasks.CollectionChanged += OnActiveTasksCollectionChanged;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;
        IsMenuVisible = NavigationService.Frame?.Content.GetType() == typeof(MainPage);
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
            using var stream =  File.Create(path);
        }

        _taskService.LoadTasks(path);
    }

    private async void OnFileOpenTodo()
    {
        var path = await _systemService.OpenFileDialogAsync();
        if (string.IsNullOrEmpty(path)) return;
        
        _taskService.LoadTasks(path);
    }

    private void OnFileExit() => Application.Current.Exit();

    private async void OnFeatureNotImplemented() => await _dialogService.ShowFeatureNotImpletmented();

    private void OnActiveTasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => RaiseCommandCanExecute();

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