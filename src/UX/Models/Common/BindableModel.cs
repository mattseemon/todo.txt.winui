using CommunityToolkit.Mvvm.ComponentModel;

namespace Seemon.Todo.Models.Common;

public class BindableModel : ObservableObject
{
    private string _bindableString = string.Empty;
    private DateTimeOffset? _bindableDateTime = null;
    private int _bindableInt = 0;

    public string BindableString { get => _bindableString; set => SetProperty(ref _bindableString, value); }

    public DateTimeOffset? BindableDateTime { get => _bindableDateTime; set => SetProperty(ref _bindableDateTime, value); }

    public int BindableInt { get => _bindableInt; set => SetProperty(ref _bindableInt, value); }
}
