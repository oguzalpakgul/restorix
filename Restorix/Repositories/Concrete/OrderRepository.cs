using Microsoft.EntityFrameworkCore;
using Restorix.Data;
using Restorix.Models;
using Restorix.Repositories.Abstract;

namespace Restorix.Repositories.Concrete
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {

        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;

        }

        public async Task<Order> GetActiveOrderForTableAsync(int tableId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.TableId == tableId && o.IsCompleted == false);
        }
    }
}
