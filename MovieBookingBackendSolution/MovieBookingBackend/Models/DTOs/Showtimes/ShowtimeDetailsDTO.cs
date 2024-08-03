using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Validations;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Showtimes
{
    public class ShowtimeDetailsDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public int MovieId { get; set; }
        [Required]
        public int TheatreId { get; set; }
    }
}
