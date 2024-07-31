using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Models.Enums;

namespace MovieBookingBackend.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone {  get; set; }
        public UserStatus Status { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordHashKey { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}
