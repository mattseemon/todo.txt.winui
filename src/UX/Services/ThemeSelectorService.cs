using Microsoft.UI.Xaml;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.Services;

public class ThemeSelectorService : IThemeSelectorService
{
    public ElementTheme Theme { get; set; } = ElementTheme.Default;

    private readonly ILocalSettingsService _localSettingsService;

    public ThemeSelectorService(ILocalSettingsService localSettingsService)
        => _localSettingsService = localSettingsService;

    public async Task InitializeAsync()
    {
        Theme = await LoadThemeFromSettingsAsync();
        await Task.CompletedTask;
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;

        await SetRequestedThemeAsync();
        await SaveThemeInSettingsAsync(Theme);
    }

    public async Task SetRequestedThemeAsync()
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;

            TitleBarHelper.UpdateTitleBar(Theme);
        }

        await Task.CompletedTask;
    }

    private async Task<ElementTheme> LoadThemeFromSettingsAsync()
    {
        var appSettings = await _localSettingsService.ReadSettingAsync<AppSettings>(Constants.SETTING_APPLICATION) ?? AppSettings.Default;

        if (Enum.TryParse(appSettings?.Theme, out ElementTheme cacheTheme))
        {
            return cacheTheme;
        }

        return ElementTheme.Default;
    }

    private async Task SaveThemeInSettingsAsync(ElementTheme theme)
    {
        var appSettings = await _localSettingsService.ReadSettingAsync<AppSettings>(Constants.SETTING_APPLICATION) ?? AppSettings.Default;
        appSettings.Theme = theme.ToString();
        await _localSettingsService.SaveSettingAsync(Constants.SETTING_APPLICATION, appSettings);
    }
}
