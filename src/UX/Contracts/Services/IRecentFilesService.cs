using System.Collections.ObjectModel;

using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.Contracts.Services;

public interface IRecentFilesService
{
    ObservableCollection<RecentFile> RecentFiles { get; }

    void AddAsync(string path);

    void RemoveAsync(string path);

    void ClearAsync();

    void SortAndTrimRecents();
}