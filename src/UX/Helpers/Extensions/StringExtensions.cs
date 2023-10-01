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
}
