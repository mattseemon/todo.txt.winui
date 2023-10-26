using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Helpers.Views;
using Seemon.Todo.Models.Settings;

namespace Seemon.Todo.ViewModels.Pages;

public class MainViewModel : ViewModelBase, INavigationAware
{
    private readonly ITaskService _taskService;
    private readonly IRecentFilesService _recentFilesService;
    private readonly ISettingsService _settingsService;

    private readonly AppSettings _appSettings;
    private readonly ViewSettings _viewSettings;

    private CollectionViewSource _tasksCollectionView = new CollectionViewSource();
    private ObservableCollection<Models.Task> _filteredTasks = new ObservableCollection<Models.Task>();
    private ObservableCollection<GroupTaskList> _groupedTasks = new ObservableCollection<GroupTaskList>();

    private FontFamily _fontFamily = FontFamily.XamlAutoFontFamily;
    private double _fontSize;
    private Func<object, object>? _group = null;

    private ICommand? _selectionChangedCommand;
    private ICommand? _doubleTappedCommand;

    public ICommand SelectionChangedCommand => _selectionChangedCommand ??= RegisterCommand(OnSelectionChanged);
    public ICommand DoubleTappedCommand => _doubleTappedCommand ??= RegisterCommand(OnDoubleTapped);

    public CollectionViewSource TasksCollectionView { get => _tasksCollectionView; set => SetProperty(ref _tasksCollectionView, value); }

    public FontFamily Font { get => _fontFamily; set => SetProperty(ref _fontFamily, value); }

    public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }

    public MainViewModel(ITaskService taskService, IRecentFilesService recentFilesService, ISettingsService settingsService)
    {
        _taskService = taskService;
        _recentFilesService = recentFilesService;
        _settingsService = settingsService;

        _viewSettings = Task.Run(() => _settingsService.GetAsync(Constants.SETTING_VIEW, ViewSettings.Default)).Result;
        _viewSettings.PropertyChanged += OnViewSettingsPropertyChanged;

        _appSettings = Task.Run(() => _settingsService?.GetAsync(Constants.SETTING_APPLICATION, AppSettings.Default)).Result;
        _appSettings.PropertyChanged += OnAppSettingsPropertyChanged;

        _taskService.Loaded += OnTasksLoaded;
        _taskService.CollectionChanged += OnCollectionChanged;

        Font = string.IsNullOrEmpty(_appSettings.FontFamily) ? FontFamily.XamlAutoFontFamily : new FontFamily(_appSettings.FontFamily);
        FontSize = _appSettings.FontSize;

        UpdateCollectionView();

        if (_appSettings.OpenRecentOnStartup && !_taskService.IsLoaded)
        {
            var recent = _recentFilesService.RecentFiles.FirstOrDefault();
            if (recent != null)
            {
                if (File.Exists(recent.Path))
                {
                    _taskService.LoadTasks(recent.Path);
                }
                else
                {
                    _recentFilesService.RemoveAsync(recent.Path);
                }
            }
        }
    }

    private void OnAppSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppSettings.FontFamily))
        {
            Font = new FontFamily(_appSettings.FontFamily);
        }
        if (e.PropertyName == nameof(AppSettings.FontSize))
        {
            FontSize = _appSettings.FontSize;
        }
    }

    private void OnSelectionChanged()
        => App.GetService<ShellViewModel>().RaiseCommandCanExecute();

    private void OnDoubleTapped()
        => App.GetService<ShellViewModel>().UpdateTaskCommand?.Execute(null);

    private void OnViewSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewSettings.QuickSearchString))
        {
            UpdateCollectionView();
        }
    }

    private bool QuickSearch(object item)
    {
        if (string.IsNullOrEmpty(_viewSettings.QuickSearchString)) return true;

        var task = (Models.Task)item;
        var today = DateTime.Today;
        var stringComparison = _viewSettings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        return _viewSettings.QuickSearchString switch
        {
            "DONE" => task.IsCompleted,
            "-DONE" => !task.IsCompleted,
            "due:today" => task.DueDate.IsDateEqualTo(today),
            "-due:today" => !task.DueDate.IsDateEqualTo(today),
            "due:tomorrow" => task.DueDate.IsDateEqualTo(today.AddDays(1)),
            "due:past" => task.DueDate.IsDateLesserThan(today),
            "-due:past" => !task.DueDate.IsDateLesserThan(today),
            "due:future" => task.DueDate.IsDateGreaterThan(today),
            "-due:future" => !task.DueDate.IsDateGreaterThan(today),
            "due:active" => (!task.DueDate.IsNullOrEmpty() && !task.DueDate.IsDateGreaterThan(today)),
            "-due:active" => !(!task.DueDate.IsNullOrEmpty() && !task.DueDate.IsDateGreaterThan(today)),
            _ => task.Raw.Contains(_viewSettings.QuickSearchString, stringComparison),
        };
    }

    private void OnCollectionChanged(object? sender, EventArgs e) => UpdateCollectionView();

    private void OnTasksLoaded(object? sender, string e) => UpdateCollectionView();

    public void OnNavigatedTo(object parameter) => UpdateCollectionView();

    public void OnNavigatedFrom() { }

    public override bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args)
        => base.ShellKeyEventTriggered(args);

    public void UpdateCollectionView()
    {
        ApplyFilter();
        ApplyQuickSearch();
        _filteredTasks = SortTasks();
        if (_viewSettings.AllowGrouping && _viewSettings.CurrentSort != SortOptions.None && _viewSettings.CurrentSort != SortOptions.Alphabetical)
        {
            _groupedTasks = GroupTasks();
            TasksCollectionView.IsSourceGrouped = true;
            TasksCollectionView.Source = _groupedTasks;
        }
        else
        {
            TasksCollectionView.IsSourceGrouped = false;
            TasksCollectionView.Source = _filteredTasks;
        }
    }

    private List<string> GetGroupKeys(object item)
    {
        var grouping = _group?.Invoke(item);
        return grouping is List<string> list
            ? list.Count > 0 ? list : new List<string> { "[n/a]" }
            : string.IsNullOrEmpty(grouping as string) ? new List<string> { "[n/a]" } : new List<string> { (string)grouping };
    }

    private ObservableCollection<GroupTaskList> GroupTasks()
    {
        var groupedTasks = new List<GroupTaskList>();

        _group = (_viewSettings.CurrentSort) switch
        {
            SortOptions.Completed => x => ((Models.Task)x).CompletedDate,
            SortOptions.Priority => x => ((Models.Task)x).Priority,
            SortOptions.Context => x => ((Models.Task)x).Contexts,
            SortOptions.Project => x => ((Models.Task)x).Projects,
            SortOptions.Created => x => ((Models.Task)x).CreatedDate,
            SortOptions.Due => x => ((Models.Task)x).DueDate,
            SortOptions.Threshold => x => ((Models.Task)x).ThresholdDate,
            _ => x => (Models.Task)x
        };

        foreach (var task in _filteredTasks)
        {
            var keys = GetGroupKeys(task);
            foreach (var key in keys)
            {
                var group = groupedTasks.FirstOrDefault(x => Comparer.Default.Compare(x.Key, key) == 0);
                if (group == null)
                {
                    group = new GroupTaskList() { Key = key };
                    groupedTasks.Add(group);
                }
                group.Add(task);
            }
        }

        return _viewSettings.CurrentSortDirection == SortDirection.Ascending
            ? new ObservableCollection<GroupTaskList>(groupedTasks.OrderBy(g => g.Key?.ToString() == "[n/a]").ThenBy(g => g.Key))
            : new ObservableCollection<GroupTaskList>(groupedTasks.OrderBy(g => g.Key?.ToString() == "[n/a]").ThenByDescending(g => g.Key));
    }

    private ObservableCollection<Models.Task> SortTasks()
    {
        IOrderedEnumerable<Models.Task> sortedList;

        if (_viewSettings.CurrentSortDirection == SortDirection.Ascending)
        {
            sortedList = _viewSettings.CurrentSort switch
            {
                SortOptions.Alphabetical => _filteredTasks.OrderBy(t => t.Raw),
                SortOptions.Completed => _filteredTasks.OrderBy(t => t.IsCompleted)
                                                .ThenBy(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenBy(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                SortOptions.Priority => _filteredTasks.OrderBy(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenBy(t => t.IsCompleted)
                                                .ThenBy(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                SortOptions.Context => _filteredTasks.OrderBy(t => t.Contexts != null && t.Contexts.Count > 0 ? t.PrimaryContext : "ZZZ")
                                                .ThenBy(t => t.IsCompleted)
                                                .ThenBy(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenBy(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                SortOptions.Project => _filteredTasks.OrderBy(t => t.Projects != null && t.Projects.Count > 0 ? t.PrimaryProject : "ZZZ")
                                                .ThenBy(t => t.IsCompleted)
                                                .ThenBy(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenBy(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                SortOptions.Created => _filteredTasks.OrderBy(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate)
                                                .ThenBy(t => t.IsCompleted)
                                                .ThenBy(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenBy(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate),
                SortOptions.Due => _filteredTasks.OrderBy(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenBy(t => t.IsCompleted)
                                                .ThenBy(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenBy(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                SortOptions.Threshold => _filteredTasks.OrderBy(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenBy(t => t.IsCompleted)
                                                .ThenBy(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenBy(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenBy(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                _ => _filteredTasks.OrderBy(t => t),
            };
        }
        else
        {
            sortedList = _viewSettings.CurrentSort switch
            {
                SortOptions.Alphabetical => _filteredTasks.OrderByDescending(t => t.Raw),
                SortOptions.Completed => _filteredTasks.OrderByDescending(t => t.IsCompleted)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                SortOptions.Priority => _filteredTasks.OrderByDescending(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenByDescending(t => t.IsCompleted)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                SortOptions.Context => _filteredTasks.OrderByDescending(t => t.Contexts != null && t.Contexts.Count > 0 ? t.PrimaryContext : "ZZZ")
                                                .ThenByDescending(t => t.IsCompleted)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                SortOptions.Project => _filteredTasks.OrderByDescending(t => t.Projects != null && t.Projects.Count > 0 ? t.PrimaryProject : "ZZZ")
                                                .ThenByDescending(t => t.IsCompleted)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                SortOptions.Created => _filteredTasks.OrderByDescending(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate)
                                                .ThenByDescending(t => t.IsCompleted)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate),
                SortOptions.Due => _filteredTasks.OrderByDescending(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenByDescending(t => t.IsCompleted)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                SortOptions.Threshold => _filteredTasks.OrderByDescending(t => string.IsNullOrEmpty(t.ThresholdDate) ? "9999-99-99" : t.ThresholdDate)
                                                .ThenByDescending(t => t.IsCompleted)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.Priority) ? "ZZZ" : t.Priority)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.DueDate) ? "9999-99-99" : t.DueDate)
                                                .ThenByDescending(t => string.IsNullOrEmpty(t.CreatedDate) ? "0000-00-00" : t.CreatedDate),
                _ => _filteredTasks.OrderByDescending(_t => _t),
            };
        }

        return new ObservableCollection<Models.Task>(sortedList);
    }

    private void ApplyFilter() => _filteredTasks = new ObservableCollection<Models.Task>(_taskService.ActiveTasks);

    private void ApplyQuickSearch()
    {
        var filtered = _filteredTasks.Where(t => QuickSearch(t));
        RemoveNonMatching(filtered);
        AddBackMatching(filtered);
    }

    private void RemoveNonMatching(IEnumerable<Models.Task> filteredData)
    {
        for (int i = _filteredTasks.Count - 1; i >= 0; i--)
        {
            var task = _filteredTasks[i];

            if (!filteredData.Contains(task))
            {
                _filteredTasks.Remove(task);
            }
        }
    }

    private void AddBackMatching(IEnumerable<Models.Task> filteredData)
    {
        foreach (var task in filteredData)
        {
            if (!_filteredTasks.Contains(task))
            {
                _filteredTasks.Add(task);
            }
        }
    }
}
