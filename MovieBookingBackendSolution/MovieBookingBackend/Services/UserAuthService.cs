using AutoMapper;
using MovieBookingBackend.Exceptions;
using MovieBookingBackend.Exceptions.Auth;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthService"/> class.
        /// </summary>
        /// <param name="repository">The repository instance used for user data access.</param>
        /// <param name="tokenService">The token service for generating JWT tokens.</param>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        /// <param name="logger">The logger instance for logging operations.</param>
        public UserAuthService(IRepository<int, User> repository, ITokenService tokenService, IMapper mapper, ILogger<UserAuthService> logger)
        {
            _repository = repository;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="userLoginDTO">The login details of the user.</param>
        /// <returns>A <see cref="UserLoginReturnDTO"/> containing the user's email and JWT token.</returns>
        /// <exception cref="NoSuchUserException">Thrown if the user does not exist.</exception>
        /// <exception cref="UserNotActiveException">Thrown if the user is not active.</exception>
        /// <exception cref="UnauthorizedUserException">Thrown if the email or password is incorrect.</exception>
        /// <exception cref="UnableToLoginException">Thrown if an error occurs during login.</exception>
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
                if(user.Status == UserStatus.Inactive)
                {
                    _logger.LogCritical("User is not verified");
                    throw new UserNotActiveException($"User is not verified. Please verify your email to continue");
                }

                HMACSHA512 hMACSHA512 = new HMACSHA512(user.PasswordHashKey);
                var encryptedPassword = hMACSHA512.ComputeHash(Encoding.UTF8.GetBytes(userLoginDTO.Password));
                bool isCorrectPassword = ComparePassword(encryptedPassword, user.PasswordHash);

                if (isCorrectPassword)
                {
                    UserLoginReturnDTO returnDTO = new UserLoginReturnDTO()
                    {
                        Email = userLoginDTO.Email,
                        Role = user.Role.ToString(),
                        Token = await _tokenService.GetUserToken(user)
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

        /// <summary>
        /// Compares the given encrypted password with the stored password hash.
        /// </summary>
        /// <param name="encryptedPassword">The encrypted password to compare.</param>
        /// <param name="password">The stored password hash.</param>
        /// <returns>True if the passwords match; otherwise, false.</returns>
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

        /// <summary>
        /// Registers a new user and returns the user's details.
        /// </summary>
        /// <param name="userRegisterDTO">The registration details of the user.</param>
        /// <returns>A <see cref="UserDTO"/> containing the registered user's details.</returns>
        /// <exception cref="UserAlreadyExistsException">Thrown if a user with the given email already exists.</exception>
        /// <exception cref="UnableToRegisterException">Thrown if an error occurs during registration.</exception>
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

        /// <summary>
        /// Registers a new admin user and returns the user's details.
        /// </summary>
        /// <param name="userRegisterDTO">The registration details of the admin user.</param>
        /// <returns>A <see cref="UserDTO"/> containing the registered admin user's details.</returns>
        /// <exception cref="UserAlreadyExistsException">Thrown if a user with the given email already exists.</exception>
        /// <exception cref="UnableToRegisterException">Thrown if an error occurs during registration.</exception>
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

        /// <summary>
        /// Updates the password for an existing user.
        /// </summary>
        /// <param name="userLoginDTO">The login details including the new password.</param>
        /// <returns>A <see cref="UserDTO"/> containing the updated user's details.</returns>
        /// <exception cref="NoSuchUserException">Thrown if the user does not exist.</exception>
        /// <exception cref="UnableToUpdateUserException">Thrown if an error occurs during password update.</exception>
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
