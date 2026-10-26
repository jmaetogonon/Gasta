using Gasta.Shared.Models;

namespace Gasta.Shared.Data.Repositories;

public interface IBudgetRepository
{
    Task<Budget?> GetForMonthAsync(int year, int month);
    Task SetForMonthAsync(int year, int month, decimal amount);

    /// <summary>Every monthly budget ever set, across all months. Used by the
    /// Data & Backup export so a JSON backup captures full budget history, not
    /// just the current month.</summary>
    Task<List<Budget>> GetAllAsync();
}
