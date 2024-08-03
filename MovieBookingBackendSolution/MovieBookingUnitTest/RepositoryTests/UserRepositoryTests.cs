using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.User;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTest.RepositoryTests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private MovieBookingContext _context;
        private UserRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new UserRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
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

            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        [Test, Order(1)]
        public async Task AddUser_Success()
        {
            var hmac = new HMACSHA512();
            var newUser = new User
            {
                Name = "New User",
                Email = "new.user@example.com",
                Status = UserStatus.Active,
                Role = UserRole.User,
                Phone = "1112223333",
                PasswordHashKey = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("newuser123"))
            };

            var addedUser = await _repository.Add(newUser);

            Assert.NotNull(addedUser);
            Assert.AreEqual(newUser.Email, addedUser.Email);
            Assert.AreEqual(3, _context.Users.Count());
        }

        [Test, Order(2)]
        public void AddUser_Failure_NullUser()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.Add(null));
        }

        [Test, Order(3)]
        public async Task GetAll_Success()
        {
            var users = await _repository.GetAll();
            Assert.AreEqual(7, users.Count()); // Adjusted to the seed count
        }

        [Test, Order(4)]
        public async Task GetAll_Failure_NoUsers()
        {
            _context.Users.RemoveRange(_context.Users);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<NoUsersFoundException>(async () => await _repository.GetAll());
        }

        [Test, Order(5)]
        public async Task GetById_Success()
        {
            var user = await _repository.GetById(1); // Use an existing ID

            Assert.NotNull(user);
        }

        [Test, Order(6)]
        public void GetById_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchUserException>(async () => await _repository.GetById(-1)); // Use a non-existing ID
        }

        [Test, Order(7)]
        public async Task UpdateUser_Success()
        {
            var user = await _repository.GetById(1);
            user.Name = "Updated Name";

            var updatedUser = await _repository.Update(user);

            Assert.NotNull(updatedUser);
        }

        [Test, Order(8)]
        public void UpdateUser_Failure_NotFound()
        {
            var user = new User
            {
                Id = 100,
                Name = "Nonexistent User"
            };

            Assert.ThrowsAsync<NoSuchUserException>(async () => await _repository.Update(user));
        }

        [Test, Order(9)]
        public async Task DeleteUser_Success()
        {
            var userToDelete = await _repository.GetById(1); // Use an existing ID
            var deletedUser = await _repository.Delete(userToDelete.Id);

            Assert.NotNull(deletedUser);
        }

        [Test, Order(10)]
        public void DeleteUser_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchUserException>(async () => await _repository.Delete(100)); // Use a non-existing ID
        }

        [Test, Order(11)]
        public async Task DeleteRange_Success()
        {
            var userIds = _context.Users.Select(u => u.Id).ToList(); // Get existing IDs
            var result = await _repository.DeleteRange(userIds);

            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.Users.Count());
        }

        [Test, Order(12)]
        public void DeleteRange_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchUserException>(async () => await _repository.DeleteRange(new List<int> { 100, 101 })); // Use non-existing IDs
        }
    }
}
