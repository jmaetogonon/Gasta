using Gasta.Shared.Data.Repositories;
using Gasta.Shared.Models;

namespace Gasta.Shared.Data.IndexedDb;

public class IndexedDbCategoryBudgetRepository : ICategoryBudgetRepository
{
    private const string Store = "categoryBudgets";
    private readonly IndexedDbService _db;

    public IndexedDbCategoryBudgetRepository(IndexedDbService db) => _db = db;

    public async Task<List<CategoryBudget>> GetAllForMonthAsync(int year, int month) =>
        (await _db.GetAllAsync<CategoryBudget>(Store))
            .Where(b => b.Year == year && b.Month == month)
            .ToList();

    public async Task<CategoryBudget?> GetForCategoryMonthAsync(int categoryId, int year, int month) =>
        (await _db.GetAllAsync<CategoryBudget>(Store))
            .FirstOrDefault(b => b.CategoryId == categoryId && b.Year == year && b.Month == month);

    public async Task SetForCategoryMonthAsync(int categoryId, int year, int month, decimal amount)
    {
        var existing = await GetForCategoryMonthAsync(categoryId, year, month);
        var budget = existing ?? new CategoryBudget { CategoryId = categoryId, Year = year, Month = month };
        budget.Amount = amount;
        var id = await _db.PutAsync(Store, budget);
        budget.Id = id;
    }

    public async Task<List<CategoryBudget>> GetAllAsync() =>
        (await _db.GetAllAsync<CategoryBudget>(Store)).ToList();
}
