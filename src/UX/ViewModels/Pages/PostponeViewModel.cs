using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Models.Common;

namespace Seemon.Todo.ViewModels.Pages;

public class PostponeViewModel : ViewModelBase
{
    private DateTimeOffset _dateTimeValue;

    public DateTimeOffset DateTimeValue
    {
        get => _dateTimeValue;
        set
        {
            SetProperty(ref _dateTimeValue, value);
            BindableModel.BindableString = value.Date.ToTodoDate();
        }
    }
}
