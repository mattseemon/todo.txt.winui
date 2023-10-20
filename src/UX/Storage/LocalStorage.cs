using Seemon.Todo.Contracts.Storage;
using Seemon.Todo.Helpers.Common;

using Windows.Storage;

namespace Seemon.Todo.Storage;

public class LocalStorage : IStorage
{
    public async Task<T> GetAsync<T>(string key, T defaultValue)
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var value))
        {
            return await JsonHelper.ToObjectAsync<T>((string)value);
        }
        else
        {
            ApplicationData.Current.LocalSettings.Values[key] = defaultValue;
            return defaultValue;
        }
    }

    public async Task SetAsync<T>(string key, T value)
    {
        if (value == null) return;

        ApplicationData.Current.LocalSettings.Values[key] = await JsonHelper.StringifyAsync(value);
    }

    public Task PersistAsync() => Task.CompletedTask;
}
