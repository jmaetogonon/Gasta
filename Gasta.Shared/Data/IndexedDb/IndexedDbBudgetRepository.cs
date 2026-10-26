using Gasta.Shared.Data.Repositories;
using Gasta.Shared.Models;

namespace Gasta.Shared.Data.IndexedDb;

public class IndexedDbBudgetRepository : IBudgetRepository
{
    private const string Store = "budgets";
    private readonly IndexedDbService _db;

    public IndexedDbBudgetRepository(IndexedDbService db) => _db = db;

    public async Task<Budget?> GetForMonthAsync(int year, int month) =>
        (await _db.GetAllAsync<Budget>(Store)).FirstOrDefault(b => b.Year == year && b.Month == month);

    public async Task SetForMonthAsync(int year, int month, decimal amount)
    {
        var existing = await GetForMonthAsync(year, month);
        var budget = existing ?? new Budget { Year = year, Month = month };
        budget.Amount = amount;
        var id = await _db.PutAsync(Store, budget);
        budget.Id = id; // put() now returns the real autoIncrement key when Id was 0
    }

    public async Task<List<Budget>> GetAllAsync() =>
        (await _db.GetAllAsync<Budget>(Store)).ToList();
}
