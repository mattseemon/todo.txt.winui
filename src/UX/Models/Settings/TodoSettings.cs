using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

namespace Seemon.Todo.Models.Settings;

public class TodoSettings : ObservableObject
{
    private bool _archiveCompleted = false;
    private bool _autoArchive = false;
    private bool _enableGlobalArchive = false;
    private string _globalArchiveFilePath = string.Empty;

    [JsonProperty("archiveCompleted")]
    public bool ArchiveCompleted
    {
        get => _archiveCompleted;
        set
        {
            SetProperty(ref _archiveCompleted, value);
            if(!value)
            {
                EnableGlobalArchive = AutoArchive = value;
            }
        }
    }

    [JsonProperty("autoArchive")]
    public bool AutoArchive
    {
        get => _autoArchive; set => SetProperty(ref _autoArchive, value);
    }

    [JsonProperty("enableGlobalArchive")]
    public bool EnableGlobalArchive
    {
        get => _enableGlobalArchive; set => SetProperty(ref _enableGlobalArchive, value);
    }

    [JsonProperty("globalArchiveFilePath")]
    public string GlobalArchiveFilePath
    {
        get => _globalArchiveFilePath; set => SetProperty(ref _globalArchiveFilePath, value);
    }

    public static TodoSettings Default = new()
    {
        ArchiveCompleted = false,
        AutoArchive = false,
        EnableGlobalArchive = false,
        GlobalArchiveFilePath = string.Empty
    };
}
