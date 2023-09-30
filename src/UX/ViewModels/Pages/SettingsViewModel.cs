using System.Windows.Input;

using Microsoft.UI.Xaml;

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

    public ICommand SwitchThemeCommand => _switchThemeCommand ??= RegisterCommand<ElementTheme>(OnSwitchTheme);
    
    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        ElementTheme = _themeSelectorService.Theme;
    }

    private async void OnSwitchTheme(ElementTheme elementTheme)
    {
        this.ElementTheme = elementTheme;
        await _themeSelectorService.SetThemeAsync(elementTheme);
    }
}
