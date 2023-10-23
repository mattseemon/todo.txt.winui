﻿using System.ComponentModel;
using System.Windows.Input;

using CommunityToolkit.WinUI.UI;

using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Contracts.ViewModels;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Comparers;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Models.Settings;

using SortDirection = CommunityToolkit.WinUI.UI.SortDirection;

namespace Seemon.Todo.ViewModels.Pages;

public class MainViewModel : ViewModelBase, INavigationAware
{
    private readonly ITaskService _taskService;
    private readonly IRecentFilesService _recentFilesService;
    private readonly ISettingsService _settingsService;

    private AppSettings _appSettings;
    private readonly ViewSettings _viewSettings;

    private FontFamily _fontFamily = FontFamily.XamlAutoFontFamily;
    private double _fontSize;

    private ICommand? _selectionChangedCommand;
    private ICommand? _doubleTappedCommand;

    public ICommand SelectionChangedCommand => _selectionChangedCommand ??= RegisterCommand(OnSelectionChanged);
    public ICommand DoubleTappedCommand => _doubleTappedCommand ??= RegisterCommand(OnDoubleTapped);

    public AdvancedCollectionView Tasks { get; private set; }


    public FontFamily Font
    {
        get => _fontFamily; set => SetProperty(ref _fontFamily, value);
    }

    public double FontSize
    {
        get => _fontSize; set => SetProperty(ref _fontSize, value);
    }

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

        Tasks = new AdvancedCollectionView(_taskService.ActiveTasks)
        {
            Filter = QuickSearch
        };

        SortList();

        Font = string.IsNullOrEmpty(_appSettings.FontFamily) ? FontFamily.XamlAutoFontFamily : new FontFamily(_appSettings.FontFamily);
        FontSize = _appSettings.FontSize;

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
            Tasks.Refresh();
        }
    }

    public void SortList()
    {
        Tasks.SortDescriptions.Clear();

        SortDirection direction = _viewSettings.CurrentSortDirection == Models.Settings.SortDirection.Ascending ? SortDirection.Ascending : SortDirection.Descending;

        switch (_viewSettings.CurrentSort)
        {
            case SortOptions.Alphabetical:
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.Raw), direction));
                break;
            case SortOptions.Completed:
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.IsCompleted), direction));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.Priority), direction, new TodoStringComparer()));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.ThresholdDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.DueDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.CreatedDate), direction, new TodoDateComparer(DateTime.MinValue)));
                break;
            case SortOptions.Priority:
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.Priority), direction, new TodoStringComparer()));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.IsCompleted), direction));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.ThresholdDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.DueDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.CreatedDate), direction, new TodoDateComparer(DateTime.MinValue)));
                break;
            case SortOptions.Context:
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.PrimaryContext), direction, new TodoStringComparer()));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.IsCompleted), direction));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.Priority), direction, new TodoStringComparer()));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.ThresholdDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.DueDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.CreatedDate), direction, new TodoDateComparer(DateTime.MinValue)));
                break;
            case SortOptions.Project:
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.PrimaryProject), direction, new TodoStringComparer()));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.IsCompleted), direction));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.Priority), direction, new TodoStringComparer()));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.ThresholdDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.DueDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.CreatedDate), direction, new TodoDateComparer(DateTime.MinValue)));
                break;
            case SortOptions.Created:
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.CreatedDate), direction, new TodoDateComparer(DateTime.MinValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.IsCompleted), direction));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.Priority), direction, new TodoStringComparer()));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.ThresholdDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.DueDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                break;
            case SortOptions.Due:
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.DueDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.IsCompleted), direction));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.Priority), direction, new TodoStringComparer()));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.ThresholdDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.CreatedDate), direction, new TodoDateComparer(DateTime.MinValue)));
                break;
            case SortOptions.Threshold:
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.ThresholdDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.IsCompleted), direction));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.Priority), direction, new TodoStringComparer()));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.DueDate), direction, new TodoDateComparer(DateTime.MaxValue)));
                Tasks.SortDescriptions.Add(new SortDescription(nameof(Models.Task.CreatedDate), direction, new TodoDateComparer(DateTime.MinValue)));
                break;
            default:
                Tasks.SortDescriptions.Add(new SortDescription(direction, new TodoIndexComparer()));
                break;

        }
        Tasks.RefreshSorting();
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

    private void OnCollectionChanged(object? sender, EventArgs e) => Tasks.Refresh();

    private void OnTasksLoaded(object? sender, string e) => Tasks.Refresh();

    public void OnNavigatedTo(object parameter)
    {
        _appSettings = Task.Run(() => _settingsService?.GetAsync(Constants.SETTING_APPLICATION, AppSettings.Default)).Result;
        Tasks.Refresh();
    }

    public void OnNavigatedFrom() { }

    public override bool ShellKeyEventTriggered(KeyboardAcceleratorInvokedEventArgs args)
        => base.ShellKeyEventTriggered(args);
}
