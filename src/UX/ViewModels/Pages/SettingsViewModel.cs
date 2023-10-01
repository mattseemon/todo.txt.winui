using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.ViewModels;

namespace Seemon.Todo.ViewModels.Pages;

public class SettingsViewModel : ViewModelBase
{
    private readonly IThemeSelectorService _themeSelectorService;

    private ICommand _switchThemeCommand;

    private ElementTheme _elementTheme;

    public ElementTheme ElementTheme
    {
        get => _elementTheme; set => SetProperty(ref _elementTheme, value);
    }

    public ICommand SwitchThemeCommand => _switchThemeCommand ??= RegisterCommand<SelectionChangedEventArgs>(OnSwitchTheme);
    
    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        ElementTheme = _themeSelectorService.Theme;
    }

    private async void OnSwitchTheme(SelectionChangedEventArgs args)
    {
        await _themeSelectorService.SetThemeAsync(this.ElementTheme);
    }
}
