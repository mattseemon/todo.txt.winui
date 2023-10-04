namespace Seemon.Todo.Helpers.Extensions;

public static class StringExtensions
{
    public const string SINGLE_SPACE = " ";
    public const string DOUBLE_SPACE = "  ";

    public static string TrimDoubleSpaces(this string text)
    {
        while (text.Contains(DOUBLE_SPACE))
        {
            text = text.Replace(DOUBLE_SPACE, SINGLE_SPACE);
        }
        return text.Trim();
    }

    public static bool IsDateEqualTo(this string dateText, DateTime date)
    {
        if (string.IsNullOrEmpty(dateText)) return false;

        if (!DateTime.TryParse(dateText, out DateTime comparison)) return false;

        return comparison.Date == date.Date;
    }

    public static bool IsDateGreaterThan(this string dateText, DateTime date)
    {
        if (string.IsNullOrEmpty(dateText)) return false;

        if (!DateTime.TryParse(dateText, out DateTime comparison)) return false;

        return comparison.Date > date.Date;
    }

    public static bool IsDateLesserThan(this string dateText, DateTime date)
    {
        if (string.IsNullOrEmpty(dateText)) return false;

        if (!DateTime.TryParse(dateText, out DateTime comparison)) return false;

        return comparison.Date < date.Date;
    }
}
