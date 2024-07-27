using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Theatres
{
    public class TheatreDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Location { get; set; }
    }
}
