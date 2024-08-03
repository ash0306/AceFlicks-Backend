using MovieBookingBackend.Models.DTOs.Users;

namespace MovieBookingBackend.Interfaces
{
    public interface IUserAuthService
    {
        public Task<UserLoginReturnDTO> Login(UserLoginDTO userLoginDTO);
        public Task<UserDTO> Register(UserRegisterDTO userRegisterDTO);
        public Task<UserDTO> RegisterAdmin(UserRegisterDTO userRegisterDTO);
        public Task<UserDTO> UpdatePassword(UserLoginDTO userLoginDTO);
    }
}
