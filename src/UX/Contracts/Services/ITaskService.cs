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
    
    void DeleteTask(Task task);

    void ArchiveCompletedTasks();

    Task Parse(string raw);
}
