using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Users
{
    public class UserLoginReturnDTO
    {
        [Required]
        public string Email { get; set; }
        [Required] 
        public string Role { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
