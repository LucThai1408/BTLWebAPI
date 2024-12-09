using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTLWebAPI.Models
{
    [Table("Blogs")]
    public class Blog
    {
        [Key]
        public int BlogId { get; set; } 

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string? Author { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ngày tạo

        public DateTime? UpdatedAt { get; set; } 
        public string? Thumbnail { get; set; } // Ảnh đại diện (tùy chọn)
    }
}

