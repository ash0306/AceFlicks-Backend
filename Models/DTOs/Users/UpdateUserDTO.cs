using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Users
{
    public class UpdateUserDTO
    {
        [Required]
        public int Id { get; set; }
        

        [EmailAddress]
        public string Email { get; set; }
        

        [MinLength(10)]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
    }
}
