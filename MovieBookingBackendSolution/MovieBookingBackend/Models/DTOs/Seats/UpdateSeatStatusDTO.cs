using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Validations;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Seats
{
    public class UpdateSeatStatusDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [EnumValidation(typeof(SeatStatus))]
        public string SeatStatus { get; set; }

        public int? BookingId { get; set; }
    }
}
