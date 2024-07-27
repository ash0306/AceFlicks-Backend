namespace MovieBookingBackend.Interfaces
{
    public interface IEmailVerificationService
    {
        Task CreateEmailVerification(int userId);
        Task<bool> VerifyEmail(int userId, string verificationCode);
    }
}
