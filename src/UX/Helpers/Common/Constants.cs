namespace Seemon.Todo.Helpers.Common;

public static class Constants
{
    public const string TODO_DATE_FORMAT = "yyyy-MM-dd";

    public const string REGEX_TODO_COMPLETED = @"^x\s((\d{4})-(\d{2})-(\d{2}))?";
    public const string REGEX_TODO_PRIORITY = @"^(\([A-Z]\)\s)";
    public const string REGEX_TODO_DATE = @"((\d{4})-(\d{2})-(\d{2}))";
    public const string REGEX_TODO_DUE_DATE = @$"\bdue:{REGEX_TODO_DATE}\b";
    public const string REGEX_TODO_THRESHOLD_DATE = $@"\bt:{REGEX_TODO_DATE}\b";
    public const string REGEX_TODO_CREATED_DATE = @"^((\d{4})-(\d{2})-(\d{2}))|\s((\d{4})-(\d{2})-(\d{2}))";
    public const string REGEX_TODO_PROECT = @"(\+[^\s]+)";
    public const string REGEX_TODO_CONTEXT = @"(^|\s)(\@[^\s]+)";
    public const string REGEX_TODO_METADATA = @"(\w+):\s*(?:""([^""]*)""|(\S+))";
    public const string REGEX_TODO_HIDDEN = @"\bh:[0/1]\b";

    public const string REGEX_TODO_RELATIVE_DATE = @"(?<relative>today|tomorrow|(?<weekday>mon(?:day)?|tue(?:sday)?|wed(?:nesday)?|thu(?:rsday)?|fri(?:day)?|sat(?:urday)?|sun(?:day)?))";
    public const string REGEX_TODO_RELATIVE_DUE_DATE = @"\bdue:" + REGEX_TODO_RELATIVE_DATE + @"\b";
    public const string REGEX_TODO_RELATIVE_THRESHOLD_DATE = @"t:" + REGEX_TODO_RELATIVE_DATE;
    public const string REGEX_TODO_METADATA_PRIORITY = @"\bpri:[A-Z]\b";

    public const string SETTING_APPLICATION = "application.settings";
    public const string SETTING_TODO = "todo.settings";
    public const string SETTING_VIEW = "view.settings";
}
