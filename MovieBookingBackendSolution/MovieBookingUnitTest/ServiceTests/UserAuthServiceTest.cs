using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Auth;
using MovieBookingBackend.Exceptions.User;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models.DTOs.Users;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using MovieBookingBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTest.ServiceTests
{
    public class UserAuthServiceTest
    {
        private MovieBookingContext _context;
        private IRepository<int, User> _repository;
        private IUserAuthService _userAuthService;
        private ITokenService _tokenService;
        private IMapper _mapper;
        private Mock<ILogger<UserAuthService>> _logger;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new UserRepository(_context);
            _tokenService = new Mock<ITokenService>().Object;
            _logger = new Mock<ILogger<UserAuthService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<UserRegisterDTO, User>();
            });
            _mapper = config.CreateMapper();

            _userAuthService = new UserAuthService(_repository, _tokenService, _mapper, _logger.Object);

            // Seed data
            var hmac = new HMACSHA512();
            var users = new List<User>
            {
                new User { Name = "ActiveUser", Email = "active@example.com", Phone="1234506789", Role = UserRole.User, Status = UserStatus.Active, PasswordHashKey = hmac.Key, PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")) },
                new User { Name = "InactiveUser", Email = "inactive@example.com", Phone="1234506789", Role = UserRole.User, Status = UserStatus.Inactive, PasswordHashKey = hmac.Key, PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")) }
            };
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Login_ValidUser_ReturnsUserLoginReturnDTO()
        {
            var userLoginDTO = new UserLoginDTO { Email = "active@example.com", Password = "password" };
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(x => x.GetUserToken(It.IsAny<User>())).ReturnsAsync("fake-jwt-token");
            _userAuthService = new UserAuthService(_repository, tokenServiceMock.Object, _mapper, _logger.Object);

            var result = await _userAuthService.Login(userLoginDTO);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Login_UserNotFound()
        {
            var userLoginDTO = new UserLoginDTO { Email = "nonexistent@example.com", Password = "password" };

            Assert.ThrowsAsync<UnableToLoginException>(() => _userAuthService.Login(userLoginDTO));
        }

        [Test]
        public void Login_UserNotActive()
        {
            var userLoginDTO = new UserLoginDTO { Email = "inactive@example.com", Password = "password" };

            Assert.ThrowsAsync<UnableToLoginException>(() => _userAuthService.Login(userLoginDTO));
        }

        [Test]
        public void Login_InvalidPassword_ThrowsUnauthorizedUserException()
        {
            var userLoginDTO = new UserLoginDTO { Email = "active@example.com", Password = "wrongpassword" };

            Assert.ThrowsAsync<UnableToLoginException>(() => _userAuthService.Login(userLoginDTO));
        }

        [Test]
        public async Task Register_ValidUser_ReturnsUserDTO()
        {
            var userRegisterDTO = new UserRegisterDTO { Name="New User", Phone="9988779988", Email = "newuser@example.com", Password = "newpassword" };

            var result = await _userAuthService.Register(userRegisterDTO);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Register_UserAlreadyExists()
        {
            var userRegisterDTO = new UserRegisterDTO { Email = "active@example.com", Password = "password" };

            Assert.ThrowsAsync<UnableToRegisterException>(() => _userAuthService.Register(userRegisterDTO));
        }

        [Test]
        public async Task RegisterAdmin_ValidAdmin_ReturnsUserDTO()
        {
            var userRegisterDTO = new UserRegisterDTO {Name="New Admin", Phone="1122334455", Email = "newadmin@example.com", Password = "newpassword" };

            var result = await _userAuthService.RegisterAdmin(userRegisterDTO);

            Assert.IsNotNull(result);
        }

        [Test]
        public void RegisterAdmin_UserAlreadyExists()
        {
            var userRegisterDTO = new UserRegisterDTO { Email = "active@example.com", Password = "password" };

            Assert.ThrowsAsync<UnableToRegisterException>(() => _userAuthService.RegisterAdmin(userRegisterDTO));
        }

        [Test]
        public async Task UpdatePassword_ValidUser_ReturnsUserDTO()
        {
            var userLoginDTO = new UserLoginDTO { Email = "active@example.com", Password = "newpassword" };

            var result = await _userAuthService.UpdatePassword(userLoginDTO);

            var hmac = new HMACSHA512(_context.Users.First(u => u.Email == "active@example.com").PasswordHashKey);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("newpassword"));

            Assert.IsNotNull(result);
        }

        [Test]
        public void UpdatePassword_UserNotFound()
        {
            var userLoginDTO = new UserLoginDTO { Email = "nonexistent@example.com", Password = "password" };

            Assert.ThrowsAsync<UnableToUpdateUserException>(() => _userAuthService.UpdatePassword(userLoginDTO));
        }
    }
}
