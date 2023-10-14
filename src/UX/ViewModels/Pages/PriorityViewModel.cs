using System.Text.RegularExpressions;
using System.Windows.Input;

using Microsoft.UI.Xaml.Input;

using Seemon.Todo.Helpers.ViewModels;

using Windows.System;

namespace Seemon.Todo.ViewModels.Pages;

public class PriorityViewModel : ViewModelBase
{
    private ICommand? _previewKeyDownCommand;

    public ICommand PreviewKeyDownCommand => _previewKeyDownCommand ??= RegisterCommand<KeyRoutedEventArgs>(OnPreviewKeyDown);

    private void OnPreviewKeyDown(KeyRoutedEventArgs args)
    {
        if (args.Key == VirtualKey.Up || args.Key == VirtualKey.Down)
        {
            if (string.IsNullOrEmpty(BindableModel.BindableString))
            {
                BindableModel.BindableString = "A";
                return;
            }

            BindableModel.BindableString = BindableModel.BindableString.ToUpper();

            Regex regex = new("[A-Z]");
            if (regex.IsMatch(BindableModel.BindableString))
            {
                switch (args.Key)
                {
                    case VirtualKey.Up:
                        if (BindableModel.BindableString != "A")
                        {
                            char priority = (char)((int)BindableModel.BindableString[0] - 1);
                            BindableModel.BindableString = priority.ToString();
                        }
                        break;
                    case VirtualKey.Down:
                        if (BindableModel.BindableString != "Z")
                        {
                            char priority = (char)((int)BindableModel.BindableString[0] + 1);
                            BindableModel.BindableString = priority.ToString();
                        }
                        break;
                }
            }
            args.Handled = true;
        }
    }
}