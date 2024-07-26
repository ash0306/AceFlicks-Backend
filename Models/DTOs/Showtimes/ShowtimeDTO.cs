using MovieBookingBackend.Models;

namespace MovieBookingBackend.Models.DTOs.Showtimes
{
    public class ShowtimeDTO
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public int MovieId { get; set; }
        public int TheatreId { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public float TicketPrice { get; set; }
    }
}
