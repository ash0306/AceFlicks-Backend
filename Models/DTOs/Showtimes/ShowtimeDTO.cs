using MovieBookingBackend.Models;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Validations;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Showtimes
{
    public class ShowtimeDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        [EnumValidation(typeof(ShowtimeStatus))]
        public string Status { get; set; }
        [Required]
        public string Movie { get; set; }
        [Required]
        public string MoviePoster { get; set; }
        [Required]
        public string Theatre { get; set; }
        [Required]
        public int TotalSeats { get; set; }
        [Required]
        public int AvailableSeats { get; set; }
        [Required]
        public float TicketPrice { get; set; }
    }
}
