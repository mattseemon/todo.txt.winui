using Microsoft.UI.Xaml;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.ViewModels.Pages;

namespace Seemon.Todo.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INavigationService _navigationService;

    public DefaultActivationHandler(INavigationService navigationService)
        => _navigationService = navigationService;

    // None of the ActivationHandlers has handled the activation.
    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        => _navigationService.Frame?.Content == null;

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        _navigationService.NavigateTo(typeof(MainViewModel).FullName!, args.Arguments);
        await Task.CompletedTask;
    }
}
