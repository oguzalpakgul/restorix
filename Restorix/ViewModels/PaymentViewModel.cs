using System.Collections.Generic;
using Restorix.Models;

namespace Restorix.ViewModels
{
    public class PaymentViewModel
    {
        public Table Table { get; set; }
        public Order Order { get; set; }
        public string PaymentMethod { get; set; }
    }
}