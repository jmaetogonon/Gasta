using Gasta.Shared.Abstractions;
using SQLite;

namespace Gasta.Maui.Data;

/// <summary>
/// Owns the single SQLite connection for the app. Opened lazily and exactly once (all callers
/// share one open), and a failed open can be retried. The file path is passed in so this class
/// has no MAUI dependency (MauiProgram supplies FileSystem.AppDataDirectory).
/// </summary>
public class SqliteDatabase : IAppDatabase
{
    private readonly string _path;
    private readonly object _gate = new();
    private Task<SQLiteAsyncConnection>? _open;

    public SqliteDatabase(string path) => _path = path;

    public async Task<SQLiteAsyncConnection> ConnectionAsync()
    {
        Task<SQLiteAsyncConnection> task;
        lock (_gate) task = _open ??= OpenAsync();
        try { return await task; }
        catch
        {
            lock (_gate) { if (ReferenceEquals(_open, task)) _open = null; }
            throw;
        }
    }

    private async Task<SQLiteAsyncConnection> OpenAsync()
    {
        var conn = new SQLiteAsyncConnection(_path,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        await conn.CreateTableAsync<ExpenseRow>();
        await conn.CreateTableAsync<CategoryRow>();
        await conn.CreateTableAsync<PaymentMethodRow>();
        await conn.CreateTableAsync<BudgetRow>();
        await conn.CreateTableAsync<CategoryBudgetRow>();
        return conn;
    }

    public Task InitializeAsync() => ConnectionAsync();

    /// <summary>Wipes every table in one transaction (restore / reset). Id counters are NOT reset.</summary>
    public async Task ClearAllAsync()
    {
        var conn = await ConnectionAsync();
        await conn.RunInTransactionAsync(tx =>
        {
            tx.DeleteAll<ExpenseRow>();
            tx.DeleteAll<CategoryRow>();
            tx.DeleteAll<PaymentMethodRow>();
            tx.DeleteAll<BudgetRow>();
            tx.DeleteAll<CategoryBudgetRow>();
        });
    }

    /// <summary>
    /// Id == 0 means "new": insert and let SQLite assign the id (written back onto the row).
    /// Any other id means "this exact row": replace it, or create it with that id (this is what
    /// Restore Backup relies on to keep ids, and therefore category/payment links, intact).
    /// </summary>
    public async Task UpsertAsync<T>(T row) where T : class, IRow
    {
        var conn = await ConnectionAsync();
        if (row.Id == 0) await conn.InsertAsync(row);
        else await conn.InsertOrReplaceAsync(row);
    }
}
