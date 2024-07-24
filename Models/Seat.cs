namespace MovieBookingBackend.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public string Row {  get; set; }
        public int SeatNumber { get; set; }
        public bool IsAvailable { get; set; }
        public int ShowetimeId { get; set; }
        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }
        public Showtime Showtime { get; set; }
    }
}
