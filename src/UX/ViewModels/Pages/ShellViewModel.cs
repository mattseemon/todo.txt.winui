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

    private bool _isBackEnabled;
    private bool _isMenuVisible;

    private ICommand? _goBackCommand;
    private ICommand? _menuFileExitCommand;
    private ICommand? _menuSettingsCommand;
    private ICommand? _showAboutCommand;
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
    public ICommand MenuFileExitCommand => _menuFileExitCommand ??= RegisterCommand(OnMenuFileExit);
    public ICommand MenuSettingsCommand => _menuSettingsCommand ??= RegisterCommand(OnMenuSettings);
    public ICommand ShowAboutCommand => _showAboutCommand ??= RegisterCommand(OnShowAbout);
    public ICommand FeatureNotImplementedCommand => _featureNotImplementedCommand ??= RegisterCommand(OnFeatureNotImplemented);

    public INavigationService NavigationService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService, IDialogService dialogService)
    {
        _dialogService = dialogService;
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
    }

    private void OnGoBack() => NavigationService.GoBack();

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;
        IsMenuVisible = NavigationService.Frame?.Content.GetType() == typeof(MainPage);
    }

    private void OnMenuFileExit() => Application.Current.Exit();

    private void OnMenuSettings() => NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);

    private void OnShowAbout() => NavigationService.NavigateTo(typeof(AboutViewModel).FullName!);

    private async void OnFeatureNotImplemented() => await _dialogService.ShowFeatureNotImpletmented();

    public override bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args)
    {
        switch (args.KeyboardAccelerator.Key)
        {
            case VirtualKey.F1:
                OnShowAbout();
                return true;
            case VirtualKey.F10:
                OnMenuSettings();
                return true;
            default:
                return false;
        }
    }
}