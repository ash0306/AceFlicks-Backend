using AutoMapper;
using MovieBookingBackend.Exceptions;
using MovieBookingBackend.Exceptions.User;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Users;
using MovieBookingBackend.Models.Enums;
using System.Security.Cryptography;
using System.Text;

namespace MovieBookingBackend.Services
{
    public class UserAuthService : IUserAuthService
    {
        private readonly IRepository<int, User> _repository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAuthService> _logger;

        public UserAuthService(IRepository<int, User> repository, ITokenService tokenService, IMapper mapper, ILogger<UserAuthService> logger)
        {
            _repository = repository;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<UserLoginReturnDTO> Login(UserLoginDTO userLoginDTO)
        {
            User user;
            try
            {
                user = (await _repository.GetAll()).FirstOrDefault(u => u.Email == userLoginDTO.Email);
                if (user == null)
                {
                    _logger.LogCritical("User does not exist");
                    throw new NoSuchUserException($"No user with Email {userLoginDTO.Email} found");
                }

                HMACSHA512 hMACSHA512 = new HMACSHA512(user.PasswordHashKey);
                var encryptedPassword = hMACSHA512.ComputeHash(Encoding.UTF8.GetBytes(userLoginDTO.Password));
                bool isCorrectPassword = ComparePassword(encryptedPassword, user.PasswordHash);

                if (isCorrectPassword)
                {
                    UserLoginReturnDTO returnDTO = new UserLoginReturnDTO()
                    {
                        Email = userLoginDTO.Email,
                        Token = _tokenService.GetUserToken(user)
                    };
                    return returnDTO;
                }
                _logger.LogCritical("Could not Login");
                throw new UnauthorizedUserException("Invalid email or password");
            }
            catch(Exception ex)
            {
                _logger.LogCritical("Unable to login");
                throw new UnableToLoginException($"Unable to login user!! {ex.Message}");
            }
        }

        private bool ComparePassword(byte[] encryptedPassword, byte[] password)
        {
            for (int i = 0; i < encryptedPassword.Length; i++)
            {
                if (encryptedPassword[i] != password[i])
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<UserDTO> Register(UserRegisterDTO userRegisterDTO)
        {
            User user;

            try
            {
                var alreadyPresent = (await _repository.GetAll()).FirstOrDefault(u => u.Email == userRegisterDTO.Email);
                if (alreadyPresent != null)
                {
                    _logger.LogCritical("User with email already exists");
                    throw new UserAlreadyExistsException($"User with Email {userRegisterDTO.Email} already exists");
                }
                user = _mapper.Map<User>(userRegisterDTO);
                HMACSHA512 hMACSHA = new HMACSHA512();

                user.PasswordHashKey = hMACSHA.Key;
                user.PasswordHash = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(userRegisterDTO.Password));
                user.Role = UserRole.User;
                var newUser = await _repository.Add(user);

                UserDTO userDTO = _mapper.Map<UserDTO>(newUser);

                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unable to register user!!");
                throw new UnableToRegisterException("Unable to register at the moment!!. Plaese try again later.");
            }
        }

        public async Task<UserDTO> RegisterAdmin(UserRegisterDTO userRegisterDTO)
        {
            User user;

            try
            {
                var alreadyPresent = (await _repository.GetAll()).FirstOrDefault(u => u.Email == userRegisterDTO.Email);
                if (alreadyPresent != null)
                {
                    _logger.LogCritical("User with email already exists");
                    throw new UserAlreadyExistsException($"User with Email {userRegisterDTO.Email} already exists");
                }
                user = _mapper.Map<User>(userRegisterDTO);
                user.Role = UserRole.Admin;
                HMACSHA512 hMACSHA = new HMACSHA512();

                user.PasswordHashKey = hMACSHA.Key;
                user.PasswordHash = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(userRegisterDTO.Password));
                var newUser = await _repository.Add(user);

                UserDTO userDTO = _mapper.Map<UserDTO>(newUser);

                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unable to register user!!");
                throw new UnableToRegisterException("Unable to register at the moment!!. Plaese try again later.");
            }
        }

        public async Task<UserDTO> UpdatePassword(UserLoginDTO userLoginDTO)
        {
            User user;
            try
            {
                user = (await _repository.GetAll()).FirstOrDefault(u => u.Email == userLoginDTO.Email);
                if (user == null)
                {
                    _logger.LogCritical("User does not exist");
                    throw new NoSuchUserException($"No user with Email {userLoginDTO.Email} found");
                }

                HMACSHA512 hMACSHA512 = new HMACSHA512(user.PasswordHashKey);
                var encryptedPassword = hMACSHA512.ComputeHash(Encoding.UTF8.GetBytes(userLoginDTO.Password));

                user.PasswordHash = encryptedPassword;

                var newUser = await _repository.Update(user);
                UserDTO userDTO = _mapper.Map<UserDTO>(newUser);
                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unable to update password");
                throw new UnableToUpdateUserException($"Unable to update password for user with Email {userLoginDTO.Email}");
            }
        }
    }
}
