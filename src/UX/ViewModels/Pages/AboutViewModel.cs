using System.Reflection;

using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;

using Windows.ApplicationModel;

namespace Seemon.Todo.ViewModels.Pages;

public class AboutViewModel : ViewModelBase
{
    public string Copyright { get; }

    public string Version { get; }

    public string Description { get; }

    public string Author { get; }

    public AboutViewModel()
    {
        Copyright = "AboutPage_Copyright".GetLocalized();
        Author = "AboutPage_Author".GetLocalized();
        Description = "AboutPage_Description".GetLocalized();
        Version = GetVersionDescription();
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
}
