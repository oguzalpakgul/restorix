using Restorix.Data;
using Restorix.Models;
using Restorix.Repositories.Abstract;

namespace Restorix.Repositories.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IProductRepository Products { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public ITableRepository Tables { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IOrderItemRepository OrderItems { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Products = new ProductRepository(context);
            Categories = new CategoryRepository(context);
            Tables = new TableRepository(context);
            Orders = new OrderRepository(context);
            OrderItems = new OrderItemRepository(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
