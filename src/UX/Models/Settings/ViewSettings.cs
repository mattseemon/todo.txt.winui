using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

namespace Seemon.Todo.Models.Settings;

public class ViewSettings : ObservableObject
{
    private bool _caseSensitive = false;
    private string _quickSearchString = string.Empty;

    [JsonProperty("caseSensitive")]
    public bool CaseSensitive
    {
        get => _caseSensitive; set => SetProperty(ref _caseSensitive, value);
    }

    [JsonIgnore]
    public string QuickSearchString
    {
        get => _quickSearchString; set => SetProperty(ref _quickSearchString, value);
    }

    public static ViewSettings Default = new()
    {
        CaseSensitive = false,
        QuickSearchString = string.Empty,
    };
}
