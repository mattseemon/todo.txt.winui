using Microsoft.Extensions.Options;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.Storage;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Models.Settings;
using Seemon.Todo.Storage;

namespace Seemon.Todo.Services;

public class SettingsService : ISettingsService
{
    private IStorage _storage;

    public SettingsService(IFileService fileService, IOptions<FileSettingsOptions> options)
        => _storage = RuntimeHelper.IsMSIX ? new LocalStorage() : new FileStorage(fileService, options.Value);

    public async Task<T> GetAsync<T>(string key, T defaultValue)
        => await _storage.GetAsync(key, defaultValue);

    public async Task SetAsync<T>(string key, T value)
        => await _storage.SetAsync(key, value);

    public async Task PersistAsync()
        => await _storage.PersistAsync();
}
