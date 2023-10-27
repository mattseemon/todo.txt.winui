using CommunityToolkit.Mvvm.ComponentModel;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.Extensions;

namespace Seemon.Todo.Models;

public class Task : ObservableObject, IComparable<Task>
{
    private bool _isSelected = false;

    private bool _isHidden = false;
    private bool _isCompleted = false;
    private string _priority = string.Empty;
    private string _createdDate = string.Empty;
    private string _dueDate = string.Empty;
    private string _thresholdDate = string.Empty;
    private string _completedDate = string.Empty;
    private string _body = string.Empty;
    private string _raw = string.Empty;
    private List<string> _projects = new();
    private List<string> _contexts = new();
    private IDictionary<string, string> _metadata = new Dictionary<string, string>();

    /// <summary>
    /// Used only for view. not part of todo.txt format spec.
    /// </summary>
    public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }

    public bool IsHidden { get => _isHidden; set => SetProperty(ref _isHidden, value); }

    public bool IsCompleted { get => _isCompleted; set => SetProperty(ref _isCompleted, value); }

    public string Priority { get => _priority; set => SetProperty(ref _priority, value); }

    public string CreatedDate { get => _createdDate; set => SetProperty(ref _createdDate, value); }

    public string DueDate { get => _dueDate; set => SetProperty(ref _dueDate, value); }

    public string ThresholdDate { get => _thresholdDate; set => SetProperty(ref _thresholdDate, value); }

    public string CompletedDate { get => (IsCompleted && string.IsNullOrEmpty(_completedDate)) ? DateTime.Today.ToTodoDate() : _completedDate; set => SetProperty(ref _completedDate, value); }

    public string Body { get => _body; set => SetProperty(ref _body, value); }

    public string Raw { get => _raw; set => SetProperty(ref _raw, value); }

    public List<string> Projects { get => _projects; set => SetProperty(ref _projects, value); }

    public string PrimaryProject => _projects != null && _projects.Count > 0 ? _projects[0] : string.Empty;

    public List<string> Contexts { get => _contexts; set => SetProperty(ref _contexts, value); }

    public string PrimaryContext => _contexts != null && _contexts.Count > 0 ? _contexts[0] : string.Empty;

    public IDictionary<string, string> Metadata { get => _metadata; set => SetProperty(ref _metadata, value); }

    public int CompareTo(Task? other)
    {
        if (other == null) return -1;

        int indexX = App.GetService<ITaskService>().ActiveTasks.IndexOf(this);
        int indexY = App.GetService<ITaskService>().ActiveTasks.IndexOf(other);

        return indexX.CompareTo(indexY);
    }

    public string GetFormattedRaw()
    {
        var completed = IsCompleted ? $"x " : string.Empty;
        var completedDate = IsCompleted && !string.IsNullOrEmpty(CreatedDate) ? $"{CompletedDate} " : string.Empty;
        var priority = !string.IsNullOrEmpty(Priority) ? $"({Priority}) " : string.Empty;
        var createdDate = !string.IsNullOrEmpty(CreatedDate) ? $"{CreatedDate} " : string.Empty;
        var body = !string.IsNullOrEmpty(Body) ? $"{Body}" : string.Empty;

        var items = from item in Metadata select $"{item.Key}:{item.Value}";
        var metadata = string.Join(" ", items);

        return $"{completed}{priority}{completedDate}{createdDate}{body} {metadata}".TrimDoubleSpaces();
    }

    public override string ToString() => _raw;
}
