using Gasta.Shared.Data.Repositories;
using Gasta.Shared.Models;

namespace Gasta.Shared.Data.IndexedDb;

public class IndexedDbCategoryRepository : ICategoryRepository
{
    private const string Store = "categories";
    private readonly IndexedDbService _db;

    public IndexedDbCategoryRepository(IndexedDbService db) => _db = db;

    public async Task<List<Category>> GetActiveOrderedAsync() =>
        (await _db.GetAllAsync<Category>(Store)).OrderBy(c => c.SortOrder).ToList();

    public async Task<Category?> GetByIdAsync(int id) =>
        (await _db.GetAllAsync<Category>(Store)).FirstOrDefault(c => c.Id == id);

    public async Task<int> SaveAsync(Category category)
    {
        var id = await _db.PutAsync(Store, category);
        category.Id = id; // put() now returns the real autoIncrement key when Id was 0
        return id;
    }

    public Task DeleteAsync(int id) => _db.DeleteAsync(Store, id);
}
