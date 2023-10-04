namespace Seemon.Todo.Helpers.Extensions;


public static class IEnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
    {
        return items == null || !items.Any();
    }
}
