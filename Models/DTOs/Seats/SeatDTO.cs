using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Validations;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Seats
{
    public class SeatDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Row { get; set; }
        [Required]
        public int SeatNumber { get; set; }
        [Required]
        [EnumValidation(typeof(SeatStatus))]
        public string SeatStatus { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Required]
        public int ShowetimeId { get; set; }
        public int? BookingId { get; set; }
    }
}
