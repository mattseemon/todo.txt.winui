using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

using Seemon.Todo.Helpers.Extensions;

namespace Seemon.Todo.Helpers.Converters;

public class EnumToDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) 
        => (value as Enum) != null ? ((Enum)value).GetDescription() : DependencyProperty.UnsetValue;

    public object ConvertBack(object value, Type targetType, object parameter, string language) 
        => throw new NotImplementedException();
}
