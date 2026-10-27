using Gasta.Shared.Abstractions;

namespace Gasta.Maui.Services;

/// <summary>
/// Settings and flags (theme, accent, nickname, onboarded) in Android SharedPreferences.
/// Reads never throw: a missing or unreadable key is null, same contract as the web store.
/// </summary>
public class PreferencesKeyValueStore : IKeyValueStore
{
    public Task<string?> GetAsync(string key)
    {
        try
        {
            return Task.FromResult<string?>(
                Preferences.Default.ContainsKey(key) ? Preferences.Default.Get(key, "") : null);
        }
        catch { return Task.FromResult<string?>(null); }
    }

    public Task SetAsync(string key, string value)
    {
        Preferences.Default.Set(key, value);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        Preferences.Default.Remove(key);
        return Task.CompletedTask;
    }
}
