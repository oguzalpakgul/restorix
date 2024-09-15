using Restorix.Models;

namespace Restorix.Repositories.Abstract
{
    public interface ITableRepository : IRepository<Table>
    {
        Task<IEnumerable<Table>> GetAvailableTablesAsync();
    }
}
