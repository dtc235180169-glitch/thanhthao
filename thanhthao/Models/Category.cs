using System.ComponentModel.DataAnnotations;

namespace thanhthao.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category name is required")]
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        //Navigation property
        public ICollection<Product>? Products { get; set; } //Or you can use List<Product>
    }
}
