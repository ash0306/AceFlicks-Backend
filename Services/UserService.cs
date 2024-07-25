using AutoMapper;
using MovieBookingBackend.Exceptions.User;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Users;
using MovieBookingBackend.Models.Enums;

namespace MovieBookingBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _repository;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(IRepository<int, User> repository, ILogger<UserService> logger, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTO>> GetAllAdminUserDetails()
        {
            try
            {
                var users = (await _repository.GetAll()).Where(u => u.Role == UserRole.Admin);
                if (users.Count() <= 0)
                {
                    throw new NoUsersFoundException("No users found");
                }

                IList<UserDTO> userDTOs = new List<UserDTO>();
                foreach (var item in users)
                {
                    var result = _mapper.Map<UserDTO>(item);
                    userDTOs.Add(result);
                }
                return userDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No Users found. {ex}");
                throw new NoUsersFoundException($"No users found. {ex.Message}");
            }
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            try
            {
                var users = await _repository.GetAll();
                if(users.Count() <= 0)
                {
                    throw new NoUsersFoundException("No users found");
                }

                IList<UserDTO> userDTOs = new List<UserDTO>();
                foreach (var item in users)
                {
                    var result = _mapper.Map<UserDTO>(item);
                    userDTOs.Add(result);
                }
                return userDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No Users found. {ex}");
                throw new NoUsersFoundException($"No users found. {ex.Message}");
            }
        }

        public async Task<IEnumerable<UserDTO>> GetAllUserDetails()
        {
            try
            {
                var users = (await _repository.GetAll()).Where(u => u.Role == UserRole.User);
                if (users.Count() <= 0)
                {
                    throw new NoUsersFoundException("No users found");
                }

                IList<UserDTO> userDTOs = new List<UserDTO>();
                foreach (var item in users)
                {
                    var result = _mapper.Map<UserDTO>(item);
                    userDTOs.Add(result);
                }
                return userDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No Users found. {ex}");
                throw new NoUsersFoundException($"No users found. {ex.Message}");
            }
        }

        public async Task<UserDTO> GetUserByEmail(string email)
        {
            try
            {
                var user = (await _repository.GetAll()).FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    throw new NoSuchUserException($"No user with email {email} was found");
                }

                UserDTO userDTO = _mapper.Map<UserDTO>(user);
                
                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No User found. {ex}");
                throw new NoSuchUserException($"No user with email {email} found. {ex.Message}");
            }
        }

        public async Task<UserDTO> GetUserById(int id)
        {
            try
            {
                var user = (await _repository.GetAll()).FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    throw new NoSuchUserException($"No user with ID {id} was found");
                }

                UserDTO userDTO = _mapper.Map<UserDTO>(user);

                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No User found. {ex}");
                throw new NoSuchUserException($"No user with ID {id} found. {ex.Message}");
            }
        }

        public async Task<UserDTO> UpdateUser(UpdateUserDTO updateUserDTO)
        {
            try
            {
                var user = (await _repository.GetAll()).FirstOrDefault(u => u.Id == updateUserDTO.Id);
                if (user == null)
                {
                    throw new NoSuchUserException($"No user with ID {updateUserDTO.Id} was found");
                }

                if(!string.IsNullOrEmpty(updateUserDTO.Email))
                {
                    user.Email = updateUserDTO.Email;
                }
                if(!string.IsNullOrEmpty(updateUserDTO.PhoneNumber))
                {
                    user.Phone = updateUserDTO.PhoneNumber;
                }

                var result = _repository.Update(user);

                UserDTO userDTO = _mapper.Map<UserDTO>(result);

                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No User found. {ex}");
                throw new NoSuchUserException($"No user with ID {updateUserDTO.Id} found. {ex.Message}");
            }
        }
    }
}
