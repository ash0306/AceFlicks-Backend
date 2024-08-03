using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Booking;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using MovieBookingBackend.Exceptions.Seat;

namespace MovieBookingUnitTest.RepositoryTests
{
    public class BookingRepositoryTest
    {
        private MovieBookingContext _context;
        private BookingRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestBookingDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new BookingRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var hmac = new HMACSHA512();
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

            var movies = new List<Movie>
            {
                new Movie { Title = "Movie 1", Synopsis = "Synopsis 1", Genre = "Action", Language = "English", ImageUrl = "url1", Duration = 120, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), Status = MovieStatus.Running },
                new Movie { Title = "Movie 2", Synopsis = "Synopsis 2", Genre = "Comedy", Language = "Hindi", ImageUrl = "url2", Duration = 100, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), Status = MovieStatus.NotRunning }
            };

            var theatres = new List<Theatre>
            {
                new Theatre { Name = "Theatre 1", Location = "Location 1" },
                new Theatre { Name = "Theatre 2", Location = "Location 2" }
            };

            var showtimes = new List<Showtime>
            {
                new Showtime { MovieId = 1, TheatreId = 1, StartTime = DateTime.Now.AddHours(1), EndTime = DateTime.Now.AddHours(3), TicketPrice=120, AvailableSeats=50, TotalSeats=50, Status=ShowtimeStatus.Active },
                new Showtime { MovieId = 2, TheatreId = 2, StartTime = DateTime.Now.AddHours(2), EndTime = DateTime.Now.AddHours(4), TicketPrice=120, AvailableSeats=50, TotalSeats=50, Status=ShowtimeStatus.Active }
            };

            var bookings = new List<Booking>
            {
                new Booking { UserId = 1, ShowtimeId = 1, TotalPrice = 10.0f, Status = BookingStatus.Booked },
                new Booking { UserId = 2, ShowtimeId = 2, TotalPrice = 15.0f, Status = BookingStatus.Reserved }
            };

            _context.Users.AddRange(users);
            _context.Movies.AddRange(movies);
            _context.Theatres.AddRange(theatres);
            _context.Showtimes.AddRange(showtimes);
            _context.Bookings.AddRange(bookings);

            _context.SaveChanges();
        }

        [Test, Order(1)]
        public async Task AddBooking_Success()
        {
            var booking = new Booking
            {
                UserId = 1,
                ShowtimeId = 1,
                TotalPrice = 20.0f,
                Status = BookingStatus.Booked
            };

            var addedBooking = await _repository.Add(booking);

            Assert.NotNull(addedBooking);
            Assert.AreEqual(booking.TotalPrice, addedBooking.TotalPrice);
            Assert.AreEqual(3, _context.Bookings.Count());
        }

        [Test, Order(2)]
        public void AddBooking_Failure_NullBooking()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.Add(null));
        }

        [Test, Order(3)]
        public async Task GetAllBookings_Success()
        {
            var bookings = await _repository.GetAll();
            Assert.AreEqual(2, bookings.Count());
        }

        [Test, Order(4)]
        public async Task GetBookingById_Success()
        {
            var booking = await _repository.GetById(1);

            Assert.NotNull(booking);
            Assert.AreEqual(10.0f, booking.TotalPrice);
        }

        [Test, Order(5)]
        public void GetBookingById_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchBookingException>(async () => await _repository.GetById(-1));
        }

        [Test, Order(6)]
        public async Task DeleteBooking_Success()
        {
            var booking = await _repository.GetById(1);
            var deletedBooking = await _repository.Delete(booking.Id);

            Assert.NotNull(deletedBooking);
        }

        [Test, Order(7)]
        public void DeleteBooking_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchBookingException>(async () => await _repository.Delete(-1));
        }

        [Test, Order(8)]
        public async Task DeleteRangeBookings_Success()
        {
            var bookingIds = _context.Bookings.Select(b => b.Id).ToList();
            var result = await _repository.DeleteRange(bookingIds);

            Assert.IsTrue(result);
        }

        [Test, Order(9)]
        public void DeleteRangeBookings_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchBookingException>(async () => await _repository.DeleteRange(new List<int> { -1 }));
        }

        [Test, Order(10)]
        public async Task UpdateBooking_Success()
        {
            var booking = await _repository.GetById(2);
            booking.TotalPrice = 25.0f;
            var updatedBooking = await _repository.Update(booking);

            Assert.NotNull(updatedBooking);
        }

        [Test, Order(11)]
        public void UpdateBooking_Failure_NotFound()
        {
            var booking = new Booking { Id = -1, TotalPrice = 30.0f };
            Assert.ThrowsAsync<NoSuchBookingException>(async () => await _repository.Update(booking));
        }

        [Test, Order(12)]
        public async Task GetAll_Failure_NoBookings()
        {
            _context.Bookings.RemoveRange(_context.Bookings);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<NoBookingsFoundException>(async () => await _repository.GetAll());
        }
    }
}
