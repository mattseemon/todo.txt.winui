using System.Windows.Input;

using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.ViewModels.Pages;

public class SettingsViewModel : ViewModelBase, INavigationAware
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ISettingsService _settingsService;
    private readonly ISystemService _systemService;
    private readonly IDialogService _dialogService;

    private readonly AppSettings _appSettings;
    private readonly TodoSettings _todoSettings;
    private readonly ViewSettings _viewSettings;

    private ICommand? _switchThemeCommand;
    private ICommand? _selectGlobalArchiveFileCommand;
    private ICommand? _textBoxFocusChangedCommand;

    private string _selectedTheme = ElementTheme.Default.ToString();
    private bool _textBoxIsFocused = false;
    private FontFamily _selectedFont = FontFamily.XamlAutoFontFamily;

    public string SelectedTheme
    {
        get => _selectedTheme; set => SetProperty(ref _selectedTheme, value);
    }

    public AppSettings AppSettings => _appSettings;
    public TodoSettings TodoSettings => _todoSettings;
    public ViewSettings ViewSettings => _viewSettings;

    public IList<string> Themes => Enum.GetValues(typeof(ElementTheme)).Cast<ElementTheme>().Select(e => e.ToString()).ToList();
    public IList<string> Priorities { get; private set; }
    public string[] Fonts => CanvasTextFormat.GetSystemFontFamilies().OrderBy(x => x).ToArray();
    public FontFamily SelectedFont
    {
        get => _selectedFont; set => SetProperty(ref _selectedFont, value);
    }

    public ICommand SwitchThemeCommand => _switchThemeCommand ??= RegisterCommand<SelectionChangedEventArgs>(OnSwitchTheme);
    public ICommand SelectGlobalArchiveFileCommand => _selectGlobalArchiveFileCommand ??= RegisterCommand(OnSelectGlobalArchiveFile);
    public ICommand TextBoxFocusChangedCommand => _textBoxFocusChangedCommand ??= RegisterCommand<string>(OnTextBoxFocusChanged);

    public SettingsViewModel(IThemeSelectorService themeSelectorService, ISettingsService settingsService, ISystemService systemService, IDialogService dialogService)
    {
        _themeSelectorService = themeSelectorService;
        _settingsService = settingsService;
        _systemService = systemService;
        _dialogService = dialogService;

        _appSettings = Task.Run(() => _settingsService.GetAsync(Constants.SETTING_APPLICATION, AppSettings.Default)).Result;
        _appSettings.PropertyChanged += OnAppSettingsPropertyChanged;

        _todoSettings = Task.Run(() => _settingsService.GetAsync(Constants.SETTING_TODO, TodoSettings.Default)).Result;
        _todoSettings.PropertyChanged += OnTodoSettingsPropertyChanged;

        _viewSettings = Task.Run(() => _settingsService.GetAsync(Constants.SETTING_VIEW, ViewSettings.Default)).Result;
        _viewSettings.PropertyChanging += OnViewSettingsPropertyChanging;

        Priorities = new List<string>
        {
            "None"
        };
        for (int i = 65; i < 90; i++)
        {
            Priorities.Add(((char)i).ToString());
        }

        SelectedTheme = _themeSelectorService.Theme.ToString();
        SelectedFont = string.IsNullOrEmpty(_appSettings.FontFamily) ? FontFamily.XamlAutoFontFamily : new FontFamily(_appSettings.FontFamily);
    }

    private async void OnSwitchTheme(SelectionChangedEventArgs? args)
        => await _themeSelectorService.SetThemeAsync((ElementTheme)Enum.Parse(typeof(ElementTheme), SelectedTheme));

    private async void OnSelectGlobalArchiveFile()
    {
        var filename = await _systemService.OpenFileDialogAsync();
        if (string.IsNullOrEmpty(filename)) return;

        if (!File.Exists(filename)) await _dialogService.ShowMessageAsync("Invalid file", $"The selected file does not exists");
        else TodoSettings.GlobalArchiveFilePath = filename;
    }

    private void OnTextBoxFocusChanged(string? value)
        => _textBoxIsFocused = Convert.ToBoolean(value);

    public void OnNavigatedTo(object parameter) => SelectedTheme = _themeSelectorService.Theme.ToString();

    public async void OnNavigatedFrom() => await _settingsService.PersistAsync();

    private async void OnAppSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(AppSettings.FontFamily))
        {
            SelectedFont = new FontFamily(_appSettings.FontFamily);
        }
        await _settingsService.SetAsync(Constants.SETTING_APPLICATION, _appSettings);
    }

    private async void OnTodoSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => await _settingsService.SetAsync(Constants.SETTING_TODO, _todoSettings);

    private async void OnViewSettingsPropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        => await _settingsService.SetAsync(Constants.SETTING_VIEW, _viewSettings);

    public override bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args)
    {
        return args.KeyboardAccelerator.Key switch
        {
            Windows.System.VirtualKey.Back => _textBoxIsFocused,
            _ => false,
        };
    }
}
