using System.Collections.ObjectModel;

using Task = Seemon.Todo.Models.Task;

namespace Seemon.Todo.Contracts.Services;

public interface ITaskService
{
    event EventHandler<string> Loaded;
    event EventHandler CollectionChanged;

    public bool IsLoaded
    {
        get; set;
    }

    ObservableCollection<Task> ActiveTasks
    {
        get;
    }

    IList<Task> SelectedTasks { get; }

    void LoadTasks(string path);

    void ReloadTasks();

    void AddTask(string raw);

    void UpdateTask(Task current, string raw);

    void DeleteTask(Task task);
    
    void ToggleCompleted(Task task);
    
    void ArchiveCompletedTasks();

    Task Parse(string raw);
}
