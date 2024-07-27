using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Theatres
{ 
    public class UpdateTheatreDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string OldLocation { get; set; }
        [Required]
        public string NewLocation { get; set; }
    }
}
