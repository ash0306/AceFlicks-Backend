using MovieBookingBackend.Models.Enums;

namespace MovieBookingBackend.Models.DTOs.Bookings
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public DateTime BookingTime { get; set; }
        public float TotalPrice { get; set; }
        public string Status { get; set; }
        public string? OfferMessage { get; set; }
        public ICollection<string> ShowtimeDetails { get; set; }
        public ICollection<string> Seats { get; set; }
    }
}
