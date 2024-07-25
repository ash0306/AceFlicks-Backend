using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Movies;

public class UpdateMovieDTO
{
    [Required]
    public string Title { get; set; }
    public int Duration { get; set; } //in minutes
    public DateTime? StartDate { get; set; } = DateTime.MinValue;
    public DateTime? EndDate { get; set; } = DateTime.MinValue;
    public string Status { get; set; }
}
