using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Validations;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Models.DTOs.Showtimes
{
    public class UpdateShowtimeStatusDTO
    {
        [Required]
        public int Id {  get; set; }
        [Required]
        [EnumValidation(typeof(ShowtimeStatus))]
        public string Status { get; set; }
    }
}
