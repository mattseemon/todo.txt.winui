using System.Collections;

namespace Seemon.Todo.Helpers.Comparers;

public class TodoStringComparer : IComparer
{
    private string _text = "ZZZ";

    public int Compare(object? x, object? y)
    {
        var stringX = x as string;
        var stringY = y as string;

        if (string.IsNullOrEmpty(stringX)) stringX = _text;
        if (string.IsNullOrEmpty(stringY)) stringY = _text;

        return stringX.CompareTo(stringY);
    }
}
