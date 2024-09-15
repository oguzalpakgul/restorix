using Microsoft.EntityFrameworkCore;
using Restorix.Data;
using Restorix.Models;
using Restorix.Repositories.Abstract;

namespace Restorix.Repositories.Concrete
{
    public class TableRepository : Repository<Table>, ITableRepository
    {
        private readonly AppDbContext _context;
        public TableRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Table>> GetAvailableTablesAsync()
        {
            return await _context.Tables
                .Where(t => !t.IsOccupied)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

    }
}
