namespace Seemon.Todo.Contracts.Storage;

public interface IStorage
{
    Task<T> GetAsync<T>(string key, T defaultValue);

    Task SetAsync<T>(string key, T value);

    Task PersistAsync();
}
