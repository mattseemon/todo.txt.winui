﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

using Seemon.Todo.Activation;
using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Models.Settings;
using Seemon.Todo.Services;
using Seemon.Todo.ViewModels.Pages;
using Seemon.Todo.Views.Pages;
using Seemon.Todo.Views.Windows;

using WinUIEx;

namespace Seemon.Todo;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host { get; }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<ITaskService, TaskService>();
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<IRecentFilesService, RecentFilesService>();
            services.AddSingleton<IFileMonitorService, FileMonitorService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddSingleton<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddSingleton<ShellViewModel>();
            services.AddTransient<ShellPage>();
            services.AddTransient<AboutViewModel>();
            services.AddTransient<AboutPage>();
            services.AddTransient<TaskViewModel>();
            services.AddTransient<TaskPage>();
            services.AddTransient<MultipleTaskViewModel>();
            services.AddTransient<MultipleTaskPage>();
            services.AddTransient<PriorityViewModel>();
            services.AddTransient<PriorityPage>();
            services.AddTransient<DateViewModel>();
            services.AddTransient<DatePage>();
            services.AddTransient<PostponeViewModel>();
            services.AddTransient<PostponePage>();

            // Configuration
            services.Configure<FileSettingsOptions>(context.Configuration.GetSection(nameof(FileSettingsOptions)));
            services.Configure<ApplicationUrls>(context.Configuration.GetSection("Urls"));
        }).
        Build();

        UnhandledException += OnUnhandledException;
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
