using MovieBookingBackend.Models;

namespace MovieBookingBackend.Interfaces
{
    public interface IEmailVerificationRepository : IRepository<int, EmailVerification>
    {
        Task<EmailVerification> GetByUserId(int userId);
    }
}
