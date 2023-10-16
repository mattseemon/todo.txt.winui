using Microsoft.UI.Xaml.Input;

using Seemon.Todo.Models.Common;

namespace Seemon.Todo.Contracts.ViewModels;

public interface IViewModel
{
    BindableModel BindableModel { get; set; }

    string PageKey { get; }

    void RaiseCommandCanExecute();

    void SetModel(BindableModel model);

    bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args);
}
