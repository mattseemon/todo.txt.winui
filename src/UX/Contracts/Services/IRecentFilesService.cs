using Seemon.Todo.Models.Settings;

using System.Collections.ObjectModel;

namespace Seemon.Todo.Contracts.Services;

public interface IRecentFilesService
{
    ObservableCollection<RecentFile> RecentFiles { get; }

    void AddAsync(string path);

    void RemoveAsync(string path);

    void ClearAsync();

    void SortAndTrimRecents();
}