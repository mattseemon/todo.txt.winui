using System.Collections.ObjectModel;

using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.Contracts.Services;

public interface IRecentFilesService
{
    ObservableCollection<RecentFile> RecentFiles { get; }

    void Add(string path);

    void Remove(string path);

    void Clear();
}