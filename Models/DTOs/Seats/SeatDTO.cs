using MovieBookingBackend.Models.Enums;

namespace MovieBookingBackend.Models.DTOs.Seats
{
    public class SeatDTO
    {
        public int Id { get; set; }
        public string Row { get; set; }
        public int SeatNumber { get; set; }
        public string SeatStatus { get; set; }
        public bool IsAvailable { get; set; }
        public int ShowetimeId { get; set; }
        public int? BookingId { get; set; }
    }
}
