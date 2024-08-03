using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Validations;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Seats
{
    public class UpdateSeatDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [EnumValidation(typeof(SeatStatus))]
        public string SeatStatus { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        public int? BookingId { get; set; }
    }
}
