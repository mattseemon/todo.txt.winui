using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.ViewModels.Pages;

public class SettingsViewModel : ViewModelBase, INavigationAware
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly AppSettings _appSettings;

    private ICommand? _switchThemeCommand;

    private string _selectedTheme = ElementTheme.Default.ToString();

    public string SelectedTheme
    {
        get => _selectedTheme; set => SetProperty(ref _selectedTheme, value);
    }

    public AppSettings AppSettings => _appSettings;

    public IList<string> Themes => Enum.GetValues(typeof(ElementTheme)).Cast<ElementTheme>().Select(e => e.ToString()).ToList();

    public ICommand SwitchThemeCommand => _switchThemeCommand ??= RegisterCommand<SelectionChangedEventArgs>(OnSwitchTheme);

    public SettingsViewModel(IThemeSelectorService themeSelectorService, ILocalSettingsService localSettingsService)
    {
        _themeSelectorService = themeSelectorService;
        _localSettingsService = localSettingsService;

        _appSettings = Task.Run(() => _localSettingsService.ReadSettingAsync<AppSettings>(Constants.SETTING_APPLICATION)).Result ?? AppSettings.Default;
        _appSettings.PropertyChanged += OnAppSettingsPropertyChanged;

        SelectedTheme = _themeSelectorService.Theme.ToString();
    }


    private async void OnSwitchTheme(SelectionChangedEventArgs args) => await _themeSelectorService.SetThemeAsync((ElementTheme)Enum.Parse(typeof(ElementTheme), SelectedTheme));

    public void OnNavigatedTo(object parameter) => SelectedTheme = _themeSelectorService.Theme.ToString();

    public async void OnNavigatedFrom()
    {
    }

    private async void OnAppSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) => await _localSettingsService.SaveSettingAsync(Constants.SETTING_APPLICATION, _appSettings);
}
