using Gasta.Shared.Abstractions;
using Microsoft.JSInterop;

namespace Gasta.Shared.Platform.Web;

public class BlobDownloadExportService : IFileExportService
{
    private readonly IJSRuntime _js;

    public BlobDownloadExportService(IJSRuntime js) => _js = js;

    public async Task SaveAsync(string fileName, string mimeType, string content) =>
        await _js.InvokeVoidAsync("gastaExport.downloadFile", fileName, mimeType, content);
}
