using Seemon.Todo.Helpers.Common;

namespace Seemon.Todo.Helpers.Extensions;

public static class DateTimeExtensions
{
    public static string ToTodoDate(this DateTime date)
    {
        return date.ToString(Constants.TODO_DATE_FORMAT);
    }
}
