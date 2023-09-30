using System.Reflection;
using System.Windows.Input;

using Microsoft.UI.Xaml;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;
using Windows.ApplicationModel;

namespace Seemon.Todo.ViewModels.Pages;

public class SettingsViewModel : ViewModelBase
{
    private readonly IThemeSelectorService _themeSelectorService;

    private ICommand _switchThemeCommand;

    private ElementTheme _elementTheme;
    private string _versionDescription;

    public ElementTheme ElementTheme
    {
        get => _elementTheme; set => SetProperty(ref _elementTheme, value);
    }

    public string VersionDescription
    {
        get => _versionDescription; set => SetProperty(ref _versionDescription, value);
    }

    public ICommand SwitchThemeCommand => _switchThemeCommand ??= RegisterCommand<ElementTheme>(OnSwitchTheme);
    

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;

        ElementTheme = _themeSelectorService.Theme;
        VersionDescription = GetVersionDescription();
    }

    private async void OnSwitchTheme(ElementTheme elementTheme)
    {
        this.ElementTheme = elementTheme;
        await _themeSelectorService.SetThemeAsync(elementTheme);
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
