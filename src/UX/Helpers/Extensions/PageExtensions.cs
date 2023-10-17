using Microsoft.UI.Xaml.Controls;

using Seemon.Todo.Contracts.ViewModels;

namespace Seemon.Todo.Helpers.Extensions;

public static class PageExtensions
{
    public static IViewModel? GetPageViewModel(this Page page)
        => page.GetType().GetProperty("ViewModel")?.GetValue(page, null) as IViewModel;
}
