using System.ComponentModel.DataAnnotations;

namespace Restorix.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı gereklidir.")]
        [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olmalıdır.")]
        public string Name { get; set; }

        public List<Product>? Products { get; set; }
    }
}
