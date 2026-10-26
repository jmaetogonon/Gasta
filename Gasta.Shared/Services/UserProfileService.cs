using Gasta.Shared.Abstractions;

namespace Gasta.Shared.Services;

/// <summary>
/// Owns the user's nickname and first-run "onboarded" flag, both persisted
/// through IKeyValueStore (localStorage on the web, Preferences in the MAUI app). Scoped so the in-memory cache is shared across the app session
/// without re-hitting localStorage on every read.
/// </summary>
public class UserProfileService
{
    private const string NicknameKey = "gasta-nickname";
    private const string OnboardedKey = "gasta-onboarded";

    private readonly IKeyValueStore _store;
    private string? _nickname;
    private bool _nicknameLoaded;

    public UserProfileService(IKeyValueStore store) => _store = store;

    public async Task<string> GetNicknameAsync()
    {
        if (!_nicknameLoaded)
        {
            _nickname = await _store.GetAsync(NicknameKey);
            _nicknameLoaded = true;
        }
        return string.IsNullOrWhiteSpace(_nickname) ? "Friend" : _nickname!;
    }

    public async Task SetNicknameAsync(string nickname)
    {
        _nickname = nickname.Trim();
        _nicknameLoaded = true;
        await _store.SetAsync(NicknameKey, _nickname);
    }

    public async Task<bool> IsOnboardedAsync() =>
        await _store.GetAsync(OnboardedKey) == "true";

    public async Task MarkOnboardedAsync() =>
        await _store.SetAsync(OnboardedKey, "true");

    /// <summary>
    /// Wipes nickname + onboarded flag, so a fresh Reset All Data means a genuinely
    /// fresh start — the person sees onboarding again next launch, not just an empty
    /// database with their old name still showing on Home.
    /// </summary>
    public async Task ClearProfileAsync()
    {
        _nickname = null;
        _nicknameLoaded = false;
        await _store.RemoveAsync(NicknameKey);
        await _store.RemoveAsync(OnboardedKey);
    }
}