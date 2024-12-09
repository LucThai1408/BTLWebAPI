using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Category
    {
        public int? CategoryId { get; set; }

        [Required, StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Product>? Products { get; set; }
    }
}
