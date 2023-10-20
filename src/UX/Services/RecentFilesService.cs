using System.Collections.ObjectModel;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.Services;

public class RecentFilesService : IRecentFilesService
{
    private readonly ISettingsService _settingsService;
    private readonly AppSettings _settings;

    public ObservableCollection<RecentFile> RecentFiles => _settings.RecentFiles;

    public RecentFilesService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _settings = Task.Run(() => _settingsService.GetAsync(Constants.SETTING_APPLICATION, AppSettings.Default)).Result;
    }

    public async void Add(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        var recent = _settings.RecentFiles.FirstOrDefault(r => r.Path == path);
        if (recent == null)
        {
            _settings.RecentFiles.Add(new() { Path = path, LastAccessed = DateTime.Now });
        }
        else
        {
            recent.LastAccessed = DateTime.Now;
        }
        SortAndTrimRecents();
        await _settingsService.SetAsync(Constants.SETTING_APPLICATION, _settings);
    }

    public async void Remove(string path)
    {
        var recent = _settings.RecentFiles.FirstOrDefault(r => r.Path == path);
        if (recent == null) return;

        _settings.RecentFiles.Remove(recent);
        await _settingsService.SetAsync(Constants.SETTING_APPLICATION, _settings);
    }

    public async void Clear()
    {
        _settings.RecentFiles.Clear();
        await _settingsService.SetAsync(Constants.SETTING_APPLICATION, _settings);
    }

    private void SortAndTrimRecents()
    {
        var recents = _settings.RecentFiles.OrderByDescending(r => r.LastAccessed).ToList();

        var count = 0;
        _settings.RecentFiles.Clear();

        foreach (var recent in recents)
        {
            if (!File.Exists(recent.Path)) continue;

            _settings.RecentFiles.Add(recent);
            count++;

            if (count == _settings.MaxRecentFileCount) break;
        }
    }
}
