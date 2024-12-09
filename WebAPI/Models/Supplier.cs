using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required, StringLength(100)]
        public string SupplierName { get; set; } = string.Empty;

        [MaxLength(15)]
        [Required]
        public string? Phone { get; set; }

        [MaxLength(255)]
        [Required]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
