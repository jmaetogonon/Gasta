using Microsoft.JSInterop;

namespace Gasta.Services;

public class ThemeService
{
    private const string StorageKey = "gasta-theme";
    private readonly IJSRuntime _js;

    public ThemeService(IJSRuntime js) => _js = js;

    public async Task ApplySavedAsync()
    {
        // Default changed from "System" to "Light" — System is being removed as an option.
        var saved = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey) ?? "Light";
        await ApplyAsync(saved, persist: false);
    }

    public async Task ApplyAsync(string option, bool persist = true)
    {
        await _js.InvokeVoidAsync("gastaTheme.apply", option);
        if (persist)
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, option);
    }

    /// <summary>
    /// Reads back the persisted theme choice for UI display (e.g. Settings' active toggle
    /// state), without re-applying it. Defaults to "Light" if nothing has been saved yet.
    /// </summary>
    public async Task<string> GetCurrentAsync()
    {
        var saved = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        return saved is "Light" or "Dark" ? saved : "Light";
    }
}
