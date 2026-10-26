using Gasta.Shared.Models;

namespace Gasta.Shared.Data.Repositories;

public interface ICategoryBudgetRepository
{
    Task<List<CategoryBudget>> GetAllForMonthAsync(int year, int month);

    Task<CategoryBudget?> GetForCategoryMonthAsync(int categoryId, int year, int month);

    Task SetForCategoryMonthAsync(int categoryId, int year, int month, decimal amount);

    /// <summary>Every category budget ever set, across all months and categories.
    /// Used by the Data & Backup export for full budget history.</summary>
    Task<List<CategoryBudget>> GetAllAsync();
}