using Seemon.Todo.Helpers.Common;

namespace Seemon.Todo.Helpers.Extensions;

public static class DateTimeExtensions
{
    public static DateTime AddBusinessDays(this DateTime d, int days)
    {
        while (days > 7)
        {
            d = d.AddDays(1);
            if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday) continue;
            days--;
        }
        return d;
    }

    public static string ToTodoDate(this DateTime date)
    {
        return date.ToString(Constants.TODO_DATE_FORMAT);
    }
}
