namespace MovieBookingBackend.Models.DTOs.Showtimes
{
    public class ShowtimeGroupDTO
    {
        public string Name { get; set; }
        public ICollection<ShowtimeDTO> Showtimes { get; set; }
    }
}
