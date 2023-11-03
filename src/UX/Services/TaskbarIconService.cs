using H.NotifyIcon;

using Microsoft.UI.Xaml.Controls.Primitives;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Models.Settings;
using Seemon.Todo.ViewModels.Controls;

using WinUIEx;

namespace Seemon.Todo.Services;

public class TaskbarIconService : ITaskbarIconService
{
    private readonly AppSettings _appSettings;
    private readonly IThemeSelectorService _themeSelectorService;

    private WindowState _windowState;

    private TaskbarIcon? _taskbarIcon;

    public TaskbarIconService(ISettingsService settingsService, IThemeSelectorService themeSelectorService)
    {
        _appSettings = Task.Run(() => settingsService.GetAsync(Constants.SETTING_APPLICATION, AppSettings.Default)).Result;
        _themeSelectorService = themeSelectorService;
    }

    public void Initialize()
    {
        if (_appSettings.ShowInNotificationArea)
        {
            if (App.MainWindow == null) return;

            if (_taskbarIcon == null)
            {
                var dataContext = App.GetService<TaskbarIconViewModel>();
                _taskbarIcon = new TaskbarIcon
                {
                    DataContext = dataContext,
                    ContextFlyout = App.Current.Resources["TBIContextMenu"] as FlyoutBase,
                    Icon = new System.Drawing.Icon(Path.Combine(AppContext.BaseDirectory, "Assets/Todo.ico")),
                    ToolTipText = "AppDisplayName".GetLocalized(),
                    LeftClickCommand = dataContext.ShowCommand,
                    RequestedTheme = _themeSelectorService.Theme,
                };
                _taskbarIcon.ForceCreate();
                App.MainWindow.WindowStateChanged += OnWindowStateChanged;
                App.HandleClosedEvents = _appSettings.ShowInNotificationArea && _appSettings.CloseToNotificationArea;
            }
        }
    }

    private void OnWindowStateChanged(object? sender, WindowState e)
    {
        if (App.MainWindow.WindowState == WindowState.Minimized)
        {
            if (_appSettings.ShowInNotificationArea && _appSettings.MinimizeToNotificationArea)
            {
                Hide();
            }
        }
        else
        {
            _windowState = App.MainWindow.WindowState;
        }
    }

    public void Show()
    {
        if (App.MainWindow == null) return;

        App.MainWindow.Show();
        App.MainWindow.WindowState = _windowState;
        App.MainWindow.Activate();
    }

    public void Hide()
    {
        if (App.MainWindow == null) return;

        App.MainWindow.Hide();
    }

    public void Destroy()
    {
        if (_taskbarIcon == null) return;
        if (App.MainWindow == null) return;

        App.MainWindow.WindowStateChanged -= OnWindowStateChanged;
        _taskbarIcon.Dispose();
        _taskbarIcon = null;
    }
}
