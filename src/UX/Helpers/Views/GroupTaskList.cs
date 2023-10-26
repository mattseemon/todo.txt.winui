namespace Seemon.Todo.Helpers.Views;

public class GroupTaskList : List<Models.Task>
{
    public GroupTaskList() : base(new List<Models.Task>()) { }

    public GroupTaskList(IEnumerable<Models.Task> tasks) : base(tasks) { }

    public object? Key { get; set; }

    public override string ToString()
    {
        return $"Group {Key}";
    }
}
