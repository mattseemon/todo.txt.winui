using Seemon.Todo.Contracts.Services;
using Seemon.Todo.Helpers.Extensions;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;

namespace Seemon.Todo.Services;

public class SystemService : ISystemService
{
    public async Task<string> OpenFileDialogAsync()
    {
        var openPicker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };

        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        openPicker.FileTypeFilter.Add(".txt");
        openPicker.FileTypeFilter.Add("*");

        var file = await openPicker.PickSingleFileAsync();

        return (file != null) ? file.Path : string.Empty;
    }

    public async Task<string> OpenSaveDialogAsync(string filename = "todo")
    {
        FileSavePicker savePicker = new();

        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        savePicker.FileTypeChoices.Add("MSG_File_Filter".GetLocalized(), new List<string>() { ".txt" });
        savePicker.SuggestedFileName = filename;

        StorageFile file = await savePicker.PickSaveFileAsync();
        if (file != null)
        {
            CachedFileManager.DeferUpdates(file);
            await FileIO.WriteTextAsync(file, string.Empty);
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

            return status switch
            {
                FileUpdateStatus.Complete or FileUpdateStatus.CompleteAndRenamed => file.Path,
                _ => string.Empty,
            };
        }
        return string.Empty;
    }
}
