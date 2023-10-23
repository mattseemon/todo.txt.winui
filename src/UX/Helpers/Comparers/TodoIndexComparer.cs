using System.Collections;

using Seemon.Todo.Contracts.Services;

using Task = Seemon.Todo.Models.Task;

namespace Seemon.Todo.Helpers.Comparers;

public class TodoIndexComparer : IComparer
{
    public int Compare(object? x, object? y)
    {
        var objectX = x as Task;
        var objectY = y as Task;

        if (objectX == null && objectY == null) return 0;
        if (objectX == null) return -1;
        if (objectY == null) return 1;

        int indexX = App.GetService<ITaskService>().ActiveTasks.IndexOf(objectX);
        int indexY = App.GetService<ITaskService>().ActiveTasks.IndexOf(objectY);

        return indexX.CompareTo(indexY);
    }
}
