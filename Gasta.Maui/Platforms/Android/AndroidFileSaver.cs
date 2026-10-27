using Android.App;
using Android.Content;

namespace Gasta.Maui;

/// <summary>
/// Android's "create document" dialog (Storage Access Framework): the user picks the folder and
/// name, Android hands back a content URI, and we write the file to it. No storage permission needed.
/// MainActivity forwards the dialog's result to OnActivityResult below.
/// </summary>
public static class AndroidFileSaver
{
    private const int RequestCode = 9001;
    private static TaskCompletionSource<Android.Net.Uri?>? _pending;

    public static async Task<bool> SaveAsync(string fileName, string mimeType, string content)
    {
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity
                       ?? throw new InvalidOperationException("No active screen to show the save dialog.");

        var tcs = new TaskCompletionSource<Android.Net.Uri?>();
        _pending = tcs;

        var intent = new Intent(Intent.ActionCreateDocument);
        intent.AddCategory(Intent.CategoryOpenable);
        intent.SetType(mimeType);
        intent.PutExtra(Intent.ExtraTitle, fileName);
        await MainThread.InvokeOnMainThreadAsync(() => activity.StartActivityForResult(intent, RequestCode));

        var uri = await tcs.Task;
        if (uri is null) return false;   // user cancelled

        using var stream = activity.ContentResolver?.OpenOutputStream(uri, "wt")
                           ?? throw new IOException("Couldn't open the chosen file for writing.");
        var bytes = new System.Text.UTF8Encoding(false).GetBytes(content);
        await stream.WriteAsync(bytes);
        await stream.FlushAsync();
        return true;
    }

    public static void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        if (requestCode != RequestCode) return;
        var pending = _pending;
        _pending = null;
        pending?.TrySetResult(resultCode == Result.Ok ? data?.Data : null);
    }
}
