using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers;

namespace Seemon.Todo.ViewModels.Pages;

public class ShellViewModel : ViewModelBase
{
    private bool _isBackEnabled;

    private ICommand _goBackCommand = null;
    private ICommand _menuFileExitCommand = null;
    private ICommand _menuSettingsCommand = null;

    public bool IsBackEnabled
    {
        get => _isBackEnabled; set => SetProperty(ref _isBackEnabled, value);
    }

    public ICommand GoBackCommand => _goBackCommand ??= RegisterCommand(OnGoBack);
    public ICommand MenuFileExitCommand => _menuFileExitCommand ??= RegisterCommand(OnMenuFileExit);
    public ICommand MenuSettingsCommand => _menuSettingsCommand ??= RegisterCommand(OnMenuSettings);

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
}
