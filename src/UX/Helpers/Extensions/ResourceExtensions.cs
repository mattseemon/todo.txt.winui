using Microsoft.Windows.ApplicationModel.Resources;

namespace Seemon.Todo.Helpers.Extensions;

public static class ResourceExtensions
{
    private static readonly ResourceLoader _resourceLoader = new();

    public static string GetLocalized(this string resourceKey) => _resourceLoader.GetString(resourceKey);
}
