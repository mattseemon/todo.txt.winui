﻿using Microsoft.UI.Xaml.Data;

namespace Seemon.Todo.Helpers.Converters;

public class EnumToBooleanConverter : IValueConverter
{
    public Type? EnumType { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (EnumType == null) throw new ArgumentNullException(nameof(EnumType));

        if (parameter is string enumString)
        {
            if (!Enum.IsDefined(EnumType, value))
            {
                throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");
            }

            var enumValue = Enum.Parse(EnumType, enumString);
            return enumValue.Equals(value);
        }
        throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (EnumType == null) throw new ArgumentNullException(nameof(EnumType));

        return parameter is string enumString
                ? Enum.Parse(EnumType, enumString)
                : throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
    }
}
