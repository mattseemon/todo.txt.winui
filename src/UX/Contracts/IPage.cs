using Seemon.Todo.Contracts.ViewModels;

namespace Seemon.Todo.Contracts;

public interface IPage
{
    IViewModel ViewModel
    {
        get; set;
    }
}
