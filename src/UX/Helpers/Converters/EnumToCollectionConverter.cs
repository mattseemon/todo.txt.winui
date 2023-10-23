using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;

using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Models.Settings;

using Windows.ApplicationModel.Chat;

namespace Seemon.Todo.Helpers.Converters;

public class EnumToCollectionConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => GetValuesAndDescriptions(value.GetType());

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();

    private static IEnumerable<ValueDescription> GetValuesAndDescriptions(Type t)
        => !t.IsEnum
        ? throw new ArgumentException($"{nameof(t)} must be an enumeration type.")
        : Enum.GetValues(t).Cast<Enum>().Select((e) => new ValueDescription() { Value = e, Description = e.GetDescription() }).ToList();

    protected override object ProvideValue(IXamlServiceProvider serviceProvider) => this;
}
