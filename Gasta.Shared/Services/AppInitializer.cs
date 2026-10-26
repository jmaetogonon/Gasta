using Gasta.Shared.Abstractions;

namespace Gasta.Shared.Services;

/// <summary>
/// Everything that must finish before the first page renders: open the database, seed it
/// on first run, apply the saved theme. Both hosts (web today, MAUI later) call this and
/// await it before showing UI, so no page's OnInitializedAsync can read mid-seed.
/// Safe to call more than once or concurrently: all callers share one run, and a failed
/// run can be retried.
/// </summary>
public class AppInitializer
{
    private readonly IAppDatabase _database;
    private readonly SeedService _seed;
    private readonly ThemeService _theme;
    private readonly object _gate = new();
    private Task? _run;

    public AppInitializer(IAppDatabase database, SeedService seed, ThemeService theme)
    {
        _database = database;
        _seed = seed;
        _theme = theme;
    }

    public Task InitializeAsync()
    {
        lock (_gate) return _run ??= RunAsync();
    }

    private async Task RunAsync()
    {
        try
        {
            await _database.InitializeAsync();
            await _seed.SeedIfEmptyAsync();
            await _theme.ApplySavedAsync();
        }
        catch
        {
            lock (_gate) _run = null;
            throw;
        }
    }
}
