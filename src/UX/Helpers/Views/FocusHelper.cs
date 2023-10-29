using Microsoft.UI.Xaml;

namespace Seemon.Todo.Helpers.Views;

public static class FocusHelper
{
    public static readonly DependencyProperty IsFocusedProperty
        = DependencyProperty.Register("IsFocused", typeof(bool), typeof(FocusHelper), new PropertyMetadata(false, new PropertyChangedCallback(OnIsFocusedPropertyChanged)));

    public static bool GetIsFocused(DependencyObject dependencyObject)
        => (bool)dependencyObject.GetValue(IsFocusedProperty);

    public static void SetIsFocused(DependencyObject dependencyObject, bool value)
        => dependencyObject.SetValue(IsFocusedProperty, value);

    public static void OnIsFocusedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is UIElement element && (bool)e.NewValue)
        {
            element.Focus(FocusState.Programmatic);
        }
    }
}
