using MovieBookingBackend.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Bookings
{
    public class AddBookingDTO
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ShowtimeId { get; set; }
        [Required]
        [MinLength(1)]
        public IList<int> Seats { get; set; }
        [Required]
        public float TotalPrice { get; set; }
    }
}
