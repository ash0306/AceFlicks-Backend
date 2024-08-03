using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Seat;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieBookingBackend.Exceptions.User;

namespace MovieBookingUnitTest.RepositoryTests
{
    public class SeatRepositoryTest
    {
        private MovieBookingContext _context;
        private SeatRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestSeatDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new SeatRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var showtimes = new List<Showtime>
            {
                new Showtime { MovieId = 1, TheatreId = 1, StartTime = DateTime.Now.AddHours(1), EndTime = DateTime.Now.AddHours(3) }
            };

            var seats = new List<Seat>
            {
                new Seat { Row = "A", SeatNumber = 1, SeatStatus = SeatStatus.Available, IsAvailable = true, ShowetimeId = 1 },
                new Seat { Row = "A", SeatNumber = 2, SeatStatus = SeatStatus.Booked, IsAvailable = false, ShowetimeId = 1 }
            };

            _context.Showtimes.AddRange(showtimes);
            _context.Seats.AddRange(seats);
            _context.SaveChanges();
        }

        [Test, Order(1)]
        public async Task AddSeat_Success()
        {
            var seat = new Seat
            {
                Row = "B",
                SeatNumber = 3,
                SeatStatus = SeatStatus.Available,
                IsAvailable = true,
                ShowetimeId = 1
            };

            var addedSeat = await _repository.Add(seat);

            Assert.NotNull(addedSeat);
        }

        [Test, Order(2)]
        public void AddSeat_Failure_NullSeat()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.Add(null));
        }

        [Test, Order(3)]
        public async Task GetAllSeats_Success()
        {
            var seats = await _repository.GetAll();
            Assert.AreEqual(2, seats.Count());
        }

        [Test, Order(4)]
        public async Task GetSeatById_Success()
        {
            var seat = await _repository.GetById(1);

            Assert.NotNull(seat);
        }

        [Test, Order(5)]
        public void GetSeatById_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchSeatException>(async () => await _repository.GetById(-1));
        }

        [Test, Order(6)]
        public async Task DeleteSeat_Success()
        {
            var seat = await _repository.GetById(1);
            var deletedSeat = await _repository.Delete(seat.Id);

            Assert.NotNull(deletedSeat);
        }

        [Test, Order(7)]
        public void DeleteSeat_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchSeatException>(async () => await _repository.Delete(-1));
        }

        [Test, Order(8)]
        public async Task DeleteRangeSeats_Success()
        {
            var seatIds = _context.Seats.Select(s => s.Id).ToList();
            var result = await _repository.DeleteRange(seatIds);

            Assert.IsTrue(result);
        }

        [Test, Order(9)]
        public void DeleteRangeSeats_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchSeatException>(async () => await _repository.DeleteRange(new List<int> { -1 }));
        }

        [Test, Order(10)]
        public async Task UpdateSeat_Success()
        {
            var seat = await _repository.GetById(2);
            seat.SeatStatus = SeatStatus.Available;
            var updatedSeat = await _repository.Update(seat);

            Assert.NotNull(updatedSeat);
        }

        [Test, Order(11)]
        public void UpdateSeat_Failure_NotFound()
        {
            var seat = new Seat { Id = -1, Row = "B", SeatNumber = 4 };
            Assert.ThrowsAsync<NoSuchSeatException>(async () => await _repository.Update(seat));
        }
        
        
        [Test, Order(12)]
        public async Task GetAll_Failure_NoSeats()
        {
            _context.Seats.RemoveRange(_context.Seats);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<NoSeatsFoundException>(async () => await _repository.GetAll());
        }
    }
}
