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

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace Seemon.Todo.ViewModels.Pages;

public class MainViewModel : ViewModelBase, INavigationAware
{
    private readonly ITaskService _taskService;
    private readonly IRecentFilesService _recentFilesService;
    private readonly ISettingsService _settingsService;
    private readonly ITaskbarIconService _taskbarIconService;

    private readonly AppSettings _appSettings;
    private readonly ViewSettings _viewSettings;
    private readonly FilterSettings _filterSettings;

    private CollectionViewSource _tasksCollectionView = new();
    private ObservableCollection<Models.Task> _filteredTasks = new();
    private ObservableCollection<GroupTaskList> _groupedTasks = new();

    private FontFamily _fontFamily = FontFamily.XamlAutoFontFamily;
    private double _fontSize;
    private Func<object, object>? _group = null;
    private string _currentFilter = string.Empty;
    private bool _isTaskListFocused;
    private Models.TasksSummary _tasksSummary;

    private ICommand? _selectionChangedCommand;
    private ICommand? _doubleTappedCommand;

    private ICommand? _clearActiveFilterCommand;
    private ICommand? _clearAllFiltersCommand;
    private ICommand? _closeFilterCommand;

    public ICommand SelectionChangedCommand => _selectionChangedCommand ??= RegisterCommand(OnSelectionChanged);
    public ICommand DoubleTappedCommand => _doubleTappedCommand ??= RegisterCommand(OnDoubleTapped);
    public ICommand ClearActiveFilterCommand => _clearActiveFilterCommand ??= RegisterCommand(OnClearActiveFilter);
    public ICommand ClearAllFiltersCommand => _clearAllFiltersCommand ??= RegisterCommand(OnClearAllFilters);
    public ICommand CloseFilterCommand => _closeFilterCommand ??= RegisterCommand(OnCloseFilter);

    public CollectionViewSource TasksCollectionView { get => _tasksCollectionView; set => SetProperty(ref _tasksCollectionView, value); }

    public FontFamily Font { get => _fontFamily; set => SetProperty(ref _fontFamily, value); }

    public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }

    public bool IsTaskListFocused { get => _isTaskListFocused; set => SetProperty(ref _isTaskListFocused, value); }

    public AppSettings AppSettings => _appSettings;
    public ViewSettings ViewSettings => _viewSettings;
    public FilterSettings FilterSettings => _filterSettings;
    public Models.TasksSummary Summary => _tasksSummary;

    public MainViewModel(ITaskService taskService, IRecentFilesService recentFilesService, ISettingsService settingsService, ITaskbarIconService taskbarIconService)
    {
        _taskService = taskService;
        _recentFilesService = recentFilesService;
        _settingsService = settingsService;
        _taskbarIconService = taskbarIconService;

        _viewSettings = Task.Run(() => _settingsService.GetAsync(Constants.SETTING_VIEW, ViewSettings.Default)).Result;
        _viewSettings.PropertyChanged += OnViewSettingsPropertyChanged;

        _appSettings = Task.Run(() => _settingsService?.GetAsync(Constants.SETTING_APPLICATION, AppSettings.Default)).Result;
        _appSettings.PropertyChanged += OnAppSettingsPropertyChanged;

        _filterSettings = Task.Run(() => _settingsService?.GetAsync(Constants.SETTING_FILTER, FilterSettings.Default)).Result;
        _filterSettings.PropertyChanged += OnFilterSettingsPropertyChanged;

        _taskService.Loaded += OnTasksLoaded;
        _taskService.CollectionChanged += OnCollectionChanged;

        _tasksSummary ??= new Models.TasksSummary();

        Font = string.IsNullOrEmpty(_appSettings.FontFamily) ? FontFamily.XamlAutoFontFamily : new FontFamily(_appSettings.FontFamily);
        FontSize = _appSettings.FontSize;
        _currentFilter = _filterSettings.ActiveFilter;

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
        if (e.PropertyName == nameof(AppSettings.ShowInNotificationArea))
        {
            if (_appSettings.ShowInNotificationArea)
            {
                _taskbarIconService.Initialize();
            }
            else
            {
                _taskbarIconService.Destroy();
            }
            App.HandleClosedEvents = _appSettings.ShowInNotificationArea && _appSettings.CloseToNotificationArea;
        }
        if (e.PropertyName == nameof(AppSettings.CloseToNotificationArea))
        {
            App.HandleClosedEvents = _appSettings.ShowInNotificationArea && _appSettings.CloseToNotificationArea;
        }
    }

    private void OnViewSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewSettings.QuickSearchString))
        {
            UpdateCollectionView();
        }
    }

    private void OnFilterSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_filterSettings.ActiveFilter))
        {
            if (_currentFilter != _filterSettings.ActiveFilter)
            {
                _currentFilter = _filterSettings.ActiveFilter;
                UpdateCollectionView();
            }
        }
    }

    private void OnSelectionChanged() => App.GetService<ShellViewModel>().RaiseCommandCanExecute();

    private void OnDoubleTapped() => App.GetService<ShellViewModel>().UpdateTaskCommand?.Execute(null);

    private void OnClearActiveFilter() => _filterSettings.ActiveFilter = string.Empty;

    private void OnClearAllFilters()
    {
        _filterSettings.PresetFilter1 = string.Empty;
        _filterSettings.PresetFilter2 = string.Empty;
        _filterSettings.PresetFilter3 = string.Empty;
        _filterSettings.PresetFilter4 = string.Empty;
        _filterSettings.PresetFilter5 = string.Empty;
        _filterSettings.PresetFilter6 = string.Empty;
        _filterSettings.PresetFilter7 = string.Empty;
        _filterSettings.PresetFilter8 = string.Empty;
        _filterSettings.PresetFilter9 = string.Empty;
        _filterSettings.ActiveFilter = string.Empty;
    }

    private void OnCloseFilter() => _filterSettings.IsFiltersVisible = false;

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

    public void OnNavigatedFrom() => App.GetService<ShellViewModel>().RaiseCommandCanExecute();

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

        UpdateSummary();

        IsTaskListFocused = true;
    }

    private void UpdateSummary()
    {
        if (_taskService.ActiveTasks == null) return;

        _tasksSummary ??= new Models.TasksSummary();

        _tasksSummary.TotalCount = _taskService.ActiveTasks.Count;
        _tasksSummary.FilteredCount = _filteredTasks.Count;

        int incomplete = 0, dueToday = 0, overdue = 0, hidden = 0;

        hidden = _taskService.ActiveTasks.Where(t => t.IsHidden).Count();

        foreach (var task in _filteredTasks)
        {
            if (!task.IsCompleted)
            {
                incomplete++;
                if (!string.IsNullOrEmpty(task.DueDate))
                {
                    if (DateTime.TryParseExact(task.DueDate, Constants.TODO_DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate))
                    {
                        if (dueDate.Date == DateTime.Today.Date) dueToday++;
                        else if (dueDate.Date < DateTime.Today.Date) overdue++;
                    }
                }
            }
        }
        _tasksSummary.DueToday = dueToday;
        _tasksSummary.Incomplete = incomplete;
        _tasksSummary.Overdue = overdue;
        _tasksSummary.Hidden = _viewSettings.ShowHiddenTasks ? 0 : hidden;
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

    private void ApplyFilter()
    {
        var filters = _filterSettings.ActiveFilter.FixNewLines();
        var comparer = _viewSettings.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

        var filtered = new List<Models.Task>();

        foreach (var task in _taskService.ActiveTasks)
        {
            bool include = true;

            if (!_viewSettings.ShowHiddenTasks)
            {
                include = !task.IsHidden;
            }

            if (include)
            {
                if (_viewSettings.HideFutureTasks)
                {
                    include = string.IsNullOrEmpty(task.ThresholdDate) || task.ThresholdDate.IsDateLesserThan(DateTime.Today.AddDays(1));
                }
            }

            if (include)
            {
                foreach (var filter in filters.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (filter.Equals("due:today", StringComparison.OrdinalIgnoreCase) && task.DueDate.Equals(DateTime.Today.ToTodoDate())) continue;
                    if (filter.Equals("due:future", StringComparison.OrdinalIgnoreCase) && task.DueDate.IsDateGreaterThan(DateTime.Today)) continue;
                    if (filter.Equals("due:past", StringComparison.OrdinalIgnoreCase) && task.DueDate.IsDateLesserThan(DateTime.Today)) continue;
                    if (filter.Equals("due:active", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(task.DueDate) && !task.DueDate.IsDateGreaterThan(DateTime.Today)) continue;

                    if (filter.Equals("-due:today", StringComparison.OrdinalIgnoreCase) && task.DueDate.Equals(DateTime.Today.ToTodoDate()))
                    {
                        include = false;
                        continue;
                    }
                    if (filter.Equals("-due:future", StringComparison.OrdinalIgnoreCase) && task.DueDate.IsDateGreaterThan(DateTime.Today))
                    {
                        include = false;
                        continue;
                    }
                    if (filter.Equals("-due:past", StringComparison.OrdinalIgnoreCase) && task.DueDate.IsDateLesserThan(DateTime.Today))
                    {
                        include = false;
                        continue;
                    }
                    if (filter.Equals("-due:active", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(task.DueDate) && !task.DueDate.IsDateGreaterThan(DateTime.Today))
                    {
                        include = false;
                        continue;
                    }

                    if (filter.Equals("-DONE", StringComparison.Ordinal))
                    {
                        if (task.IsCompleted) include = false;
                    }
                    else if (filter.Equals("DONE", StringComparison.Ordinal))
                    {
                        if (!task.IsCompleted) include = false;
                    }
                    else
                    {
                        if (filter[..1].Equals("-", StringComparison.Ordinal))
                        {
                            if (task.Raw.Contains(filter[1..], comparer)) include = false;

                        }
                        else if (!task.Raw.Contains(filter, comparer)) include = false;
                    }
                }
            }

            if (include)
            {
                filtered.Add(task);
            }
        }

        _filteredTasks = new ObservableCollection<Models.Task>(filtered);
    }

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
