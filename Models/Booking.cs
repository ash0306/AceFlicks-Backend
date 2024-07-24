namespace MovieBookingBackend.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ShowtimeId { get; set; }
        public DateTime BookingTime { get; set; }
        public float TotalPrice { get; set; }
        public Showtime Showtime { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
