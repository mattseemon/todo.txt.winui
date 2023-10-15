﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Exceptions;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Models.Common;

namespace Seemon.Todo.Services;

public class DialogService : IDialogService
{
    private readonly INavigationService _navigationService;

    public DialogService(INavigationService navigationService) => _navigationService = navigationService;

    public async Task<BindableModel?> ShowDialogAsync<T>(string title, BindableModel? model = null)
        where T : Page
    {
        var shell = _navigationService.Frame?.Content as Page ?? throw new TaskException("Could not find shell window");
        ElementTheme actualTheme = shell.ActualTheme;

        var page = App.GetService<T>() as Page ?? throw new TaskException("Could not find page to load.");

        var viewModel = page.GetPageViewModel();
        viewModel?.SetModel(model ?? new());

        var dialog = new ContentDialog
        {
            XamlRoot = shell?.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = title,
            Content = page,
            PrimaryButtonText = "OK",
            DefaultButton = ContentDialogButton.Primary,
            CloseButtonText = "Cancel",
            RequestedTheme = actualTheme,
        };

        return await dialog.ShowAsync() == ContentDialogResult.Primary ? (viewModel?.BindableModel) : null;
    }

    public async Task ShowFeatureNotImpletmented(string feature) 
        => await ShowMessageAsync(feature, $"The feature you are trying to access is currently not implemented yet.");

    public async Task ShowMessageAsync(string title, string message)
    {
        var shell = _navigationService.Frame?.Content as Page ?? throw new TaskException("Could not find shell window");
        ElementTheme actualTheme = shell.ActualTheme;

        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "Ok",
            XamlRoot = shell?.XamlRoot,
            RequestedTheme = actualTheme,
        };

        await dialog.ShowAsync();
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        var shell = _navigationService.Frame?.Content as Page ?? throw new TaskException("Could not find shell window");
        ElementTheme actualTheme = shell.ActualTheme;

        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = "OK",
            PrimaryButtonStyle = (Style)App.Current.Resources["AccentButtonStyle"],
            XamlRoot = shell?.XamlRoot,
            RequestedTheme = actualTheme,
        };

        return await dialog.ShowAsync() == ContentDialogResult.Primary;
    }
}
