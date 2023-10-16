namespace Seemon.Todo.Contracts.Services;

public interface ILocalSettingsService
{
    Task<T> ReadSettingAsync<T>(string key, T defaultValue);

    Task SaveSettingAsync<T>(string key, T value);
}
