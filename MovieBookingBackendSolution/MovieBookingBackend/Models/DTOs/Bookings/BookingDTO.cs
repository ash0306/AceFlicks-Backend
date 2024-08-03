using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Validations;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Bookings
{
    public class BookingDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ShowtimeId { get; set; }

        [Required]
        public DateTime BookingTime { get; set; }

        [Required]
        public float TotalPrice { get; set; }

        [Required]
        [EnumValidation(typeof(BookingStatus))]
        public string Status { get; set; }
        public string? OfferMessage { get; set; }

        [Required]
        public ICollection<string> ShowtimeDetails { get; set; }

        [Required]
        public ICollection<string> Seats { get; set; }
    }
}
