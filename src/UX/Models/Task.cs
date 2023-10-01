using CommunityToolkit.Mvvm.ComponentModel;

using Seemon.Todo.Helpers.Extensions;

namespace Seemon.Todo.Models;

public class Task : ObservableObject
{
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

    public bool IsHidden
    {
        get => _isCompleted; set => SetProperty(ref _isHidden, value);
    }

    public bool IsCompleted
    {
        get => _isCompleted; set => SetProperty(ref _isCompleted, value);
    }

    public string Priority
    {
        get => _priority; set => SetProperty(ref _priority, value);
    }

    public string CreatedDate
    {
        get => _createdDate; set => SetProperty(ref _createdDate, value);
    }

    public string DueDate
    {
        get => _dueDate; set => SetProperty(ref _dueDate, value);
    }

    public string ThresholdDate
    {
        get => _thresholdDate; set => SetProperty(ref _thresholdDate, value);
    }

    public string CompletedDate
    {
        get => _completedDate; set => SetProperty(ref _completedDate, value);
    }

    public string Body
    {
        get => _body; set => SetProperty(ref _body, value);
    }

    public string Raw
    {
        get => _raw; set => SetProperty(ref _raw, value);
    }

    public List<string> Projects
    {
        get => _projects; set => SetProperty(ref _projects, value);
    }

    public List<string> Contexts
    {
        get => _contexts; set => SetProperty(ref _contexts, value);
    }

    public IDictionary<string, string> Metadata
    {
        get => _metadata; set => SetProperty(ref _metadata, value);
    }

    public string GetFormattedRaw()
    {
        var completed = IsCompleted ? $"x " : string.Empty;
        var completedDate = IsCompleted && !string.IsNullOrEmpty(CreatedDate) ? $"{CompletedDate} " : string.Empty;
        var priority = !string.IsNullOrEmpty(Priority) ? $"({Priority}) " : string.Empty;
        var createdDate = !string.IsNullOrEmpty(CreatedDate) ? $"{CreatedDate} " : string.Empty;
        var body = !string.IsNullOrEmpty(Body) ? $"{Body}" : string.Empty;

        var items = from item in Projects select $"+{item}";
        var projects = string.Join(" ", items);

        items = from item in Contexts select $"@{item}";
        var contexts = string.Join(" ", items);

        items = from item in Metadata select $"{item.Key}:{item.Value}";
        var metadata = string.Join(" ", items);

        return $"{completed}{priority}{completedDate}{createdDate}{body}{projects} {contexts} {metadata}".TrimDoubleSpaces();
    }

    public override string ToString() => _raw;
}
