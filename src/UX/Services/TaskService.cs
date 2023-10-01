using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Exceptions;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Task = Seemon.Todo.Models.Task;

namespace Seemon.Todo.Services;

public class TaskService : ObservableObject, ITaskService
{
    public event EventHandler<string> Loaded;
    public event EventHandler<NotifyCollectionChangedEventArgs> ActiveTasksCollectionChanged;

    private ObservableCollection<Task> _activeTasks;

    private string _todoPath = string.Empty;

    private bool _isLoaded = false;

    public TaskService()
    {
        _activeTasks = new ObservableCollection<Task>();
        _activeTasks.CollectionChanged += OnActiveTasksCollectionChanged;
        IsLoaded = false;
    }

    public bool IsLoaded
    {
        get => _isLoaded; set => SetProperty(ref _isLoaded, value);
    }

    public ObservableCollection<Task> ActiveTasks => _activeTasks;

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

    private void OnActiveTasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ActiveTasksCollectionChanged?.Invoke(this, e);
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
}
