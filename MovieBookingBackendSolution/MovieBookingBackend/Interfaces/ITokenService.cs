using MovieBookingBackend.Models;

namespace MovieBookingBackend.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GetUserToken(User user);
    }
}
