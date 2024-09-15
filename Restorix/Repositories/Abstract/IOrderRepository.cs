using Restorix.Models;

namespace Restorix.Repositories.Abstract
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetActiveOrderForTableAsync(int tableId);

    }
}
