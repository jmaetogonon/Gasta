using Gasta.Shared.Abstractions;
using Gasta.Shared.Data.IndexedDb;

namespace Gasta.Shared.Platform.Web;

public class IndexedDbAppDatabase : IAppDatabase
{
    private readonly IndexedDbService _db;

    public IndexedDbAppDatabase(IndexedDbService db) => _db = db;

    public Task InitializeAsync() => _db.InitializeAsync();

    public Task ClearAllAsync() => _db.ClearAllAsync();
}
