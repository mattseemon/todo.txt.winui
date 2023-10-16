using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.Models.Common;

namespace Seemon.Todo.Contracts.Services;

public interface IDialogService
{
    Task<BindableModel?> ShowDialogAsync<T>(string title, BindableModel? model = null)
        where T : Page;

    Task ShowMessageAsync(string title, string message);

    Task<bool> ShowConfirmationAsync(string title, string message);

    Task ShowFeatureNotImpletmented(string feature);
}
