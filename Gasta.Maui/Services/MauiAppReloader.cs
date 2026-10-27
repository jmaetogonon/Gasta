using Gasta.Shared.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Gasta.Maui.Services;

/// <summary>
/// After a restore or reset every page must re-read fresh data. On the web that is
/// location.reload(); here it is a forced navigation, which restarts the app inside the
/// BlazorWebView. (Unverified on a device yet: confirm Restore Backup and Reset All Data
/// land back on a freshly loaded Home screen.)
/// </summary>
public class MauiAppReloader : IAppReloader
{
    private readonly NavigationManager _nav;

    public MauiAppReloader(NavigationManager nav) => _nav = nav;

    public Task ReloadAsync()
    {
        _nav.Refresh(forceReload: true);
        return Task.CompletedTask;
    }
}
