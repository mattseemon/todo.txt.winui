using Newtonsoft.Json.Linq;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.Storage;
using Seemon.Todo.Exceptions;
using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.Storage;

public class FileStorage : IStorage
{
    private const string _defaultApplicationDataFolder = "Seemon.Todo/ApplicationData";
    private const string _defaultSettingsFile = "todo.settings.json";

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _applicationDataFolder;
    private readonly string _settingsFile;

    private readonly IFileService _fileService;
    private readonly FileSettingsOptions _options;

    private IDictionary<string, object> _settings;

    private bool _isInitialized;

    public FileStorage(IFileService fileService, FileSettingsOptions options)
    {
        _fileService = fileService;
        _options = options;

        _applicationDataFolder = Path.Combine(_localApplicationData, _options.ApplicationDataFolder ?? _defaultApplicationDataFolder);
        _settingsFile = _options.SettingsFile ?? _defaultSettingsFile;

        _settings = new Dictionary<string, object>();
    }

    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            _settings = await Task.Run(() => _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _settingsFile) ?? new Dictionary<string, object>());
            _isInitialized = true;
        }
    }


    public async Task<T> GetAsync<T>(string key, T defaultValue)
    {
        await InitializeAsync();

        if (_settings == null) return default;

        if (_settings.TryGetValue(key, out var value))
        {
            if (value is JObject jObject)
            {
                var setting = jObject.ToObject<T>() ?? throw new TaskException($"Could not convert stored json to {typeof(T)}.");
                _settings[key] = setting;
                return setting;
            }
            else if (value is JArray jArray)
            {
                var setting = jArray.ToObject<T>() ?? throw new TaskException($"Could not convert stored json to {typeof(T)}.");
                _settings[key] = setting;
                return setting;
            }
            else
            {
                return (T)value;
            }
        }
        else
        {
            if (defaultValue == null) return default;

            _settings[key] = defaultValue;
            return defaultValue;
        };
    }

    public async Task SetAsync<T>(string key, T value)
    {
        await InitializeAsync();

        if (_settings == null || value == null) return;

        _settings[key] = value;
        await Task.CompletedTask;
    }

    public async Task PersistAsync()
    {
        await Task.Run(() => _fileService.Save(_applicationDataFolder, _settingsFile, _settings));
    }
}
