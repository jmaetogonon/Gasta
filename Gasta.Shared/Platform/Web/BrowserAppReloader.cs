using Gasta.Shared.Abstractions;
using Microsoft.JSInterop;

namespace Gasta.Shared.Platform.Web;

public class BrowserAppReloader : IAppReloader
{
    private readonly IJSRuntime _js;

    public BrowserAppReloader(IJSRuntime js) => _js = js;

    public async Task ReloadAsync() => await _js.InvokeVoidAsync("location.reload");
}
