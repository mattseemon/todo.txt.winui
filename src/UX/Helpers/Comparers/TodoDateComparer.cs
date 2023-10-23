using System.Collections;

namespace Seemon.Todo.Helpers.Comparers;

public class TodoDateComparer : IComparer
{
    private DateTime _date;

    public TodoDateComparer(DateTime date)
    {
        _date = date;
    }

    public int Compare(object? x, object? y)
    {
        var dateX = string.IsNullOrEmpty(x as string) ? _date : Convert.ToDateTime(x as string);
        var dateY = string.IsNullOrEmpty(y as string) ? _date : Convert.ToDateTime(y as string);
        return dateX.CompareTo(dateY);
    }
}
