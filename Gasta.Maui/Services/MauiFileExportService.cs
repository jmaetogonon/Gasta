using Gasta.Shared.Abstractions;

namespace Gasta.Maui.Services;

/// <summary>
/// A WebView cannot "download" a blob the way a browser does. The user chooses:
///   Save to device : Android's save-file dialog (Downloads, Drive, anywhere they pick)
///   Share          : Android share sheet (Drive, WhatsApp, email...)
/// </summary>
public class MauiFileExportService : IFileExportService
{
    private const string SaveChoice = "Save to device";
    private const string ShareChoice = "Share…";

    public async Task SaveAsync(string fileName, string mimeType, string content)
    {
        string? choice = null;
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page is not null)
                choice = await page.DisplayActionSheetAsync(fileName, "Cancel", null, SaveChoice, ShareChoice);
        });

        if (choice == SaveChoice)
        {
#if ANDROID
            await AndroidFileSaver.SaveAsync(fileName, mimeType, content);
#endif
        }
        else if (choice == ShareChoice)
        {
            var safeName = string.Concat(fileName.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
            var path = Path.Combine(FileSystem.CacheDirectory, safeName);
            await File.WriteAllTextAsync(path, content);

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = fileName,
                    File = new ShareFile(path, mimeType)
                });
            });
        }
    }
}
