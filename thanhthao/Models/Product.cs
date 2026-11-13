using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace thanhthao.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100)]//product name is limited to 100 characters
        public string? ProductName { get; set; }
        [Range(0.01, 100000, ErrorMessage = "Product price must be greater than 0 and less than 100000")]
        public decimal ProductPrice { get; set; }
        // Foreign key to Category
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        // Navigation property
        public Category? Category { get; set; }
    }
}
