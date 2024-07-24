using MovieBookingBackend.Models;

namespace MovieBookingBackend.Interfaces
{
    public interface ITokenService
    {
        public string GetUserToken(User user);
    }
}
