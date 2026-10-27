using Gasta.Shared.Data.Repositories;
using Gasta.Shared.Models;

namespace Gasta.Maui.Data;

// Behaviour mirrors the IndexedDb* repositories exactly (same ordering, same "set for month"
// upserts, same Save-returns-the-id), so no page or service can tell the difference.

public class SqliteExpenseRepository : IExpenseRepository
{
    private readonly SqliteDatabase _db;
    public SqliteExpenseRepository(SqliteDatabase db) => _db = db;

    public async Task<List<Expense>> GetAllAsync()
    {
        var c = await _db.ConnectionAsync();
        return (await c.Table<ExpenseRow>().OrderBy(r => r.Id).ToListAsync()).Select(r => RowMap.ToModel(r)).ToList();
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        var c = await _db.ConnectionAsync();
        var row = await c.FindAsync<ExpenseRow>(id);
        return row is null ? null : RowMap.ToModel(row);
    }

    public async Task<int> SaveAsync(Expense expense)
    {
        var row = RowMap.ToRow(expense);
        await _db.UpsertAsync(row);
        expense.Id = row.Id;
        return row.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var c = await _db.ConnectionAsync();
        await c.DeleteAsync<ExpenseRow>(id);
    }
}

public class SqliteCategoryRepository : ICategoryRepository
{
    private readonly SqliteDatabase _db;
    public SqliteCategoryRepository(SqliteDatabase db) => _db = db;

    public async Task<List<Category>> GetActiveOrderedAsync()
    {
        var c = await _db.ConnectionAsync();
        var rows = await c.Table<CategoryRow>().OrderBy(r => r.Id).ToListAsync();
        return rows.Select(r => RowMap.ToModel(r)).OrderBy(x => x.SortOrder).ToList();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var c = await _db.ConnectionAsync();
        var row = await c.FindAsync<CategoryRow>(id);
        return row is null ? null : RowMap.ToModel(row);
    }

    public async Task<int> SaveAsync(Category category)
    {
        var row = RowMap.ToRow(category);
        await _db.UpsertAsync(row);
        category.Id = row.Id;
        return row.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var c = await _db.ConnectionAsync();
        await c.DeleteAsync<CategoryRow>(id);
    }
}

public class SqlitePaymentMethodRepository : IPaymentMethodRepository
{
    private readonly SqliteDatabase _db;
    public SqlitePaymentMethodRepository(SqliteDatabase db) => _db = db;

    public async Task<List<PaymentMethod>> GetActiveOrderedAsync()
    {
        var c = await _db.ConnectionAsync();
        var rows = await c.Table<PaymentMethodRow>().OrderBy(r => r.Id).ToListAsync();
        return rows.Select(r => RowMap.ToModel(r)).OrderBy(x => x.SortOrder).ToList();
    }

    public async Task<PaymentMethod?> GetByIdAsync(int id)
    {
        var c = await _db.ConnectionAsync();
        var row = await c.FindAsync<PaymentMethodRow>(id);
        return row is null ? null : RowMap.ToModel(row);
    }

    public async Task<int> SaveAsync(PaymentMethod method)
    {
        var row = RowMap.ToRow(method);
        await _db.UpsertAsync(row);
        method.Id = row.Id;
        return row.Id;
    }
}

public class SqliteBudgetRepository : IBudgetRepository
{
    private readonly SqliteDatabase _db;
    public SqliteBudgetRepository(SqliteDatabase db) => _db = db;

    public async Task<Budget?> GetForMonthAsync(int year, int month)
    {
        var c = await _db.ConnectionAsync();
        var row = await c.Table<BudgetRow>().Where(b => b.Year == year && b.Month == month).FirstOrDefaultAsync();
        return row is null ? null : RowMap.ToModel(row);
    }

    public async Task SetForMonthAsync(int year, int month, decimal amount)
    {
        var budget = await GetForMonthAsync(year, month) ?? new Budget { Year = year, Month = month };
        budget.Amount = amount;
        var row = RowMap.ToRow(budget);
        await _db.UpsertAsync(row);
        budget.Id = row.Id;
    }

    public async Task<List<Budget>> GetAllAsync()
    {
        var c = await _db.ConnectionAsync();
        return (await c.Table<BudgetRow>().OrderBy(r => r.Id).ToListAsync()).Select(r => RowMap.ToModel(r)).ToList();
    }
}

public class SqliteCategoryBudgetRepository : ICategoryBudgetRepository
{
    private readonly SqliteDatabase _db;
    public SqliteCategoryBudgetRepository(SqliteDatabase db) => _db = db;

    public async Task<List<CategoryBudget>> GetAllForMonthAsync(int year, int month)
    {
        var c = await _db.ConnectionAsync();
        var rows = await c.Table<CategoryBudgetRow>().Where(b => b.Year == year && b.Month == month).OrderBy(r => r.Id).ToListAsync();
        return rows.Select(r => RowMap.ToModel(r)).ToList();
    }

    public async Task<CategoryBudget?> GetForCategoryMonthAsync(int categoryId, int year, int month)
    {
        var c = await _db.ConnectionAsync();
        var row = await c.Table<CategoryBudgetRow>()
            .Where(b => b.CategoryId == categoryId && b.Year == year && b.Month == month).FirstOrDefaultAsync();
        return row is null ? null : RowMap.ToModel(row);
    }

    public async Task SetForCategoryMonthAsync(int categoryId, int year, int month, decimal amount)
    {
        var budget = await GetForCategoryMonthAsync(categoryId, year, month)
                     ?? new CategoryBudget { CategoryId = categoryId, Year = year, Month = month };
        budget.Amount = amount;
        var row = RowMap.ToRow(budget);
        await _db.UpsertAsync(row);
        budget.Id = row.Id;
    }

    public async Task<List<CategoryBudget>> GetAllAsync()
    {
        var c = await _db.ConnectionAsync();
        return (await c.Table<CategoryBudgetRow>().OrderBy(r => r.Id).ToListAsync()).Select(r => RowMap.ToModel(r)).ToList();
    }
}
