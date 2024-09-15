using Restorix.Models;

namespace Restorix.ViewModels
{
    public class OrderManagementViewModel
    {
        public Table Table { get; set; }
        public List<Product> Products { get; set; }
        public Order Order { get; set; }
    }
}
