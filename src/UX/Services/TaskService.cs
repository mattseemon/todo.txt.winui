using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using CommunityToolkit.Mvvm.ComponentModel;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Exceptions;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.Services;
using Seemon.Todo.Models.Settings;

using Task = Seemon.Todo.Models.Task;

namespace Seemon.Todo.Services;

public class TaskService : ObservableObject, ITaskService
{
    public event EventHandler<string>? Loaded;
    public event EventHandler<NotifyCollectionChangedEventArgs>? ActiveTasksCollectionChanged;

    private readonly ILocalSettingsService _localSettingsService;

    private TodoSettings _todoSettings;

    private ObservableCollection<Task> _activeTasks;

    private const string ARCHIVE_FILENAME = "done.txt";

    private string _todoPath = string.Empty;

    private bool _isLoaded = false;

    public TaskService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;

        _activeTasks = new ObservableCollection<Task>();
        _activeTasks.CollectionChanged += OnActiveTasksCollectionChanged;
        _todoSettings = System.Threading.Tasks.Task.Run(() => _localSettingsService.ReadSettingAsync<TodoSettings>(Constants.SETTING_TODO)).Result ?? TodoSettings.Default;

        IsLoaded = false;
    }

    public bool IsLoaded
    {
        get => _isLoaded; set => SetProperty(ref _isLoaded, value);
    }

    public ObservableCollection<Task> ActiveTasks => _activeTasks;

    private void OnActiveTasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ActiveTasksCollectionChanged?.Invoke(this, e);
    }

    public void LoadTasks(string path)
    {
        // Load todo file
        _todoPath = path;
        if (!File.Exists(_todoPath))
        {
            throw new TaskException($"Could not load todo file: {_todoPath}");
        }

        _activeTasks ??= new ObservableCollection<Task>();
        _activeTasks.Clear();

        _activeTasks.CollectionChanged -= OnActiveTasksCollectionChanged;
        var lines = FileHelper.ReadLinesFromFile(path);
        foreach (var line in lines)
        {
            _activeTasks.Add(Parse(line));
        }
        _activeTasks.CollectionChanged += OnActiveTasksCollectionChanged;

        IsLoaded = true;
        Loaded?.Invoke(this, path);
    }

    public void DeleteTask(Task task)
    {
        if (_activeTasks == null) return;

        try
        {
            if (_activeTasks.Remove(task))
            {
                SaveActiveTasks();
            }
        }
        catch (IOException ex)
        {
            var message = "Could not remove task from todo.txt file due to an unexpected error. Please see details.";
            throw new TaskException(message, ex);
        }
        catch (Exception) { throw; }
    }

    public void ArchiveCompletedTasks()
    {
        if (_activeTasks == null) return;
        var tasks = _activeTasks.ToList();

        var archivePath = GetArchivePath();
        if (string.IsNullOrEmpty(archivePath)) return;

        var archivedTasks = FileHelper.ReadLinesFromFile(archivePath);
        foreach (var task in tasks)
        {
            if (task.IsCompleted)
            {
                archivedTasks.Add(task.Raw);
                DeleteTask(task);
            }
        }
        FileHelper.WriteLinesToFile(archivePath, archivedTasks);
    }

    public Task Parse(string raw)
    {
        Task task = new();

        raw = raw.Replace(Environment.NewLine, string.Empty);

        raw = ParseDate(raw, Constants.REGEX_TODO_RELATIVE_DUE_DATE);
        raw = ParseDate(raw, Constants.REGEX_TODO_RELATIVE_THRESHOLD_DATE);

        task.Raw = raw;

        var regex = new Regex(Constants.REGEX_TODO_COMPLETED);
        var match = regex.Match(raw);

        if (match.Success)
        {
            var temp = match.Value.Trim();
            task.IsCompleted = true;
            if (temp.Length > 1)
            {
                task.CompletedDate = temp[2..];
            }
            raw = regex.Replace(raw, string.Empty);
        }

        regex = new Regex(Constants.REGEX_TODO_METADATA, RegexOptions.IgnoreCase);
        var matches = regex.Matches(raw);

        task.Metadata.Clear();
        if (matches.Count > 0)
        {
            foreach (var metadata in from Match item in matches
                                     let metadata = item.Value.Split(':')
                                     select metadata)
            {
                task.Metadata.Add(metadata[0], metadata[1]);
            }

            raw = regex.Replace(raw, string.Empty);
        }

        regex = new Regex(Constants.REGEX_TODO_PRIORITY, RegexOptions.IgnoreCase);
        match = regex.Match(raw);
        if (match.Success)
        {
            var temp = match.Value.Trim();
            task.Priority = temp.Substring(1, 1);
            raw = regex.Replace(raw, string.Empty);
        }

        regex = new Regex(Constants.REGEX_TODO_DATE, RegexOptions.IgnoreCase);
        match = regex.Match(raw);
        if (match.Success)
        {
            task.CreatedDate = match.Value.Trim();
            raw = regex.Replace(raw, string.Empty);
        }

        regex = new Regex(Constants.REGEX_TODO_PROECT, RegexOptions.IgnoreCase);
        matches = regex.Matches(raw);

        task.Projects.Clear();
        if (matches.Count > 0)
        {
            task.Projects.AddRange(from Match item in matches
                                   let project = item.Value.Trim()
                                   select project[1..]);
            raw = regex.Replace(raw, string.Empty);
        }

        regex = new Regex(Constants.REGEX_TODO_CONTEXT, RegexOptions.IgnoreCase);
        matches = regex.Matches(raw);

        task.Contexts.Clear();
        if (matches.Count > 0)
        {
            task.Contexts.AddRange(from Match item in matches
                                   let context = item.Value.Trim()
                                   select context[1..]);
            raw = regex.Replace(raw, string.Empty);
        }

        if (task.Metadata.Count > 0)
        {
            task.DueDate = task.Metadata.ContainsKey("due") ? task.Metadata["due"] : string.Empty;
            task.ThresholdDate = task.Metadata.ContainsKey("t") ? task.Metadata["t"] : string.Empty;
            task.IsHidden = task.Metadata.ContainsKey("h") && Convert.ToBoolean(Convert.ToInt32(task.Metadata["h"]));
        }

        task.Body = raw.Trim();

        return task;
    }

    private static string ParseDate(string text, string pattern)
    {
        // Replacing relative dates with actual date formatted to todo.txt format specifications;
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        var match = regex.Match(text);

        if (match.Success)
        {
            var date = DateTime.Today;
            var isValid = false;

            var relativeDate = match.Groups["relative"].Value.Trim();

            if (!string.IsNullOrEmpty(relativeDate))
            {
                relativeDate = relativeDate.ToLower();

                switch (relativeDate)
                {
                    case "tomorrow":
                        date = date.AddDays(1);
                        isValid = true;
                        break;
                    case "today":
                        isValid = true;
                        break;
                }

                if (match.Groups["weekday"].Success)
                {
                    var shortDay = relativeDate[..3];

                    for (var i = 0; i < 7; i++)
                    {
                        date = date.AddDays(i);
                        isValid = string.Equals(date.ToString("ddd"), shortDay, StringComparison.OrdinalIgnoreCase);
                        if (isValid) break;
                    }
                }
            }

            if (isValid)
            {
                var replace = pattern == Constants.REGEX_TODO_RELATIVE_DUE_DATE ? $"due:{date.ToTodoDate()}" : $"t:{date.ToTodoDate()}";
                text = text.Replace(text, replace);
            }
        }
        return text;
    }

    private void SaveActiveTasks()
    {
        try
        {
            if (_activeTasks != null)
            {
                var tasks = _activeTasks.Select(t => t.Raw).ToList();
                FileHelper.WriteLinesToFile(_todoPath, tasks);
            }

        }
        catch { throw; }
    }

    private string GetArchivePath()
    {
        _todoSettings = System.Threading.Tasks.Task.Run(() => _localSettingsService.ReadSettingAsync<TodoSettings>(Constants.SETTING_TODO)).Result ?? TodoSettings.Default;
        if (_todoSettings.EnableGlobalArchive && File.Exists(_todoSettings.GlobalArchiveFilePath))
        {
            return _todoSettings.GlobalArchiveFilePath;
        }
        else
        {
            var root = Path.GetDirectoryName(_todoPath);
            return (string.IsNullOrEmpty(root)) ? string.Empty : Path.Combine(root, ARCHIVE_FILENAME);
        }
    }
}
