using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MovieBookingBackend.Contexts;
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
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MovieBookingUnitTest.ServiceTests
{
    public class UserServiceTests
    {
        private MovieBookingContext _context;
        private IRepository<int, User> _repository;
        private IUserService _userService;
        private IMapper _mapper;
        private Mock<ILogger<UserService>> _logger;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new UserRepository(_context);
            _logger = new Mock<ILogger<UserService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<UpdateUserDTO, User>();
            });
            _mapper = config.CreateMapper();

            _userService = new UserService(_repository, _logger.Object, _mapper);

            var hmac = new HMACSHA512();
            // Seed initial data
            var users = new List<User>
            {
                new User {
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Status = UserStatus.Active,
                    Role = UserRole.User,
                    Phone = "1234567890",
                    PasswordHashKey = hmac.Key,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("johndoe123"))
                },
                new User {
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    Status = UserStatus.Active,
                    Role = UserRole.Admin,
                    Phone = "0987654321",
                    PasswordHashKey = hmac.Key,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("janesmith123"))
                }
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
        public async Task GetAllAdminUserDetails_ReturnsAdminUsers()
        {
            var result = await _userService.GetAllAdminUserDetails();
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void GetAllAdminUserDetails_NoAdminUsersFound_ThrowsNoUsersFoundException()
        {
            _context.Users.RemoveRange(_context.Users.Where(u => u.Role == UserRole.Admin));
            _context.SaveChanges();

            Assert.ThrowsAsync<NoUsersFoundException>(() => _userService.GetAllAdminUserDetails());
        }

        [Test]
        public async Task GetAllUsers_ReturnsAllUsers()
        {
            var result = await _userService.GetAllUsers();
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAllUsers_NoUsersFound_ThrowsNoUsersFoundException()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            Assert.ThrowsAsync<NoUsersFoundException>(() => _userService.GetAllUsers());
        }

        [Test]
        public async Task GetAllUserDetails_ReturnsRegularUsers()
        {
            var result = await _userService.GetAllUserDetails();
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void GetAllUserDetails_NoRegularUsersFound_ThrowsNoUsersFoundException()
        {
            _context.Users.RemoveRange(_context.Users.Where(u => u.Role == UserRole.User));
            _context.SaveChanges();

            Assert.ThrowsAsync<NoUsersFoundException>(() => _userService.GetAllUserDetails());
        }

        [Test]
        public async Task GetUserByEmail_ReturnsUser_WhenUserExists()
        {
            var result = await _userService.GetUserByEmail("john.doe@example.com");
            Assert.AreEqual("john.doe@example.com", result.Email);
        }

        [Test]
        public void GetUserByEmail_NoUserFound_ThrowsNoSuchUserException()
        {
            Assert.ThrowsAsync<NoSuchUserException>(() => _userService.GetUserByEmail("nonexistent@example.com"));
        }

        [Test]
        public async Task GetUserById_ReturnsUser_WhenUserExists()
        {
            var result = await _userService.GetUserById(1);
            Assert.AreEqual(1, result.Id);
        }

        [Test]
        public void GetUserById_NoUserFound_ThrowsNoSuchUserException()
        {
            Assert.ThrowsAsync<NoSuchUserException>(() => _userService.GetUserById(99));
        }

        [Test]
        public async Task UpdateUser_UpdatesUserDetailsSuccessfully()
        {
            var updateUserDTO = new UpdateUserDTO
            {
                Id = 2,
                PhoneNumber = "9876543210"
            };

            var result = await _userService.UpdateUser(updateUserDTO);
            Assert.AreEqual("9876543210", result.Phone);
        }

        [Test]
        public void UpdateUser_NoUserFound_ThrowsNoSuchUserException()
        {
            var updateUserDTO = new UpdateUserDTO
            {
                Id = 99,
                PhoneNumber = "9876543210"
            };

            Assert.ThrowsAsync<NoSuchUserException>(() => _userService.UpdateUser(updateUserDTO));
        }
    }
}
