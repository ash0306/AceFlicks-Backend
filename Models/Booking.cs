using MovieBookingBackend.Models.Enums;

namespace MovieBookingBackend.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public DateTime BookingTime { get; set; } = DateTime.Now;
        public float TotalPrice { get; set; }
        public BookingStatus Status { get; set; }

        public User User { get; set; }
        public Showtime Showtime { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
