using MovieBookingBackend.Models.DTOs.Bookings;

namespace MovieBookingBackend.Interfaces
{
    public interface IEmailVerificationService
    {
        Task CreateEmailVerification(int userId);
        Task<bool> VerifyEmail(int userId, string verificationCode);
        Task SendOfferCodeEmail(int userId, string offerCode);
        Task SendBookingConfirmationEmail(int userId, BookingDTO bookingDTO, byte[] bookingQR);
    }
}
