using Gasta.Shared.Abstractions;
using Microsoft.JSInterop;

namespace Gasta.Shared.Platform.Web;

public class LocalStorageKeyValueStore : IKeyValueStore
{
    private readonly IJSRuntime _js;

    public LocalStorageKeyValueStore(IJSRuntime js) => _js = js;

    // Reads never throw: storage can be blocked (private mode, disabled cookies), and a
    // missing preference should fall back to its default rather than crash startup.
    public async Task<string?> GetAsync(string key)
    {
        try { return await _js.InvokeAsync<string?>("localStorage.getItem", key); }
        catch { return null; }
    }

    public async Task SetAsync(string key, string value) =>
        await _js.InvokeVoidAsync("localStorage.setItem", key, value);

    public async Task RemoveAsync(string key) =>
        await _js.InvokeVoidAsync("localStorage.removeItem", key);
}
