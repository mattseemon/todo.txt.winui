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

    public async Task ShowFeatureNotImpletmented()
    {
        var page = (Page)_navigationService.Frame.Content;

        ContentDialog dialog = new ContentDialog()
        {
            Title = "Feature not implemented",
            Content = "The feature you are trying to access is currently not implemented yet.",
            CloseButtonText = "Ok"
        };
        dialog.XamlRoot = page.XamlRoot;

        await dialog.ShowAsync();
    }
}
