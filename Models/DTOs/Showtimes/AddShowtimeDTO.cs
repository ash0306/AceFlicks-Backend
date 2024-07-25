using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Showtimes
{
    public class AddShowtimeDTO
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public int TheatreId { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        [Required]
        public float TicketPrice { get; set; }
    }
}
