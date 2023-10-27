using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Seemon.Todo.Models.Settings;

public class FilterSettings : ObservableObject
{
    private bool _isFiltersVisible = false;
    private PresetFilters _currentFilterPreset = PresetFilters.None;
    private string _activeFilter = string.Empty;
    private string _presetFilter1 = string.Empty;
    private string _presetFilter2 = string.Empty;
    private string _presetFilter3 = string.Empty;
    private string _presetFilter4 = string.Empty;
    private string _presetFilter5 = string.Empty;
    private string _presetFilter6 = string.Empty;
    private string _presetFilter7 = string.Empty;
    private string _presetFilter8 = string.Empty;
    private string _presetFilter9 = string.Empty;

    [JsonProperty("currentPresetFilter")]
    [JsonConverter(typeof(StringEnumConverter))]
    public PresetFilters CurrentPresetFilter { get => _currentFilterPreset; set => SetProperty(ref _currentFilterPreset, value); }

    [JsonProperty("activeFilter")]
    public string ActiveFilter
    {
        get => _activeFilter;
        set
        {
            SetProperty(ref _activeFilter, value);
            if (string.IsNullOrEmpty(value)) CurrentPresetFilter = PresetFilters.None;
        }
    }

    [JsonProperty("presetFilter1")]
    public string PresetFilter1 { get => _presetFilter1; set => SetProperty(ref _presetFilter1, value); }

    [JsonProperty("presetFilter2")]
    public string PresetFilter2 { get => _presetFilter2; set => SetProperty(ref _presetFilter2, value); }

    [JsonProperty("presetFilter3")]
    public string PresetFilter3 { get => _presetFilter3; set => SetProperty(ref _presetFilter3, value); }

    [JsonProperty("presetFilter4")]
    public string PresetFilter4 { get => _presetFilter4; set => SetProperty(ref _presetFilter4, value); }

    [JsonProperty("presetFilter5")]
    public string PresetFilter5 { get => _presetFilter5; set => SetProperty(ref _presetFilter5, value); }

    [JsonProperty("presetFilter6")]
    public string PresetFilter6 { get => _presetFilter6; set => SetProperty(ref _presetFilter6, value); }

    [JsonProperty("presetFilter7")]
    public string PresetFilter7 { get => _presetFilter7; set => SetProperty(ref _presetFilter7, value); }

    [JsonProperty("presetFilter8")]
    public string PresetFilter8 { get => _presetFilter8; set => SetProperty(ref _presetFilter8, value); }

    [JsonProperty("presetFilter9")]
    public string PresetFilter9 { get => _presetFilter9; set => SetProperty(ref _presetFilter9, value); }

    [JsonIgnore]
    public bool IsFiltersVisible { get => _isFiltersVisible; set => SetProperty(ref _isFiltersVisible, value); }

    public static FilterSettings Default => new()
    {
        CurrentPresetFilter = PresetFilters.None,
        ActiveFilter = string.Empty,
        PresetFilter1 = string.Empty,
        PresetFilter2 = string.Empty,
        PresetFilter3 = string.Empty,
        PresetFilter4 = string.Empty,
        PresetFilter5 = string.Empty,
        PresetFilter6 = string.Empty,
        PresetFilter7 = string.Empty,
        PresetFilter8 = string.Empty,
        PresetFilter9 = string.Empty,
    };
}
