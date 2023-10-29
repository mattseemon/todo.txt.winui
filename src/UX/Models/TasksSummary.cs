using CommunityToolkit.Mvvm.ComponentModel;

namespace Seemon.Todo.Models;

public class TasksSummary : ObservableObject
{
    private int _totalCount = 0;
    private int _filteredCount = 0;
    private int _incomplete = 0;
    private int _dueToday = 0;
    private int _overdue = 0;
    private int _hidden = 0;

    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int FilteredCount { get => _filteredCount; set => SetProperty(ref _filteredCount, value); }
    public int Incomplete { get => _incomplete; set => SetProperty(ref _incomplete, value); }
    public int DueToday { get => _dueToday; set => SetProperty(ref _dueToday, value); }
    public int Overdue { get => _overdue; set => SetProperty(ref _overdue, value); }
    public int Hidden { get => _hidden; set => SetProperty(ref _hidden, value); }
}
