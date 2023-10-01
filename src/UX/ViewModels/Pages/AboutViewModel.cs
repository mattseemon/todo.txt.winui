using System.Reflection;

using Windows.ApplicationModel;

using Seemon.Todo.Helpers.Common;
using Seemon.Todo.Helpers.Extensions;
using Seemon.Todo.Helpers.ViewModels;

namespace Seemon.Todo.ViewModels.Pages;

public class AboutViewModel : ViewModelBase
{
    public string Copyright
    {
        get;
    }

    public string Version
    {
        get;
    }

    public string Description
    {
        get;
    }

    public string Author
    {
        get;
    }

    public AboutViewModel()
    {
        Copyright = "AppCopyright".GetLocalized();
        Author = "AppAuthor".GetLocalized();
        Description = "AppDescription".GetLocalized();
        Version = GetVersionDescription();
    }

    private static string GetVersionDescription()
    {
        Version version;
        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;
            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }
        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
