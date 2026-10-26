namespace Gasta.Shared.Abstractions;

/// <summary>
/// Tiny persistent string store for app settings and flags (theme, accent, nickname,
/// onboarded). The web host backs it with window.localStorage; the MAUI host will back
/// it with Preferences. GetAsync never throws: an unreadable or missing key is null.
/// </summary>
public interface IKeyValueStore
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    Task RemoveAsync(string key);
}
