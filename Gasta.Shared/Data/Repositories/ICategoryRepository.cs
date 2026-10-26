using Gasta.Shared.Models;

namespace Gasta.Shared.Data.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetActiveOrderedAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<int> SaveAsync(Category category);
    Task DeleteAsync(int id);
}
