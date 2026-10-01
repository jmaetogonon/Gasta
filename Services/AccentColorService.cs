using Microsoft.JSInterop;

namespace Gasta.Services;

public record AccentPreset(
    string Id, string Name, string SwatchHex,
    string LightPrimary, string LightPrimaryDark, string LightNavActive,
    string DarkPrimary, string DarkPrimaryDark, string DarkNavActive);

/// <summary>
/// Runtime accent-color switching. Works because every component in theme.css reads
/// color exclusively through CSS custom properties — this injects a small stylesheet
/// that redefines --color-primary/--color-primary-dark/--nav-active-bg for BOTH the
/// light (:root) and dark ([data-theme="dark"]) selectors, same shape as theme.css
/// itself, so it participates correctly in the light/dark cascade instead of
/// overriding it. Backgrounds/surfaces/borders are NOT touched here in either theme —
/// light mode keeps its original purple-tinted neutrals regardless of accent, and
/// dark mode uses the shared neutral base defined directly in theme.css.
/// </summary>
public class AccentColorService
{
    private const string StorageKey = "gasta-accent-id";
    private readonly IJSRuntime _js;

    public AccentColorService(IJSRuntime js) => _js = js;

    public static readonly List<AccentPreset> Presets = new()
    {
        new("purple", "Purple", "#5B4FE8", "#5B4FE8", "#4A3FC4", "#C9C2F5", "#7C6FFF", "#6355E0", "#453E7A"),
        new("blue",   "Blue",   "#3B6FEA", "#3B6FEA", "#2F58C4", "#C2D3F5", "#4C82E0", "#3A68C0", "#324B78"),
        new("teal",   "Teal",   "#12A594", "#12A594", "#0D8577", "#B7E8E1", "#26B39C", "#1D8F7D", "#1F5C52"),
        new("coral",  "Coral",  "#E85B4F", "#E85B4F", "#C43F35", "#F5C9C2", "#D45E4F", "#B84A3D", "#7A3E3E"),
        new("rose",   "Rose",   "#E84F91", "#E84F91", "#C43575", "#F5C2DD", "#E0568F", "#C43F73", "#7A3E5C"),
        new("amber",  "Amber",  "#A87A15", "#A87A15", "#8A6410", "#F0DDB0", "#B3812A", "#93691F", "#6B5320"),        
        new("gold",   "Gold",   "#B8850F", "#B8850F", "#936A0C", "#F3E3B4", "#B98A0F", "#93690C", "#5C4515"), 
        new("slate",  "Slate",  "#5B6B8C", "#5B6B8C", "#45526B", "#C9D2E0", "#6E82AC", "#556694", "#3A4356"),
    };

    public async Task<string> GetCurrentPresetIdAsync()
    {
        var saved = await SafeGetItem(StorageKey);
        return Presets.Any(p => p.Id == saved) ? saved! : "purple";
    }

    public async Task ApplyCurrentAsync() => await ApplyInternal(await GetCurrentPresetIdAsync());

    public async Task SetAsync(string presetId)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, presetId);
        await ApplyInternal(presetId);
    }

    private async Task ApplyInternal(string presetId)
    {
        var preset = Presets.FirstOrDefault(p => p.Id == presetId) ?? Presets[0];
        var css =
            $":root {{ --color-primary:{preset.LightPrimary}; --color-primary-dark:{preset.LightPrimaryDark}; --nav-active-bg:{preset.LightNavActive}; }}\n" +
            $"[data-theme=\"dark\"] {{ --color-primary:{preset.DarkPrimary}; --color-primary-dark:{preset.DarkPrimaryDark}; --nav-active-bg:{preset.DarkNavActive}; }}";
        await _js.InvokeVoidAsync("gastaAccent.apply", css);
    }

    private async Task<string?> SafeGetItem(string key)
    {
        try { return await _js.InvokeAsync<string?>("localStorage.getItem", key); }
        catch { return null; }
    }
}