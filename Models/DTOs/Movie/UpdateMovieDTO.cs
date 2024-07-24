using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Movie
{
    public class UpdateMovieDTO
    {
        [Required]
        public string Title { get; set; }
        public int Duration { get; set; } //in minutes
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}
