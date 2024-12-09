using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
	public class Product
	{
		public int? ProductId { get; set; }

		[Required, StringLength(100)]
		public string ProductName { get; set; } = string.Empty;
		public int CategoryId { get; set; }

		[Required]
		public double Price { get; set; }
		public double SalePrice { get; set; }
		public string? Image { get; set; }
		public int SupplierId { get; set; }
		public string? Description { get; set; }
		[Required]
		public int Stock { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
    }
}
