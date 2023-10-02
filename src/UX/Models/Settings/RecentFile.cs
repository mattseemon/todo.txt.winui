using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Seemon.Todo.Models.Settings;

public class RecentFile : ObservableObject
{
    private string _path;
    private DateTime _lastAccessed;

    [JsonProperty("path")]
    public string Path
    {
        get => _path; set => SetProperty(ref _path, value);
    }

    [JsonProperty("lastAccessed")]
    public DateTime LastAccessed
    {
        get => _lastAccessed; set => SetProperty(ref _lastAccessed, value);
    }
}
