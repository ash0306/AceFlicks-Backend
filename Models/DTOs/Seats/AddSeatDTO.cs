using MovieBookingBackend.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Seats
{
    public class AddSeatDTO
    {
        [Required]
        public string Row { get; set; }
        [Required]
        public int SeatNumber { get; set; }
        [Required]
        public string SeatStatus { get; set; }
        [Required]
        public int ShowetimeId { get; set; }
    }
}
