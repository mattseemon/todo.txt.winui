using System.ComponentModel;

namespace Seemon.Todo.Models.Settings;

public enum SortOptions
{
    [Description("Order in File")]
    None = 0,
    [Description("Alphabetical")]
    Alphabetical = 1,
    [Description("Completed Date")]
    Completed = 2,
    [Description("Priorities")]
    Priority = 3,
    [Description("Contexts")]
    Context = 4,
    [Description("Projects")]
    Project = 5,
    [Description("Created Date")]
    Created = 6,
    [Description("Due Date")]
    Due = 7,
    [Description("Threshold Date")]
    Threshold = 8,
}
