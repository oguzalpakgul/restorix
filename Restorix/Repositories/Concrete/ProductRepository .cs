using Restorix.Data;
using Restorix.Models;
using Restorix.Repositories.Abstract;

namespace Restorix.Repositories.Concrete
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

   
    }
}
