using Gasta.Shared.Models;

namespace Gasta.Shared.Data.Repositories;

public interface IExpenseRepository
{
    Task<List<Expense>> GetAllAsync();
    Task<Expense?> GetByIdAsync(int id);
    Task<int> SaveAsync(Expense expense);
    Task DeleteAsync(int id);
}
