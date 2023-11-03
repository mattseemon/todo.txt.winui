using WinUIEx;

namespace Seemon.Todo.Contracts.Services;

public interface IWindowManagerService
{
    WindowEx MainWindow { get; }

    void RestoreWindowSettings();

    void SaveWindowSettings();
}
