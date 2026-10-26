using Microsoft.JSInterop;

namespace Gasta.Shared.Data.IndexedDb;

public class IndexedDbService : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private Task? _dbReadyTask;
    private readonly object _dbReadyLock = new();

    public IndexedDbService(IJSRuntime jsRuntime)
    {
        // Keep the leading "./": it resolves against <base href>, so it works at the site root
        // (localhost) AND under a sub-path (jmaetogonon.github.io/Gasta/). A leading "/" is
        // root-absolute and silently points outside the app on GitHub Pages.
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Gasta.Shared/js/db.js").AsTask());
    }

    private async Task<IJSObjectReference> ModuleAsync() => await _moduleTask.Value;

    /// <summary>
    /// Ensures openDb() has been called and completed exactly once, no matter how many
    /// components/repositories race to call a DB method first. Every public method below
    /// awaits this before touching the JS module, which is what was missing before —
    /// InitializeAsync() existed but nothing guaranteed it ran before GetAllAsync etc.
    /// </summary>
    private Task EnsureOpenAsync()
    {
        lock (_dbReadyLock)
        {
            _dbReadyTask ??= OpenInternalAsync();
            return _dbReadyTask;
        }
    }

    private async Task OpenInternalAsync()
    {
        var module = await ModuleAsync();
        await module.InvokeVoidAsync("openDb");
    }

    /// <summary>
    /// Safe to call explicitly too (e.g. eagerly in Program.cs) but no longer required —
    /// every other method self-initializes via EnsureOpenAsync().
    /// </summary>
    public Task InitializeAsync() => EnsureOpenAsync();

    public async Task<List<T>> GetAllAsync<T>(string storeName)
    {
        await EnsureOpenAsync();
        var module = await ModuleAsync();
        return await module.InvokeAsync<List<T>>("getAll", storeName);
    }

    public async Task<int> PutAsync<T>(string storeName, T item)
    {
        await EnsureOpenAsync();
        var module = await ModuleAsync();
        return await module.InvokeAsync<int>("put", storeName, item);
    }

    public async Task DeleteAsync(string storeName, int id)
    {
        await EnsureOpenAsync();
        var module = await ModuleAsync();
        await module.InvokeVoidAsync("remove", storeName, id);
    }

    public async Task ClearAllAsync()
    {
        await EnsureOpenAsync();
        var module = await ModuleAsync();
        await module.InvokeVoidAsync("clearAll");
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}