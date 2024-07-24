namespace MovieBookingBackend.Models
{
    public class Showtime
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MovieId { get; set; }
        public int TheatreId { get; set; }
        public int TotalSeats { get; set; }
        public float TicketPrice { get; set; }

        public Movie Movie { get; set; }
        public Theatre Theatre { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
