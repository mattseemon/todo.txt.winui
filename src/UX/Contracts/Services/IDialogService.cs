namespace Seemon.Todo.Contracts.Services;

public interface IDialogService
{
    Task ShowMessageAsync(string title, string message);

    Task ShowFeatureNotImpletmented(string feature);
}
