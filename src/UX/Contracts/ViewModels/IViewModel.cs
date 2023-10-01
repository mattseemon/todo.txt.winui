using Microsoft.UI.Xaml.Input;

namespace Seemon.Todo.Contracts.ViewModels;

public interface IViewModel
{
    string PageKey
    {
        get;
    }

    void RaiseCommandCanExecute();

    void SetModel(object model);

    bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args);
}
