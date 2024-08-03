using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Showtimes
{
    public class UpdateShowtimeDTO
    {
        [Required]
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TotalSeats { get; set; }
        public float TicketPrice { get; set; }
    }
}
