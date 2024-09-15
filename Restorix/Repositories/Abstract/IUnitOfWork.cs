using Restorix.Models;

namespace Restorix.Repositories.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ITableRepository  Tables { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }

        Task<int> CompleteAsync();
    }
}
