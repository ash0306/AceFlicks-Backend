using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Validations;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Bookings
{
    public class BookingStatusDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [EnumValidation(typeof(BookingStatus))]
        public string Status { get; set; }
    }
}
