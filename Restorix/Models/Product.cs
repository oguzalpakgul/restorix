using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Restorix.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olmalıdır.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Fiyat gereklidir.")]
        [Range(0.01, 10000, ErrorMessage = "Fiyat 0.01 ile 10000 arasında olmalıdır.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Kategori seçimi gereklidir.")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [StringLength(255)]
        public string? ImagePath { get; set; } 

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }

}
