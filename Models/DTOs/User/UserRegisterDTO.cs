using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.User
{
    public class UserRegisterDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        public string Phone { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
