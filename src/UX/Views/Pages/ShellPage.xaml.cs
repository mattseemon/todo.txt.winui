using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.ViewModels.Pages;

using Windows.System;

namespace Seemon.Todo.Views.Pages;

public sealed partial class ShellPage : Page
{
    private static readonly int MAX_LENGTH = 50;

    public ShellViewModel ViewModel { get; }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;

        // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
        // A custom title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleBarText.Text = "AppDisplayName".GetLocalized();
        ViewModel.RecentFiles.CollectionChanged += OnRecentFilesCollectionChanged;
        LoadRecentFilesMenubar();
    }

    private void OnRecentFilesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        LoadRecentFilesMenubar();
    }

    private void LoadRecentFilesMenubar()
    {
        foreach (var menu in RecentFilesMenu.Items.Cast<MenuFlyoutItem>()) menu.KeyboardAccelerators.Clear();
        RecentFilesMenu.Items.Clear();

        for (var i = 0; i < ViewModel.RecentFiles.Count; i++)
        {
            var item = ViewModel.RecentFiles[i];

            var menuItem = new MenuFlyoutItem()
            {
                AccessKey = (i).ToString(),
                Text = TrimPath(item.Path),
            };

            menuItem.Command = ViewModel.OpenRecentCommand;
            menuItem.CommandParameter = item.Path;

            ToolTipService.SetToolTip(menuItem, item.Path);

            RecentFilesMenu.Items.Add(menuItem);
        }
    }

    private static string TrimPath(string path)
    {
        if (path.Length > MAX_LENGTH)
        {
            var trimLength = MAX_LENGTH - Path.GetFileName(path).Length;
            var suffix = path[path.LastIndexOf('\\')..];

            if (trimLength <= 0) return path;
            do
            {
                path = path[..path.LastIndexOf('\\')];

            } while (path.Length > trimLength);

            path = $"{path}\\...{suffix}";
        }
        return path;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);

        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Back));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F10));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F1));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F3));
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";

        AppTitleBarText.Foreground = (SolidColorBrush)App.Current.Resources[resource];
        App.AppTitlebar = AppTitleBarText as UIElement;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
    }

    private KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue) keyboardAccelerator.Modifiers = modifiers.Value;

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
        return keyboardAccelerator;
    }

    private void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        // First let the loaded page handle the event.
        var result = ViewModel.ShellKeyEventTriggered(args);
        if (result)
        {
            args.Handled = true;
            return;
        }

        switch (args.KeyboardAccelerator.Key)
        {
            case VirtualKey.Back:
                var navigationService = App.GetService<INavigationService>();
                result = navigationService.GoBack();
                break;
        }
        args.Handled = result;
    }
}