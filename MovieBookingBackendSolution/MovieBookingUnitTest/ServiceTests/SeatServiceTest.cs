using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MovieBookingBackend.Exceptions.Seat;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models.DTOs.Seats;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using MovieBookingBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Repositories;

namespace MovieBookingUnitTest.ServiceTests
{
    public class SeatServiceTest
    {
        private MovieBookingContext _context;
        private IRepository<int, Seat> _repository;
        private ILogger<SeatService> _logger;
        private IMapper _mapper;
        private ISeatService _seatService;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new SeatRepository(_context);

            var mockLogger = new Mock<ILogger<SeatService>>();
            _logger = mockLogger.Object;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Seat, SeatDTO>().ReverseMap();
                cfg.CreateMap<UpdateSeatDTO, Seat>().ReverseMap();
                cfg.CreateMap<UpdateSeatStatusDTO, Seat>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _seatService = new SeatService(_repository, _logger, _mapper);

            await SeedDatabase();
        }

        private async Task SeedDatabase()
        {
            var showtime = new Showtime
            {
                Id = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(2),
                Status = ShowtimeStatus.Active,
                MovieId = 1,
                TheatreId = 1,
                TotalSeats = 10,
                AvailableSeats = 10,
                TicketPrice = 15.0f,
                Movie = new Movie { Id = 1, Title = "Inception", Genre = "Sci-Fi", Duration = 148, ImageUrl="urll", Language="English", StartDate=DateTime.Now, EndDate=DateTime.Now.AddDays(2), Status=MovieStatus.Running, Synopsis="synopsis" },
                Theatre = new Theatre { Id = 1, Name = "Grand Cinema" , Location="Downtown"}
            };

            _context.Showtimes.Add(showtime);
            await _context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddSeats_AddsSeatsSuccessfully()
        {
            var showtime = await _context.Showtimes.FindAsync(1);

            await _seatService.AddSeats(showtime);

            var seats = _context.Seats.Where(s => s.ShowetimeId == 1).ToList();
            Assert.AreEqual(showtime.TotalSeats, seats.Count);
        }
        

        [Test]
        public async Task DeleteSeats_DeletesSeatsSuccessfully()
        {
            var showtime = await _context.Showtimes.FindAsync(1);
            await _seatService.AddSeats(showtime);

            await _seatService.DeleteSeats(1);

            var seats = _context.Seats.Where(s => s.ShowetimeId == 1).ToList();
            Assert.AreEqual(0, seats.Count);
        }

        [Test]
        public void DeleteSeats_Failure()
        {
            Assert.ThrowsAsync<UnableToDeleteSeatException>(() => _seatService.DeleteSeats(999));
        }

        [Test]
        public async Task GetSeatById_ReturnsSeatSuccessfully()
        {
            var showtime = await _context.Showtimes.FindAsync(1);
            await _seatService.AddSeats(showtime);
            var seat = _context.Seats.First();

            var result = await _seatService.GetSeatById(seat.Id);

            Assert.IsNotNull(result);
        }

        [Test]
        public void GetSeatById_NoSuchSeat_ThrowsNoSuchSeatException()
        {
            Assert.ThrowsAsync<NoSuchSeatException>(() => _seatService.GetSeatById(999));
        }

        [Test]
        public async Task UpdateSeat_UpdatesSeatSuccessfully()
        {
            var showtime = await _context.Showtimes.FindAsync(1);
            await _seatService.AddSeats(showtime);
            var seat = _context.Seats.First();
            var updateSeatDTO = new UpdateSeatDTO
            {
                Id = seat.Id,
                SeatStatus = "Booked",
                IsAvailable = false,
                BookingId = 123
            };

            var result = await _seatService.UpdateSeat(updateSeatDTO);

            Assert.IsTrue(result);
        }

        [Test]
        public void UpdateSeat_NoSuchSeat_ThrowsNoSuchSeatException()
        {
            var updateSeatDTO = new UpdateSeatDTO
            {
                Id = 999,
                SeatStatus = "Booked",
                IsAvailable = false,
                BookingId = 123
            };

            Assert.ThrowsAsync<UnableToUpdateSeatException>(() => _seatService.UpdateSeat(updateSeatDTO));
        }

        [Test]
        public async Task UpdateSeatStatus_UpdatesSeatStatusSuccessfully()
        {
            var showtime = await _context.Showtimes.FindAsync(1);
            await _seatService.AddSeats(showtime);
            var seat = _context.Seats.First();
            var updateSeatStatusDTO = new UpdateSeatStatusDTO
            {
                Id = seat.Id,
                SeatStatus = "Booked",
                BookingId = 123
            };

            var result = await _seatService.UpdateSeatStatus(updateSeatStatusDTO);

            Assert.IsNotNull(result);
        }

        [Test]
        public void UpdateSeatStatus_NoSuchSeat_ThrowsNoSuchSeatException()
        {
            var updateSeatStatusDTO = new UpdateSeatStatusDTO
            {
                Id = 999,
                SeatStatus = "Booked",
                BookingId = 123
            };

            Assert.ThrowsAsync<NoSuchSeatException>(() => _seatService.UpdateSeatStatus(updateSeatStatusDTO));
        }
    }
}
