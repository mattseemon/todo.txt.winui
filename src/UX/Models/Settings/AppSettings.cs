using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

using System.Collections.ObjectModel;

namespace Seemon.Todo.Models.Settings;

public class AppSettings : ObservableObject
{
    private string _theme = "Default";
    private string _lastOpenedPath = string.Empty;
    private ObservableCollection<RecentFile> _recentFiles;
    private bool _openRecentOnStartup = false;
    private int _maxRecentFileCount = 10;
    private bool _autoRefreshFile = false;
    private string _fontFamily = string.Empty;
    private double _fontSize = 14;
    private bool _showStatusBar = true;
    private bool _alwaysOnTop = false;

    public AppSettings() => _recentFiles = new ObservableCollection<RecentFile>();

    [JsonProperty("theme")]
    public string Theme { get => _theme; set => SetProperty(ref _theme, value); }

    [JsonProperty("lastOpenedPath")]
    public string LastOpenedPath { get => _lastOpenedPath; set => SetProperty(ref _lastOpenedPath, value); }

    [JsonProperty("recentFiles")]
    public ObservableCollection<RecentFile> RecentFiles { get => _recentFiles; set => SetProperty(ref _recentFiles, value); }

    [JsonProperty("openRecentOnStartup")]
    public bool OpenRecentOnStartup { get => _openRecentOnStartup; set => SetProperty(ref _openRecentOnStartup, value); }

    [JsonProperty("maxRecentFileCount")]
    public int MaxRecentFileCount { get => _maxRecentFileCount; set => SetProperty(ref _maxRecentFileCount, value); }

    [JsonProperty("autoRefreshFile")]
    public bool AutoRefreshFile { get => _autoRefreshFile; set => SetProperty(ref _autoRefreshFile, value); }

    [JsonProperty("fontFamily")]
    public string FontFamily { get => _fontFamily; set => SetProperty(ref _fontFamily, value); }

    [JsonProperty("fontSize")]
    public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }

    [JsonProperty("showStatusBar")]
    public bool ShowStatusBar { get => _showStatusBar; set => SetProperty(ref _showStatusBar, value); }

    [JsonProperty("alwaysOnTop")]
    public bool AlwaysOnTop { get => _alwaysOnTop; set => SetProperty(ref _alwaysOnTop, value); }

    public static AppSettings Default => new()
    {
        Theme = "Default",
        LastOpenedPath = string.Empty,
        RecentFiles = new ObservableCollection<RecentFile>(),
        OpenRecentOnStartup = false,
        MaxRecentFileCount = 10,
        AutoRefreshFile = false,
        FontFamily = string.Empty,
        ShowStatusBar = false,
        AlwaysOnTop = false,
    };
}
