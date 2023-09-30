using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.ViewModels;
using Windows.System;

namespace Seemon.Todo.ViewModels.Pages;

public class ShellViewModel : ViewModelBase
{
    private bool _isBackEnabled;

    private ICommand _goBackCommand = null;
    private ICommand _menuFileExitCommand = null;
    private ICommand _menuSettingsCommand = null;
    private ICommand _showAboutCommand = null;

    public bool IsBackEnabled
    {
        get => _isBackEnabled; set => SetProperty(ref _isBackEnabled, value);
    }

    public ICommand GoBackCommand => _goBackCommand ??= RegisterCommand(OnGoBack);
    public ICommand MenuFileExitCommand => _menuFileExitCommand ??= RegisterCommand(OnMenuFileExit);
    public ICommand MenuSettingsCommand => _menuSettingsCommand ??= RegisterCommand(OnMenuSettings);
    public ICommand ShowAboutCommand => _showAboutCommand ??= RegisterCommand(OnShowAbout);

    public INavigationService NavigationService { get; }

    public ShellViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
    }

    private void OnGoBack() => NavigationService.GoBack();

    private void OnNavigated(object sender, NavigationEventArgs e) => IsBackEnabled = NavigationService.CanGoBack;

    private void OnMenuFileExit() => Application.Current.Exit();

    private void OnMenuSettings() => NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);

    private void OnShowAbout() => NavigationService.NavigateTo(typeof(AboutViewModel).FullName!);

    public override bool ShellKeyEventTriggered(object parameter)
    {
        var result = false;
        var args = parameter as KeyboardAcceleratorInvokedEventArgs;
        switch (args.KeyboardAccelerator.Key)
        {
            case VirtualKey.F1:
                OnShowAbout();
                break;
            case VirtualKey.F10:
                OnMenuSettings();
                result = true;
                break;
            default:
                result = true;
                break;
        }
        return false;
    }
}
