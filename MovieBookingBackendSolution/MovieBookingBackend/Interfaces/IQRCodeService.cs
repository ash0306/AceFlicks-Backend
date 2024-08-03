using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Bookings;

namespace MovieBookingBackend.Interfaces
{
    public interface IQRCodeService
    {
        Task<byte[]> CreateQRCode(BookingDTO bookingDTO);
        Task<byte[]> GetQRCodeByBookingId(int bookingId);
    }
}
