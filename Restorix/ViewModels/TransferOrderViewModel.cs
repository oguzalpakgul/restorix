using Restorix.Models;

namespace Restorix.ViewModels
{
    public class TransferOrderViewModel
    {
        public Order Order { get; set; }
        public IEnumerable<Table> AvailableTables { get; set; }
    }
}
