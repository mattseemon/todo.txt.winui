using System.ComponentModel;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Input;

using Seemon.Todo.Contracts.ViewModels;

namespace Seemon.Todo.Helpers.ViewModels;

public class ViewModelBase : ObservableValidator, IViewModel
{
    private readonly IList<ICommand> _commands;

    public ViewModelBase() => _commands = new List<ICommand>();

    public string PageKey => GetType().FullName;

    public IList<ICommand> Commands => _commands;

    public ICommand RegisterCommand<T>(Action<T> execute, Predicate<T>? canExecute = null)
    {
        var command = canExecute == null ? new RelayCommand<T>(execute) : new RelayCommand<T>(execute, canExecute);
        _commands.Add(command);
        return command;
    }

    public ICommand RegisterCommand(Action execute, Func<bool>? canExecute = null)
    {
        var command = canExecute == null ? new RelayCommand(execute) : new RelayCommand(execute, canExecute);
        _commands.Add(command);
        return command;
    }

    public void RaiseCommandCanExecute()
    {
        foreach (var command in _commands.Cast<IRelayCommand>())
        {
            command.NotifyCanExecuteChanged();
        }
    }

    public virtual void SetModel(object model)
    {
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        RaiseCommandCanExecute();
    }

    public virtual bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args)
    {
        return false;
    }
}
