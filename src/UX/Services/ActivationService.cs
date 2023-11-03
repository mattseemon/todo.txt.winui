using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.Activation;
using Seemon.Todo.Contracts.Activation;
using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Views.Pages;

namespace Seemon.Todo.Services;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ISettingsService _settingsService;
    private readonly IRecentFilesService _recentFilesService;
    private readonly ITaskbarIconService _taskbarIconService;
    private UIElement? _shell = null;

    public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers,
        IThemeSelectorService themeSelectorService, ISettingsService settingsService, IRecentFilesService recentFilesService, ITaskbarIconService taskbarIconService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _themeSelectorService = themeSelectorService;
        _settingsService = settingsService;
        _recentFilesService = recentFilesService;
        _taskbarIconService = taskbarIconService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        // Execute tasks before activation.
        await InitializeAsync();

        // Set the MainWindow Content.
        if (App.MainWindow.Content == null)
        {
            _shell = App.GetService<ShellPage>();
            App.MainWindow.Content = _shell ?? new Frame();
        }

        _taskbarIconService.Initialize();

        // Handle activation via ActivationHandlers.
        await HandleActivationAsync(activationArgs);

        // Activate the MainWindow.
        App.MainWindow.Activate();

        // Execute tasks after activation.
        await StartupAsync();
    }

    public async Task DeactivateAsync()
    {
        _taskbarIconService.Destroy();
        _recentFilesService?.SortAndTrimRecents();
        await _settingsService.PersistAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (_defaultHandler.CanHandle(activationArgs))
        {
            await _defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        await _themeSelectorService.InitializeAsync().ConfigureAwait(false);
        await Task.CompletedTask;
    }

    private async Task StartupAsync()
    {
        await _themeSelectorService.SetRequestedThemeAsync();
        await Task.CompletedTask;
    }
}
