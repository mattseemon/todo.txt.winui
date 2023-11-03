using Microsoft.Extensions.Options;

using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;
using Seemon.Todo.Models.Settings;

using System.Reflection;
using System.Windows.Input;

using Windows.ApplicationModel;

namespace Seemon.Todo.ViewModels.Pages;

public class AboutViewModel : ViewModelBase
{
    private readonly ApplicationUrls _urls;
    private readonly ISystemService _systemService;

    private ICommand? _openInBrowserCommand;

    public string Copyright { get; }

    public string Version { get; }

    public string Description { get; }

    public string Author { get; }

    public ICommand OpenInBrowserCommand => _openInBrowserCommand ??= RegisterCommand<string>(OnOpenInBrowser);

    public AboutViewModel(IOptions<ApplicationUrls> applicationUrls, ISystemService systemService)
    {
        Copyright = "AboutPage_Copyright".GetLocalized();
        Author = "AboutPage_Author".GetLocalized();
        Description = "AboutPage_Description".GetLocalized();
        Version = GetVersionDescription();
        _urls = applicationUrls.Value;
        _systemService = systemService;
    }

    private static string GetVersionDescription()
    {
        string version;
        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;
            version = $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision}";
        }
        else
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            version = assemblyVersion != null ? assemblyVersion.InformationalVersion : "1.0.0.0";
        }
        return version;
    }

    private void OnOpenInBrowser(string? parameter)
    {
        if (parameter != null)
        {
            _systemService.OpenInWebBrowser(_urls[parameter]);
        }
    }
}
