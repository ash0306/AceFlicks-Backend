namespace MovieBookingBackend.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public string Genre { get; set; }
        public int Duration { get; set; } //in minutes
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public ICollection<Showtime> Showtimes { get; set; }
    }
}
