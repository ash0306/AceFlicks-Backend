using MovieBookingBackend.Models.Enums;

namespace MovieBookingBackend.Models.DTOs.Bookings
{
    public class AddBookingDTO
    {
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public float TotalPrice { get; set; }
        public IList<int> Seats { get; set; }
    }
}
