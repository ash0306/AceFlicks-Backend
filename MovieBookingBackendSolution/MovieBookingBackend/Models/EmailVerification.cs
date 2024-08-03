using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models
{
    public class EmailVerification
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string? VerificationCode { get; set; }

        public DateTime ExpiryDate { get; set; }

        public User? User { get; set; }
    }
}
