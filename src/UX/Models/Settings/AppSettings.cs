using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

namespace Seemon.Todo.Models.Settings;

public class AppSettings : ObservableObject
{
    private string _theme = "Default";
    private string _lastOpenedPath = string.Empty;
    private ObservableCollection<RecentFile> _recentFiles;
    private bool _openRecentOnStartup = false;
    private int _maxRecentFileCount = 10;
    private bool _autoRefreshFile = false;

    public AppSettings()
    {
        _recentFiles = new ObservableCollection<RecentFile>();
    }

    [JsonProperty("theme")]
    public string Theme
    {
        get => _theme; set => SetProperty(ref _theme, value);
    }

    [JsonProperty("lastOpenedPath")]
    public string LastOpenedPath
    {
        get => _lastOpenedPath; set => SetProperty(ref _lastOpenedPath, value);
    }

    [JsonProperty("recentFiles")]
    public ObservableCollection<RecentFile> RecentFiles
    {
        get => _recentFiles; set => SetProperty(ref _recentFiles, value);
    }

    [JsonProperty("openRecentOnStartup")]
    public bool OpenRecentOnStartup
    {
        get => _openRecentOnStartup; set => SetProperty(ref _openRecentOnStartup, value);
    }

    [JsonProperty("maxRecentFileCount")]
    public int MaxRecentFileCount
    {
        get => _maxRecentFileCount; set => SetProperty(ref _maxRecentFileCount, value);
    }

    [JsonProperty("autoRefreshFile")]
    public bool AutoRefreshFile
    {
        get => _autoRefreshFile; set => SetProperty(ref _autoRefreshFile, value);
    }

    public static AppSettings Default => new()
    {
        Theme = "Default",
        LastOpenedPath = string.Empty,
        RecentFiles = new ObservableCollection<RecentFile>(),
        OpenRecentOnStartup = false,
        MaxRecentFileCount = 10,
        AutoRefreshFile = false,
    };
}
