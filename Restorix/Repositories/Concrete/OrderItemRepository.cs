using Restorix.Data;
using Restorix.Models;
using Restorix.Repositories.Abstract;

namespace Restorix.Repositories.Concrete
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(AppDbContext context) : base(context)
        {
        }


    }
}
