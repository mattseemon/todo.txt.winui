using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

namespace Seemon.Todo.Models.Settings;

public class TodoSettings : ObservableObject
{
    private bool _addCreatedDate = true;
    private string _defaultPriority = string.Empty;
    private bool _archiveCompleted = false;
    private bool _autoArchive = false;
    private bool _enableGlobalArchive = false;
    private string _globalArchiveFilePath = string.Empty;
    private bool _confirmBeforeDelete = true;

    [JsonProperty("addCreatedDate")]
    public bool AddCreatedDate
    {
        get => _addCreatedDate; set => SetProperty(ref _addCreatedDate, value);
    }

    [JsonProperty("defaultPriority")]
    public string DefaultPriority
    {
        get => _defaultPriority; set => SetProperty(ref _defaultPriority, value);
    }

    [JsonProperty("archiveCompleted")]
    public bool ArchiveCompleted
    {
        get => _archiveCompleted;
        set
        {
            SetProperty(ref _archiveCompleted, value);
            if (!value)
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

    [JsonProperty("confirmBeforeDelete")]
    public bool ConfirmBeleteDelete
    {
        get => _confirmBeforeDelete; set => SetProperty(ref _confirmBeforeDelete, value);
    }

    public static TodoSettings Default => new()
    {
        AddCreatedDate = true,
        DefaultPriority = "None",
        ArchiveCompleted = false,
        AutoArchive = false,
        EnableGlobalArchive = false,
        GlobalArchiveFilePath = string.Empty,
        ConfirmBeleteDelete = true,
    };
}
