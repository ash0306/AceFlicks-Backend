using MovieBookingBackend.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Movies;

public class MovieDTO
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Synopsis { get; set; }

    [Required]
    public string Genre { get; set; }
    [Required] 
    public string Language { get; set; }

    [Required]
    public int Duration { get; set; } //in minutes

    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public string ImageUrl { get; set; }

    [Required]
    public string Status { get; set; }
}
