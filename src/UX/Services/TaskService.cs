using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using CommunityToolkit.Mvvm.ComponentModel;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Exceptions;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Models.Common;
using Seemon.Todo.Models.Settings;

using Task = Seemon.Todo.Models.Task;

namespace Seemon.Todo.Services;

public class TaskService : ObservableObject, ITaskService
{
    public event EventHandler<string>? Loaded;
    public event EventHandler? CollectionChanged;

    private readonly ILocalSettingsService? _localSettingsService;
    private readonly IFileMonitorService? _fileMonitorService;
    private readonly IRecentFilesService? _recentFilesService;

    private TodoSettings _todoSettings;
    private readonly AppSettings _appSettings;

    private ObservableCollection<Task> _activeTasks;

    public IList<Task> SelectedTasks => _activeTasks.Where(t => t.IsSelected).ToList();

    private const string ARCHIVE_FILENAME = "done.txt";

    private string _todoPath = string.Empty;

    private bool _isLoaded = false;

    public TaskService(ILocalSettingsService localSettingsService, IFileMonitorService fileMonitorService, IRecentFilesService recentFilesService)
    {
        _localSettingsService = localSettingsService;

        _fileMonitorService = fileMonitorService;
        _fileMonitorService.Changed += OnFileMonitorServiceChanged;

        _recentFilesService = recentFilesService;

        _activeTasks = new ObservableCollection<Task>();
        _activeTasks.CollectionChanged += OnActiveTasksCollectionChanged;
        _todoSettings = System.Threading.Tasks.Task.Run(() => _localSettingsService.ReadSettingAsync(Constants.SETTING_TODO, TodoSettings.Default)).Result;
        _appSettings = System.Threading.Tasks.Task.Run(() => _localSettingsService.ReadSettingAsync(Constants.SETTING_APPLICATION, AppSettings.Default)).Result;
        _appSettings.PropertyChanged += OnAppSettingsPropertyChanged;

        IsLoaded = false;
    }

    private void OnAppSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_appSettings.AutoRefreshFile)
        {
            _fileMonitorService?.WatchFile(_todoPath);
        }
        else
        {
            _fileMonitorService?.UnWatchFile();
        }
    }

    private void OnFileMonitorServiceChanged() => ReloadTasks();

    public bool IsLoaded
    {
        get => _isLoaded; set => SetProperty(ref _isLoaded, value);
    }

    public ObservableCollection<Task> ActiveTasks => _activeTasks;

    private void OnActiveTasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

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
        var lines = ReadLinesFromFile(path);
        foreach (var line in lines)
        {
            _activeTasks.Add(Parse(line));
        }
        _activeTasks.CollectionChanged += OnActiveTasksCollectionChanged;

        IsLoaded = true;
        Loaded?.Invoke(this, path);
        _fileMonitorService?.WatchFile(_todoPath);
        _recentFilesService?.Add(path);
    }

    public void ReloadTasks() => LoadTasks(_todoPath);

    public void AddTask(string raw)
    {
        if (_activeTasks == null) return;

        raw = raw.Trim();
        if (raw.Length == 0) return;

        try
        {
            var tempTask = Parse(raw);
            if (_todoSettings.AddCreatedDate)
            {
                var today = DateTime.Now.ToTodoDate();
                if (string.IsNullOrEmpty(tempTask.CreatedDate))
                {
                    raw = string.IsNullOrEmpty(tempTask.Priority) ? $"{today} {raw}" : raw.Insert(3, $" {today}");
                }
            }

            if ((_todoSettings.DefaultPriority != "None") && string.IsNullOrEmpty(tempTask.Priority))
            {
                raw = $"({_todoSettings.DefaultPriority.ToUpper()}) {raw}";
            }

            raw = raw.TrimDoubleSpaces();

            var task = Parse(raw);
            _activeTasks.Add(task);

            SaveActiveTasks();
            task.IsSelected = true;
        }
        catch (IOException ex)
        {
            var message = "Could not add task to todo.txt file due to an unexpected error. Please see details.";
            throw new TaskException(message, ex);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void UpdateTask(Task current, string raw)
    {
        if (_activeTasks == null) return;

        try
        {
            raw = raw.TrimDoubleSpaces();

            var updatedTask = Parse(raw);

            var index = _activeTasks.IndexOf(_activeTasks.First(t => t == current));
            if (index < 0)
            {
                throw new TaskException("Task no longer exists in the todo.txt file.");
            }

            _activeTasks[index] = updatedTask;
            SaveActiveTasks();

            updatedTask.IsSelected = true;
        }
        catch (IOException ex)
        {
            var message = "Could not update task to todo.txt file due to an unexpected error. Please see details.";
            throw new TaskException(message, ex);
        }
        catch (Exception)
        {
            throw;
        }
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

    public void ToggleCompleted(Task task)
    {
        if (_activeTasks == null) return;

        try
        {
            var raw = task.Raw;
            var completed = false;

            var regex = new Regex(Constants.REGEX_TODO_COMPLETED);

            if (regex.IsMatch(raw))
            {
                raw = regex.Replace(raw, string.Empty).Trim();
                regex = new Regex(Constants.REGEX_TODO_METADATA_PRIORITY);
                if (regex.IsMatch(raw))
                {
                    var priority = regex.Match(raw).Value.Replace("pri:", string.Empty);
                    raw = regex.Replace(raw, string.Empty);
                    raw = $"({priority}) {raw}";
                }
            }
            else
            {
                regex = new Regex(Constants.REGEX_TODO_PRIORITY);
                if (regex.IsMatch(raw))
                {
                    var priority = regex.Match(raw).Value.Substring(1, 1);
                    raw = regex.Replace(raw, string.Empty).Trim();
                    raw = $"{raw.Trim()} pri:{priority}";
                }

                var completedDate = DateTime.Today.ToTodoDate();
                regex = new Regex(Constants.REGEX_TODO_CREATED_DATE);
                raw = regex.IsMatch(raw) ? $"x {completedDate} {raw}" : $"x {raw}";
                completed = true;
            }
            UpdateTask(task, raw);

            if (_todoSettings.ArchiveCompleted && _todoSettings.AutoArchive)
                ArchiveCompletedTasks();

            regex = new Regex(Constants.REGEX_TODO_METADATA);
            var recurrance = string.Empty;
            foreach (Match match in regex.Matches(raw).Cast<Match>())
            {
                if (match.Value.StartsWith("rec")) recurrance = match.Value;
            }

            if (completed && !string.IsNullOrEmpty(recurrance))
            {
                recurrance = recurrance.Replace("rec:", string.Empty);

                var strict = false;
                if (recurrance.StartsWith("+"))
                {
                    strict = true;
                    recurrance = recurrance[1..];
                }

                var temp = recurrance[..^1];
                recurrance = recurrance.Replace(temp, string.Empty);
                var frequency = Convert.ToInt32(recurrance);

                var dueDate = DateTime.Now;

                var startDate = !strict ? DateTime.Today : string.IsNullOrEmpty(task.DueDate) ? DateTime.Today : DateTime.Parse(task.DueDate);
                switch (recurrance)
                {
                    case "d":
                        dueDate = startDate.AddDays(frequency);
                        break;
                    case "b":
                        dueDate = startDate.AddBusinessDays(frequency);
                        break;
                    case "w":
                        dueDate = startDate.AddDays(frequency * 7);
                        break;
                    case "m":
                        dueDate = startDate.AddMonths(frequency);
                        break;
                    case "y":
                        dueDate = startDate.AddYears(frequency);
                        break;
                }

                raw = task.Raw;

                regex = new Regex(Constants.REGEX_TODO_CREATED_DATE);
                if (regex.IsMatch(raw)) raw = regex.Replace(raw, $" {DateTime.Today.ToTodoDate()}");

                regex = new Regex(Constants.REGEX_TODO_DUE_DATE);
                if (regex.IsMatch(raw)) raw = regex.Replace(raw, $"due:{dueDate.ToTodoDate()}");

                AddTask(raw);
            }
        }
        catch (IOException ex)
        {
            var message = "Could not toggle task completion in todo.txt file due to an unexpected error. Please see details.";
            throw new TaskException(message, ex);
        }
        catch (Exception) { throw; }
    }

    public void ToggleHidden(Task task)
    {
        if (_activeTasks == null) return;

        try
        {
            var raw = task.Raw;

            var regex = new Regex(Constants.REGEX_TODO_HIDDEN);
            if (regex.IsMatch(raw))
            {
                raw = regex.Match(raw).Value == "h:0" ? regex.Replace(raw, "h:1") : regex.Replace(raw, string.Empty);
            }
            else
            {
                raw = $"{raw} h:1";
            }
            UpdateTask(task, raw);
        }
        catch (IOException ex)
        {
            var message = "Could not toggle task hidden in todo.txt file due to an unexpected error. Please see details.";
            throw new TaskException(message, ex);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void ArchiveCompletedTasks()
    {
        if (_activeTasks == null) return;
        var tasks = _activeTasks.ToList();

        var archivePath = GetArchivePath();
        if (string.IsNullOrEmpty(archivePath)) return;

        var archivedTasks = ReadLinesFromFile(archivePath);
        foreach (var task in tasks)
        {
            if (task.IsCompleted)
            {
                archivedTasks.Add(task.Raw);
                DeleteTask(task);
            }
        }
        WriteLinesToFile(archivePath, archivedTasks);
    }

    public void SetPriority(Task task, string priority)
    {
        if (_activeTasks == null) return;

        if (task.IsCompleted) return;

        try
        {
            var raw = task.Raw;
            Regex regex = new(Constants.REGEX_TODO_PRIORITY);

            raw = string.IsNullOrEmpty(priority)
                ? regex.IsMatch(raw) ? regex.Replace(raw, string.Empty) : raw
                : regex.IsMatch(raw) ? regex.Replace(raw, $"({priority.ToUpper()}) ") : $"({priority.ToUpper()}) {raw}";

            UpdateTask(task, raw);
        }
        catch (IOException ex)
        {
            var message = "Could not set priority for the task due to any unexpected error. Please see details.";
            throw new TaskException(message, ex);
        }
        catch { throw; }
    }

    public void SetDate(Task task, string date, DateTypes type)
    {
        if (_activeTasks == null) return;

        try
        {
            var raw = task.Raw;
            var pattern = type == DateTypes.Due ? Constants.REGEX_TODO_DUE_DATE : Constants.REGEX_TODO_THRESHOLD_DATE;
            var formattedDate = type == DateTypes.Due ? $"due:{date}" : $"t:{date}";

            var regex = new Regex(pattern);

            raw = string.IsNullOrEmpty(date)
                ? regex.IsMatch(raw) ? regex.Replace(raw, string.Empty) : raw
                : regex.IsMatch(raw) ? regex.Replace(raw, formattedDate) : $"{raw} {formattedDate}";

            UpdateTask(task, raw);
        }
        catch (IOException ex)
        {
            var dateType = type == DateTypes.Due ? "due" : "threshold";
            var message = $"Could not set {dateType} date for selected task due to an unexpected error. Please see details.";
            throw new TaskException(message, ex);
        }
        catch (Exception) { throw; }
    }

    public void PostponeDate(Task task, string postpone, DateTypes type)
    {
        if (_activeTasks == null) return;

        try
        {
            var raw = task.Raw;

            // set default postpone date to today, in case of issues
            DateTime? postponeDate = DateTime.Today;

            // if specific date is choosen, set postpone date to that date
            var regex = new Regex(Constants.REGEX_TODO_DATE);

            if (regex.IsMatch(postpone))
            {
                postponeDate = Convert.ToDateTime(postpone);
            }
            else
            {
                // calculate postponed date based on exist date in task else today
                var startDateString = type == DateTypes.Due ? task.DueDate : task.ThresholdDate;
                var startDate = string.IsNullOrEmpty(startDateString) ? postponeDate : Convert.ToDateTime(startDateString);

                postponeDate = GetPostponeDate(postpone, startDate);
            }

            if (postponeDate.HasValue)
            {
                SetDate(task, postponeDate.Value.ToTodoDate(), type);
            }
        }
        catch (IOException ex)
        {
            var dateType = type == DateTypes.Due ? "due" : "threshold";
            var message = $"Could not postpone the {dateType} date for selected task due to an unexpected error. Please see details.";
            throw new TaskException(message, ex);
        }
        catch (Exception) { throw; }
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
        }

        regex = new Regex(Constants.REGEX_TODO_CONTEXT, RegexOptions.IgnoreCase);
        matches = regex.Matches(raw);

        task.Contexts.Clear();
        if (matches.Count > 0)
        {
            task.Contexts.AddRange(from Match item in matches
                                   let context = item.Value.Trim()
                                   select context[1..]);
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
                        isValid = string.Equals(date.ToString("ddd"), shortDay, StringComparison.CurrentCultureIgnoreCase);
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

    private DateTime? GetPostponeDate(string postpone, DateTime? startDate = null)
    {
        postpone = postpone.Trim().ToLower();

        // check if postpone date string is in relative date format i.e. today, tomorrow, monday..sunday
        // assume start date for relative dates is always today
        var regex = new Regex(Constants.REGEX_TODO_RELATIVE_DATE, RegexOptions.IgnoreCase);
        if (regex.IsMatch(postpone))
        {
            bool isValid;
            var count = 0;

            switch (postpone)
            {
                case "today":
                    return DateTime.Today;
                case "tomorrow":
                    return DateTime.Today.AddDays(1);
                default:
                    var postponeDate = DateTime.Today;
                    var shortCode = postpone[..3];
                    do
                    {
                        count++;
                        postponeDate = postponeDate.AddDays(1);
                        isValid = string.Equals(postponeDate.ToString("ddd"), shortCode, StringComparison.CurrentCultureIgnoreCase);
                    } while (!isValid && count < 7);
                    return postponeDate;
            }
        }

        // if not relative date then it must be # of days. anything else is ignored.
        if (postpone.Length > 0)
        {
            try
            {
                if (startDate == null) startDate = DateTime.Today;
                return startDate.Value.AddDays(Convert.ToInt32(postpone));
            }
            catch { }
        }

        // return null if all else fails
        return null;
    }

    private void SaveActiveTasks()
    {
        try
        {
            if (_activeTasks != null)
            {
                var tasks = _activeTasks.Select(t => t.Raw).ToList();
                WriteLinesToFile(_todoPath, tasks);
            }

        }
        catch { throw; }
    }

    private string GetArchivePath()
    {
        _todoSettings = System.Threading.Tasks.Task.Run(() => _localSettingsService?.ReadSettingAsync(Constants.SETTING_TODO, TodoSettings.Default)).Result;
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

    private IList<string> ReadLinesFromFile(string path)
    {
        var lines = new List<string>();

        if (File.Exists(path))
        {
            try
            {
                var stream = File.OpenRead(path);

                using var reader = new StreamReader(stream);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(line.Trim());
                    }
                }
            }
            catch (IOException ex) { throw new TaskException("An error occurred while trying to read the todo file.", ex); }
            catch (Exception) { throw; }
        }
        return lines;
    }

    private void WriteLinesToFile(string path, IList<string> lines)
    {
        if (_todoPath == path) _fileMonitorService?.UnWatchFile();
        try
        {
            using StreamWriter writer = new(path);
            foreach (var task in lines)
            {
                writer.WriteLine(task);
            }
            writer.Close();
        }
        catch (IOException ex)
        {
            var message = "An erorr occurred when trying to write to the todo.txt file.";
            throw new TaskException(message, ex);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            _fileMonitorService?.WatchFile(_todoPath);
        }
    }
}
