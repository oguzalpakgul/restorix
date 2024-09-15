using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Restorix.Models
{
    public class Table
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Masa adı gereklidir.")]
        [StringLength(20, ErrorMessage = "Masa adı en fazla 20 karakter olmalıdır.")]
        public string Name { get; set; }

        public bool IsOccupied { get; set; }

        public int? CurrentOrderId { get; set; }

        [ForeignKey("CurrentOrderId")]
        public Order? CurrentOrder { get; set; }

        public Table()
        {
            this.IsOccupied = false;
        }
    }
}
