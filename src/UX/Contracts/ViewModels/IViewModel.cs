namespace Seemon.Todo.Contracts.ViewModels;

public interface IViewModel
{
    string PageKey { get; }

    void RaiseCommandCanExecute();

    void SetModel(object model);
}
