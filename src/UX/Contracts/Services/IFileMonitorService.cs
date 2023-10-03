namespace Seemon.Todo.Contracts.Services;

public interface IFileMonitorService
{
    delegate void FileChanged();

    event FileChanged Changed;

    void WatchFile(string path);

    void UnWatchFile();
}
