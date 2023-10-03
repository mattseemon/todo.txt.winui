using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

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
    private readonly ISystemService _systemService;
    private readonly IDialogService _dialogService;

    private readonly AppSettings _appSettings;
    private readonly TodoSettings _todoSettings;

    private ICommand? _switchThemeCommand;
    private ICommand? _selectGlobalArchiveFileCommand;
    private ICommand? _textBoxFocusChangedCommand;

    private string _selectedTheme = ElementTheme.Default.ToString();
    private bool _textBoxIsFocused = false;

    public string SelectedTheme
    {
        get => _selectedTheme; set => SetProperty(ref _selectedTheme, value);
    }

    public AppSettings AppSettings => _appSettings;
    public TodoSettings TodoSettings => _todoSettings;

    public IList<string> Themes => Enum.GetValues(typeof(ElementTheme)).Cast<ElementTheme>().Select(e => e.ToString()).ToList();

    public ICommand SwitchThemeCommand => _switchThemeCommand ??= RegisterCommand<SelectionChangedEventArgs>(OnSwitchTheme);
    public ICommand SelectGlobalArchiveFileCommand => _selectGlobalArchiveFileCommand ??= RegisterCommand(OnSelectGlobalArchiveFile);
    public ICommand TextBoxFocusChangedCommand => _textBoxFocusChangedCommand ??= RegisterCommand<string>(OnTextBoxFocusChanged);

    public SettingsViewModel(IThemeSelectorService themeSelectorService, ILocalSettingsService localSettingsService, ISystemService systemService, IDialogService dialogService)
    {
        _themeSelectorService = themeSelectorService;
        _localSettingsService = localSettingsService;
        _systemService = systemService;
        _dialogService = dialogService;

        _appSettings = Task.Run(() => _localSettingsService.ReadSettingAsync<AppSettings>(Constants.SETTING_APPLICATION)).Result ?? AppSettings.Default;
        _appSettings.PropertyChanged += OnAppSettingsPropertyChanged;

        _todoSettings = Task.Run(() => _localSettingsService.ReadSettingAsync<TodoSettings>(Constants.SETTING_TODO)).Result ?? TodoSettings.Default;
        _todoSettings.PropertyChanged += OnTodoSettingsPropertyChanged;

        SelectedTheme = _themeSelectorService.Theme.ToString();
    }

    private async void OnSwitchTheme(SelectionChangedEventArgs args)
        => await _themeSelectorService.SetThemeAsync((ElementTheme)Enum.Parse(typeof(ElementTheme), SelectedTheme));

    private async void OnSelectGlobalArchiveFile()
    {
        var filename = await _systemService.OpenFileDialogAsync();
        if (string.IsNullOrEmpty(filename)) return;

        if (!File.Exists(filename)) await _dialogService.ShowMessageAsync("Invalid file", $"The selected file does not exists");
        else TodoSettings.GlobalArchiveFilePath = filename;
    }

    private void OnTextBoxFocusChanged(string value)
    {
        _textBoxIsFocused = Convert.ToBoolean(value);
    }

    public void OnNavigatedTo(object parameter) => SelectedTheme = _themeSelectorService.Theme.ToString();

    public void OnNavigatedFrom()
    {
    }

    private async void OnAppSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => await _localSettingsService.SaveSettingAsync(Constants.SETTING_APPLICATION, _appSettings);

    private async void OnTodoSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => await _localSettingsService.SaveSettingAsync(Constants.SETTING_TODO, _todoSettings);

    public override bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args)
    {
        switch (args.KeyboardAccelerator.Key)
        {
            case Windows.System.VirtualKey.Back:
                return _textBoxIsFocused;
        }
        return false;
    }
}
