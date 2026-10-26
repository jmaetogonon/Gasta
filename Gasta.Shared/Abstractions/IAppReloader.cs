namespace Gasta.Shared.Abstractions;

/// <summary>
/// Restarts the UI after a restore or reset so every page re-reads fresh data. On the web
/// that is location.reload().
/// </summary>
public interface IAppReloader
{
    Task ReloadAsync();
}
