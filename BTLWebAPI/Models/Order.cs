using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BTLWebAPI.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        [Required]
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;



        [ForeignKey("UserId")]
        public Account? Accounts { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
