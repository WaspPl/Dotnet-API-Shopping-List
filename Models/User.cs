using System.ComponentModel.DataAnnotations;

namespace Zaliczeniowy4.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public required string Username { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public required string Email { get; set; }
        
        [Required]
        public required string PasswordHash { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
