using Gasta.Shared.Models;

namespace Gasta.Shared.Data.Repositories;

public interface IPaymentMethodRepository
{
    Task<List<PaymentMethod>> GetActiveOrderedAsync();
    Task<PaymentMethod?> GetByIdAsync(int id);
    Task<int> SaveAsync(PaymentMethod method);
}
