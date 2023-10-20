namespace Seemon.Todo.Contracts.Services;

public interface ISettingsService
{
    Task<T> GetAsync<T>(string key, T defaultValue);

    Task SetAsync<T>(string key, T value);

    Task PersistAsync();
}
