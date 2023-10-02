using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.Contracts.Services;

namespace Seemon.Todo.Services;

public class DialogService : IDialogService
{
    private readonly INavigationService _navigationService;

    public DialogService(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public async Task ShowFeatureNotImpletmented(string feature)
    {
        await ShowMessageAsync(feature, $"The feature you are trying to access is currently not implemented yet.");
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        var page = _navigationService.Frame?.Content as Page;

        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "Ok",
            XamlRoot = page?.XamlRoot
        };

        await dialog.ShowAsync();
    }
}
