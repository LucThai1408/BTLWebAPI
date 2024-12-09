using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BTLWebAPI.Models
{
    public class ProductImage
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public double Price { get; set; }
        public double SalePrice { get; set; }
        public string? PictureOld { get; set; }
        public int SupplierId { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public IFormFile? Image {  get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
