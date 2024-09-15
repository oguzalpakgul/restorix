using Restorix.Data;
using Restorix.Models;
using Restorix.Repositories.Abstract;

namespace Restorix.Repositories.Concrete
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }


    }
}
