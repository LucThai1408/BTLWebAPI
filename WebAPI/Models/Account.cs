using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }


        [Required, MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [NotMapped]
        [Compare("Password", ErrorMessage = "Confirmation password does not match")]
        public string ConfirmPassword { get; set; } = string.Empty;


        [Required]
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;
        public string Role { get; set; } = "user";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
