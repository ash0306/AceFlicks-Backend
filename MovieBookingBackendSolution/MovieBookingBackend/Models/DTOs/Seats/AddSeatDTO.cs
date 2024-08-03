using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Validations;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Seats
{
    public class AddSeatDTO
    {
        [Required]
        public string Row { get; set; }
        [Required]
        public int SeatNumber { get; set; }
        [Required]
        [EnumValidation(typeof(SeatStatus))]
        public string SeatStatus { get; set; }
        [Required]
        public int ShowetimeId { get; set; }
    }
}
