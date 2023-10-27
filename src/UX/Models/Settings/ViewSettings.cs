using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Seemon.Todo.Models.Settings;

public class ViewSettings : ObservableObject
{
    private bool _caseSensitive = false;
    private string _quickSearchString = string.Empty;
    private SortOptions _currentSort = SortOptions.None;
    private SortDirection _currentSortDirection = SortDirection.Ascending;
    private bool _allowGrouping = false;
    private bool _hideFutureTasks = false;
    private bool _showHiddenTasks = false;

    [JsonProperty("caseSensitive")]
    public bool CaseSensitive { get => _caseSensitive; set => SetProperty(ref _caseSensitive, value); }

    [JsonProperty("currentSort")]
    [JsonConverter(typeof(StringEnumConverter))]
    public SortOptions CurrentSort { get => _currentSort; set => SetProperty(ref _currentSort, value); }

    [JsonProperty("currentSortDirection")]
    [JsonConverter(typeof(StringEnumConverter))]
    public SortDirection CurrentSortDirection { get => _currentSortDirection; set => SetProperty(ref _currentSortDirection, value); }

    [JsonProperty("allowGroupin")]
    public bool AllowGrouping { get => _allowGrouping; set => SetProperty(ref _allowGrouping, value); }

    [JsonProperty("hideFutureTasks")]
    public bool HideFutureTasks { get => _hideFutureTasks; set => SetProperty(ref _hideFutureTasks, value); }

    [JsonProperty("showHiddenTasks")]
    public bool ShowHiddenTasks { get => _showHiddenTasks; set => SetProperty(ref _showHiddenTasks, value); }

    [JsonIgnore]
    public string QuickSearchString { get => _quickSearchString; set => SetProperty(ref _quickSearchString, value); }

    public static ViewSettings Default => new()
    {
        CaseSensitive = false,
        QuickSearchString = string.Empty,
        CurrentSort = SortOptions.None,
        CurrentSortDirection = SortDirection.Ascending,
        AllowGrouping = false
    };
}
