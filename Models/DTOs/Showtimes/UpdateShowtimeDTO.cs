namespace MovieBookingBackend.Models.DTOs.Showtimes
{
    public class UpdateShowtimeDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int TheatreId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TotalSeats { get; set; }
        public float TicketPrice { get; set; }
    }
}
