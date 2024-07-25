using MovieBookingBackend.Models.DTOs.Users;

namespace MovieBookingBackend.Interfaces
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDTO>> GetAllUsers();
        public Task<IEnumerable<UserDTO>> GetAllAdminUserDetails();
        public Task<IEnumerable<UserDTO>> GetAllUserDetails();
        public Task<UserDTO> GetUserById(int id);
        public Task<UserDTO> GetUserByEmail(string email);
        public Task<UserDTO> UpdateUser(UpdateUserDTO updateUserDTO);
    }
}
