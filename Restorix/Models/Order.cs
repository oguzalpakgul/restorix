using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Restorix.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TableId { get; set; }

        [ForeignKey("TableId")]
        public Table Table { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false; 

        public DateTime? ClosedAt { get; set; }

    }

}
