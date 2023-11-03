using Seemon.Todo.Contracts.Services;

using WinUIEx;

namespace Seemon.Todo.Services;

public class WindowManagerService : IWindowManagerService
{
    public WindowEx MainWindow => App.MainWindow;

    public void RestoreWindowSettings()
    {
        throw new NotImplementedException();
    }

    public void SaveWindowSettings()
    {

    }
}
