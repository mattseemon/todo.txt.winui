namespace Seemon.Todo.Contracts.Services;

public interface ISystemService
{
    Task<string> OpenFileDialogAsync();

    Task<string> OpenSaveDialogAsync(string filename = "todo");
}
