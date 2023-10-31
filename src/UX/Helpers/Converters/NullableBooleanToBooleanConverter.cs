using Microsoft.UI.Xaml.Data;

namespace Seemon.Todo.Helpers.Converters;

public class NullableBooleanToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => value is bool boolVal ? boolVal : (object)false;

    public object ConvertBack(object value, Type targetType, object parameter, string language) => value is bool boolVal ? boolVal : (object)false;
}
