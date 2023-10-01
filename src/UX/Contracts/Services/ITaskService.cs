using System.Collections.ObjectModel;

using Task = Seemon.Todo.Models.Task;

namespace Seemon.Todo.Contracts.Services;

public interface ITaskService
{
    event EventHandler<string> Loaded;

    public bool IsLoaded
    {
        get; set;
    }

    ObservableCollection<Task> ActiveTasks
    {
        get;
    }

    void LoadTasks(string path);

    Task Parse(string raw);
}
