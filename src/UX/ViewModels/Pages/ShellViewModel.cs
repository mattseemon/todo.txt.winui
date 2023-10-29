using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Models.Common;
using Seemon.Todo.Models.Settings;
using Seemon.Todo.Views.Pages;

using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace Seemon.Todo.ViewModels.Pages;

public class ShellViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly ISystemService _systemService;
    private readonly ITaskService _taskService;
    private readonly IRecentFilesService _recentFilesService;
    private readonly ISettingsService _settingsService;

    private readonly AppSettings _appSettings;
    private readonly ViewSettings _viewSettings;
    private readonly TodoSettings _todoSettings;
    private readonly FilterSettings _filterSettings;

    private bool _isBackEnabled;
    private bool _isMenuVisible;
    private bool _isRecentEnabled;
    private bool _isQuickSearchFocused;

    private ICommand? _goBackCommand;

    private ICommand? _showSettingsCommand;
    private ICommand? _showAboutCommand;
    private ICommand? _quickSearchFocusedChangedCommand;

    private ICommand? _newTodoCommand;
    private ICommand? _openTodoCommand;
    private ICommand? _reloadTodoFileCommand;
    private ICommand? _archiveCompletedTasksCommand;
    private ICommand? _openRecentCommand;
    private ICommand? _clearRecentCommand;
    private ICommand? _applicationExitCommand;

    private ICommand? _copyToClipboardCommand;
    private ICommand? _pasteFromClipboardCommad;

    private ICommand? _addNewTaskCommand;
    private ICommand? _addMultipleNewTaskCommand;
    private ICommand? _copyToNewTaskCommand;
    private ICommand? _updateTaskCommand;
    private ICommand? _appendTextToTaskCommand;
    private ICommand? _deleteTaskCommand;
    private ICommand? _toggleCompletedCommand;
    private ICommand? _toggleHiddenCommand;
    private ICommand? _setPriorityCommand;
    private ICommand? _clearPriorityCommand;
    private ICommand? _increasePriorityCommand;
    private ICommand? _decreasePriorityCommand;
    private ICommand? _setDateCommand;
    private ICommand? _postponeDateCommand;
    private ICommand? _increaseDateCommand;
    private ICommand? _decreaseDateCommand;
    private ICommand? _clearDateCommand;

    private ICommand? _sortOptionCommand;
    private ICommand? _sortDirectionCommand;
    private ICommand? _showInGroupsCommand;

    private ICommand? _hideFutureTasksCommand;
    private ICommand? _showHiddentTasksCommand;
    private ICommand? _showFiltersCommannd;
    private ICommand? _applyPresetFilterCommand;

    private ICommand? _featureNotImplementedCommand;

    public bool IsBackEnabled { get => _isBackEnabled; set => SetProperty(ref _isBackEnabled, value); }

    public bool IsMenuVisible { get => _isMenuVisible; set => SetProperty(ref _isMenuVisible, value); }

    public bool IsRecentEnabled { get => _isRecentEnabled; set => SetProperty(ref _isRecentEnabled, value); }

    public bool IsQuickSearchFocused { get => _isQuickSearchFocused; set => SetProperty(ref _isQuickSearchFocused, value); }

    public INavigationService NavigationService { get; }

    public ViewSettings ViewSettings => _viewSettings;

    public AppSettings AppSettings => _appSettings;

    public FilterSettings FilterSettings => _filterSettings;

    public ObservableCollection<RecentFile> RecentFiles => _recentFilesService.RecentFiles;

    public ICommand GoBackCommand => _goBackCommand ??= RegisterCommand(OnGoBack);

    public ICommand ShowSettingsCommand => _showSettingsCommand ??= RegisterCommand(OnShowSettings);
    public ICommand ShowAboutCommand => _showAboutCommand ??= RegisterCommand(OnShowAbout);
    public ICommand QuickSearchFocusedChangedCommand => _quickSearchFocusedChangedCommand ??= RegisterCommand<string>(OnQuickSearchFocusedChanged);

    public ICommand NewTodoCommand => _newTodoCommand ??= RegisterCommand(OnNewTodo);
    public ICommand OpenTodoCommad => _openTodoCommand ??= RegisterCommand(OnOpenTodo);
    public ICommand ReloadTodoFileCommand => _reloadTodoFileCommand ??= RegisterCommand(OnReloadTodoFile, CanReloadTodoFile);
    public ICommand ArchiveCompletedTasksCommand => _archiveCompletedTasksCommand ??= RegisterCommand(OnArchiveCompletedTasks, CanArchiveCompletedTasks);
    public ICommand OpenRecentCommand => _openRecentCommand ??= RegisterCommand<string>(OnOpenRecent);
    public ICommand ClearRecentCommand => _clearRecentCommand ??= RegisterCommand(OnClearRecent, CanFileClearRecent);
    public ICommand ApplicationExitCommand => _applicationExitCommand ??= RegisterCommand(OnApplicationExit);

    public ICommand CopyToClipboardCommand => _copyToClipboardCommand ??= RegisterCommand(OnCopyToClipboard, CanCopyToClipboard);
    public ICommand PasteFromClipboardCommad => _pasteFromClipboardCommad ??= RegisterCommand(OnPasteFromClipboard, CanPasteFromClipboard);

    public ICommand AddNewTaskCommand => _addNewTaskCommand ??= RegisterCommand(OnAddNewTask, CanAddNewTasks);
    public ICommand AddMultipleNewTasksCommand => _addMultipleNewTaskCommand ??= RegisterCommand(OnAddMultipleNewTasks, CanAddNewTasks);
    public ICommand CopyToNewTaskCommand => _copyToNewTaskCommand ??= RegisterCommand(OnCopyToNewTask, CanCopyToNewTask);
    public ICommand AppendTextToTaskCommand => _appendTextToTaskCommand ??= RegisterCommand(OnAppendTextToTask, CanAppendTextToTask);
    public ICommand UpdateTaskCommand => _updateTaskCommand ??= RegisterCommand(OnUpdateTask, CanUpdateTask);
    public ICommand DeleteTaskCommand => _deleteTaskCommand ??= RegisterCommand(OnDeleteTask, CanDeleteTask);
    public ICommand ToggleCompletedCommand => _toggleCompletedCommand ??= RegisterCommand(OnToggleCompleted, CanToggleCompleted);
    public ICommand ToggleHiddenCommand => _toggleHiddenCommand ??= RegisterCommand(OnToggleHidden, CanToggleHidden);
    public ICommand SetPriorityCommand => _setPriorityCommand ??= RegisterCommand(OnSetPriority, CanSetPriority);
    public ICommand ClearPriorityCommand => _clearPriorityCommand ??= RegisterCommand(OnClearPriority, CanClearPriority);
    public ICommand IncreasePriorityCommand => _increasePriorityCommand ??= RegisterCommand(OnIncreasePriority, CanIncreasePriority);
    public ICommand DecreasePriorityCommand => _decreasePriorityCommand ??= RegisterCommand(OnDecreasePriority, CanDecreasePriority);
    public ICommand SetDateCommand => _setDateCommand ??= RegisterCommand<string>(OnSetDate, CanSetDate);
    public ICommand PostponeDateCommand => _postponeDateCommand ??= RegisterCommand<string>(OnPostponeDate, CanPostponeDate);
    public ICommand IncreaseDateCommad => _increaseDateCommand ??= RegisterCommand<string>(OnIncreaseDate, CanIncreaseDate);
    public ICommand DecreaseDateCommad => _decreaseDateCommand ??= RegisterCommand<string>(OnDecreaseDate, CanDecreaseDate);
    public ICommand ClearDateCommand => _clearDateCommand ??= RegisterCommand<string>(OnClearDate, CanClearDate);

    public ICommand SortOptionCommand => _sortOptionCommand ??= RegisterCommand<string>(OnSortOption);
    public ICommand SortDirectionCommand => _sortDirectionCommand ??= RegisterCommand<string>(OnSortDirection);
    public ICommand ShowInGroupsCommand => _showInGroupsCommand ??= RegisterCommand(OnShowInGroups);

    public ICommand HideFutureTasksCommand => _hideFutureTasksCommand ??= RegisterCommand(OnHideFutureTasks);
    public ICommand ShowHiddenTasksCommand => _showHiddentTasksCommand ??= RegisterCommand(OnShowHiddenTasks);
    public ICommand ShowFiltersCommand => _showFiltersCommannd ??= RegisterCommand(OnShowFilters);
    public ICommand ApplyPresetFilterCommand => _applyPresetFilterCommand ??= RegisterCommand<string>(OnApplyPresetFilter);

    public ICommand FeatureNotImplementedCommand => _featureNotImplementedCommand ??= RegisterCommand<string>(OnFeatureNotImplemented);

    public ShellViewModel(INavigationService navigationService, IDialogService dialogService, ISystemService systemService, ITaskService taskService, IRecentFilesService recentFilesService, ISettingsService settingsService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;

        _dialogService = dialogService;
        _systemService = systemService;

        _taskService = taskService;
        _taskService.ActiveTasks.CollectionChanged += OnActiveTasksCollectionChanged;

        _recentFilesService = recentFilesService;
        _recentFilesService.RecentFiles.CollectionChanged += OnRecentFilesCollectionChanged;

        _settingsService = settingsService;
        _appSettings = Task.Run(() => _settingsService.GetAsync(Constants.SETTING_APPLICATION, AppSettings.Default)).Result;
        _viewSettings = Task.Run(() => _settingsService.GetAsync(Constants.SETTING_VIEW, ViewSettings.Default)).Result;
        _todoSettings = Task.Run(() => _settingsService.GetAsync(Constants.SETTING_TODO, TodoSettings.Default)).Result;
        _filterSettings = Task.Run(() => _settingsService.GetAsync(Constants.SETTING_FILTER, FilterSettings.Default)).Result;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;
        IsMenuVisible = NavigationService.Frame?.Content.GetType() == typeof(MainPage);
        IsRecentEnabled = _recentFilesService.RecentFiles.Count > 0;
    }

    private void OnGoBack() => NavigationService.GoBack();

    private void OnShowSettings() => NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);

    private void OnShowAbout() => NavigationService.NavigateTo(typeof(AboutViewModel).FullName!);

    private void OnQuickSearchFocusedChanged(string? value)
    {
        if (value == null) return;

        IsQuickSearchFocused = bool.Parse(value);
    }

    private async void OnNewTodo()
    {
        var path = await _systemService.OpenSaveDialogAsync();
        if (string.IsNullOrEmpty(path)) return;

        if (!File.Exists(path))
        {
            using var stream = File.Create(path);
        }

        OpenTodo(path);
    }

    private async void OnOpenTodo()
    {
        var path = await _systemService.OpenFileDialogAsync();
        if (string.IsNullOrEmpty(path)) return;

        OpenTodo(path);
    }

    private bool CanReloadTodoFile() => _taskService.IsLoaded;

    private void OnReloadTodoFile() => _taskService.ReloadTasks();

    private bool CanArchiveCompletedTasks() => _taskService.ActiveTasks.Any(t => t.IsCompleted);

    private void OnArchiveCompletedTasks() => _taskService.ArchiveCompletedTasks();

    private async void OnOpenRecent(string? path)
    {
        if (!File.Exists(path))
        {
            await _dialogService.ShowMessageAsync("MSG_Open_Recent_Alert_Title".GetLocalized(), $"{"MSG_Open_Recent_Alert_Body".GetLocalized()}{path}");
            return;
        }
        OpenTodo(path);
    }

    private bool CanFileClearRecent() => _recentFilesService.RecentFiles.Count > 0;

    private void OnClearRecent() => _recentFilesService.ClearAsync();

    private void OnApplicationExit() => Application.Current.Exit();

    private bool CanCopyToClipboard() => _taskService.SelectedTasks.Count > 0;

    private void OnCopyToClipboard()
    {
        var package = new DataPackage();

        var content = string.Join(Environment.NewLine, _taskService.SelectedTasks.Select(t => t.Raw).ToList());
        package.SetText(content);

        Clipboard.SetContent(package);
    }

    private bool CanPasteFromClipboard() => _taskService.IsLoaded && Clipboard.GetContent().Contains(StandardDataFormats.Text);

    private async void OnPasteFromClipboard()
    {
        var package = Clipboard.GetContent();
        if (package.Contains(StandardDataFormats.Text))
        {
            var text = await package.GetTextAsync();
            var lines = text.Split(Environment.NewLine);

            foreach (var line in lines)
            {
                _taskService.AddTask(line);
            }
        }
    }

    private bool CanAddNewTasks() => _taskService.IsLoaded;

    private async void OnAddNewTask()
    {
        var response = await _dialogService.ShowDialogAsync<TaskPage>("TaskPage_New_Title".GetLocalized());
        if (response != null)
        {
            _taskService.AddTask(response.BindableString.Trim());
        }
    }

    private async void OnAddMultipleNewTasks()
    {
        var response = await _dialogService.ShowDialogAsync<MultipleTaskPage>("MultipleTaskPage_Title".GetLocalized());
        if (response != null)
        {
            var tasks = response.BindableString.ReplaceLineEndings().Split(Environment.NewLine);
            foreach (var task in tasks)
            {
                _taskService.AddTask(task.Trim());
            }
        }
    }
    private bool CanCopyToNewTask() => _taskService.SelectedTasks.Count == 1;

    private async void OnCopyToNewTask()
    {
        var model = new BindableModel
        {
            BindableString = _taskService.SelectedTasks[0].Raw,
        };

        var response = await _dialogService.ShowDialogAsync<TaskPage>("TaskPage_Copy_Title".GetLocalized(), model);
        if (response != null)
        {
            _taskService.AddTask(response.BindableString.Trim());
        }
    }

    private bool CanUpdateTask() => _taskService.SelectedTasks.Count == 1;

    private async void OnUpdateTask()
    {
        var model = new BindableModel
        {
            BindableString = _taskService.SelectedTasks.First().Raw,
        };
        var response = await _dialogService.ShowDialogAsync<TaskPage>("TaskPage_Update_Title".GetLocalized(), model);
        if (response != null)
        {
            _taskService.UpdateTask(_taskService.SelectedTasks.First(), response.BindableString);
        }
    }

    private bool CanAppendTextToTask() => _taskService.SelectedTasks.Count > 0;
    
    private async void OnAppendTextToTask()
    {
        var response = await _dialogService.ShowDialogAsync<TaskPage>("TaskPage_Append_Title".GetLocalized());
        if (response != null)
        {
            foreach(var task in  _taskService.SelectedTasks)
            {
                var tempTask = _taskService.Parse(task.Raw);
                if (tempTask != null)
                {
                    tempTask.Body = $"{tempTask.Body} {response.BindableString}";
                    _taskService.UpdateTask(task, tempTask.GetFormattedRaw());
                }
            }
            
        }
    }

    private bool CanDeleteTask() => _taskService.SelectedTasks.Count > 0;

    private async void OnDeleteTask()
    {
        var confirmed = true;

        if (_todoSettings.ConfirmBeleteDelete)
        {
            confirmed = await _dialogService.ShowConfirmationAsync("MSG_Delete_Confirmation_Title".GetLocalized(), "MSG_Delete_Confirmation_Body".GetLocalized());
        }
        if (confirmed)
        {
            foreach (var task in _taskService.SelectedTasks.ToList())
            {
                _taskService.DeleteTask(task);
            }
        }
    }

    private bool CanToggleCompleted() => _taskService.SelectedTasks.Count > 0;

    private void OnToggleCompleted()
    {
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.ToggleCompleted(task);
        }
    }

    private bool CanToggleHidden() => _taskService.SelectedTasks.Count > 0;

    private void OnToggleHidden()
    {
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.ToggleHidden(task);
        }
    }

    private bool CanSetPriority() => _taskService.SelectedTasks.Count > 0;

    private async void OnSetPriority()
    {
        var lastSelectedTask = _taskService.SelectedTasks.LastOrDefault();
        var model = new BindableModel
        {
            BindableString = !string.IsNullOrEmpty(lastSelectedTask?.Priority)
                ? lastSelectedTask.Priority
                : (string.IsNullOrEmpty(_todoSettings.DefaultPriority)) ? _todoSettings.DefaultPriority.ToString() : "A",
        };
        var response = await _dialogService.ShowDialogAsync<PriorityPage>("PriorityPage_Title".GetLocalized(), model);
        if (response != null)
        {
            var priority = response.BindableString.ToUpper().Trim();
            priority = !string.IsNullOrWhiteSpace(priority) && char.IsLetter(priority[0]) ? priority : string.Empty;

            foreach (var task in _taskService.SelectedTasks.ToList())
            {
                _taskService.SetPriority(task, priority);
            }
        }
    }

    private bool CanClearPriority() => _taskService.SelectedTasks.Count > 0;

    private void OnClearPriority()
    {
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.SetPriority(task, string.Empty);
        }
    }

    private bool CanIncreasePriority()
    {
        return _taskService.SelectedTasks.Count switch
        {
            0 => false,
            > 1 => true,
            _ => _taskService.SelectedTasks[0].Priority != "A"
        };
    }

    private void OnIncreasePriority()
    {
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            ChangePriority(task, -1);
        }
    }

    private bool CanDecreasePriority()
    {
        return _taskService.SelectedTasks.Count switch
        {
            0 => false,
            > 1 => true,
            _ => _taskService.SelectedTasks[0].Priority != "Z"
        };
    }

    private void OnDecreasePriority()
    {
        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            ChangePriority(task, 1);
        }
    }

    private bool CanSetDate(string? parameter) => _taskService.SelectedTasks.Count > 0;

    private async void OnSetDate(string? parameter)
    {
        if (parameter == null) return;

        DateTypes type = (DateTypes)Enum.Parse(typeof(DateTypes), parameter);
        var title = type == DateTypes.Due ? "DatePage_Due_Title".GetLocalized() : "DatePage_Threshold_Title".GetLocalized();

        var lastSelectedTask = _taskService.SelectedTasks.LastOrDefault();
        var model = new BindableModel
        {
            BindableDateTime = !string.IsNullOrEmpty(type == DateTypes.Due ? lastSelectedTask?.DueDate : lastSelectedTask?.ThresholdDate)
                ? Convert.ToDateTime(type == DateTypes.Due ? lastSelectedTask?.DueDate : lastSelectedTask?.ThresholdDate)
                : DateTime.Today,
        };
        var response = await _dialogService.ShowDialogAsync<DatePage>(title, model);
        if (response != null && response?.BindableDateTime != null)
        {
            var date = response.BindableDateTime.Value.Date.ToTodoDate();

            foreach (var task in _taskService.SelectedTasks.ToList())
            {
                _taskService.SetDate(task, date, type);
            }
        }
    }

    private bool CanPostponeDate(string? parameter) => _taskService.SelectedTasks?.Count > 0;

    private async void OnPostponeDate(string? parameter)
    {
        if (parameter == null) return;

        DateTypes type = (DateTypes)Enum.Parse(typeof(DateTypes), parameter);
        var title = type == DateTypes.Due ? "PostponePage_Due_Title".GetLocalized() : "PostponePage_Threshold_Title".GetLocalized();

        var model = new BindableModel();
        var response = await _dialogService.ShowDialogAsync<PostponePage>(title, model);

        if (response != null && !string.IsNullOrEmpty(response?.BindableString))
        {
            foreach (var task in _taskService.SelectedTasks.ToList())
            {
                _taskService.PostponeDate(task, response.BindableString, type);
            }
        }
    }

    private bool CanIncreaseDate(string? parameter) => _taskService.SelectedTasks.Count > 0;

    private void OnIncreaseDate(string? parameter)
    {
        if (parameter == null) return;

        DateTypes type = (DateTypes)Enum.Parse(typeof(DateTypes), parameter);

        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.PostponeDate(task, "1", type);
        }
    }

    private bool CanDecreaseDate(string? parameter) => _taskService.SelectedTasks.Count > 0;

    private void OnDecreaseDate(string? parameter)
    {
        if (parameter == null) return;

        DateTypes type = (DateTypes)Enum.Parse(typeof(DateTypes), parameter);

        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.PostponeDate(task, "-1", type);
        }
    }

    private bool CanClearDate(string? parameter) => _taskService.SelectedTasks?.Count > 0;

    private void OnClearDate(string? parameter)
    {
        if (parameter == null) return;

        DateTypes type = (DateTypes)Enum.Parse(typeof(DateTypes), parameter);

        foreach (var task in _taskService.SelectedTasks.ToList())
        {
            _taskService.SetDate(task, string.Empty, type);
        }
    }

    private void OnSortOption(string? sortOption)
    {
        _viewSettings.CurrentSort = (SortOptions)Enum.Parse(typeof(SortOptions), sortOption ?? "None");
        App.GetService<MainViewModel>().UpdateCollectionView();
    }

    private void OnSortDirection(string? sortDirection)
    {
        _viewSettings.CurrentSortDirection = (SortDirection)Enum.Parse(typeof(SortDirection), sortDirection ?? "Ascending");
        App.GetService<MainViewModel>().UpdateCollectionView();
    }

    private void OnShowInGroups() => App.GetService<MainViewModel>().UpdateCollectionView();

    private void OnHideFutureTasks() => App.GetService<MainViewModel>().UpdateCollectionView();

    private void OnShowHiddenTasks() => App.GetService<MainViewModel>().UpdateCollectionView();

    private void OnShowFilters() => _filterSettings.IsFiltersVisible = !_filterSettings.IsFiltersVisible;

    private void OnApplyPresetFilter(string? index)
    {
        if (string.IsNullOrEmpty(index)) return;

        var ndx = Int32.Parse(index);

        _filterSettings.ActiveFilter = ndx switch
        {
            1 => _filterSettings.PresetFilter1,
            2 => _filterSettings.PresetFilter2,
            3 => _filterSettings.PresetFilter3,
            4 => _filterSettings.PresetFilter4,
            5 => _filterSettings.PresetFilter5,
            6 => _filterSettings.PresetFilter6,
            7 => _filterSettings.PresetFilter7,
            8 => _filterSettings.PresetFilter8,
            9 => _filterSettings.PresetFilter9,
            _ => string.Empty
        };

        _filterSettings.CurrentPresetFilter = (PresetFilters)Enum.Parse(typeof(PresetFilters), index);

        App.GetService<MainViewModel>().UpdateCollectionView();
    }

    private async void OnFeatureNotImplemented(string? feature) => await _dialogService.ShowFeatureNotImpletmented(feature ?? "Feature_Not_Implemented_Title".GetLocalized());

    private void OnActiveTasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => RaiseCommandCanExecute();

    private void OnRecentFilesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => IsRecentEnabled = _recentFilesService.RecentFiles.Count > 0;

    private void ChangePriority(Models.Task task, int shift)
    {
        if (string.IsNullOrEmpty(task.Priority))
        {
            _taskService.SetPriority(task, "A");
        }
        else
        {
            var current = task.Priority[0];
            var priority = (char)(current + shift);

            if (char.IsLetter(priority))
            {
                _taskService.SetPriority(task, priority.ToString());
            }
        }
    }

    private void OpenTodo(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        _taskService.LoadTasks(path);
        _viewSettings.QuickSearchString = string.Empty;
    }

    public override bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args)
    {
        if (NavigationService.Frame?.GetPageViewModel() is not IViewModel vm) return false;

        if (vm.ShellKeyEventTriggered(args)) return true;

        switch (args.KeyboardAccelerator.Key)
        {
            case VirtualKey.F1:
                OnShowAbout();
                return true;
            case VirtualKey.F3:
                OnQuickSearchFocusedChanged(true.ToString());
                return true;
            case VirtualKey.F10:
                OnShowSettings();
                return true;
            default:
                return false;
        }
    }
}