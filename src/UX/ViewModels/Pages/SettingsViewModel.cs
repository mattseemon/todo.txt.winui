using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.ViewModels;

namespace Seemon.Todo.ViewModels.Pages;

public class SettingsViewModel : ViewModelBase, INavigationAware
{
    private readonly IThemeSelectorService _themeSelectorService;

    private ICommand? _switchThemeCommand;

    private string _selectedTheme = ElementTheme.Default.ToString();

    public string SelectedTheme
    {
        get => _selectedTheme; set => SetProperty(ref _selectedTheme, value);
    }

    public IList<string> Themes => Enum.GetValues(typeof(ElementTheme)).Cast<ElementTheme>().Select(e => e.ToString()).ToList();

    public ICommand SwitchThemeCommand => _switchThemeCommand ??= RegisterCommand<SelectionChangedEventArgs>(OnSwitchTheme);

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        SelectedTheme = _themeSelectorService.Theme.ToString();
    }

    private async void OnSwitchTheme(SelectionChangedEventArgs args) => await _themeSelectorService.SetThemeAsync((ElementTheme)Enum.Parse(typeof(ElementTheme), SelectedTheme));

    public void OnNavigatedTo(object parameter) => SelectedTheme = _themeSelectorService.Theme.ToString();

    public void OnNavigatedFrom()
    {
    }
}
