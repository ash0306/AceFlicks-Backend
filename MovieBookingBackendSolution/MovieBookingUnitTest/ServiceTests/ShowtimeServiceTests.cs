using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Showtime;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models.DTOs.Showtimes;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using MovieBookingBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieBookingBackend.Models.DTOs.Seats;
using MovieBookingBackend.Models.DTOs.Movies;
using MovieBookingBackend.Models.DTOs.Theatres;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Exceptions.Seat;
using System.Runtime.CompilerServices;

namespace MovieBookingUnitTest.ServiceTests
{
    public class ShowtimeServiceTests
    {
        private MovieBookingContext _context;
        private IRepository<int, Showtime> _repository;
        private IRepository<int, Movie> _movieRepository;
        private IRepository<int, Theatre> _theatreRepository;
        private IShowtimeService _showtimeService;
        private IMapper _mapper;
        private Mock<ILogger<ShowtimeService>> _logger;
        private Mock<ISeatService> _seatService;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new ShowtimeRepository(_context);
            _movieRepository = new MovieRepository(_context);
            _theatreRepository = new TheatreRepository(_context);
            _logger = new Mock<ILogger<ShowtimeService>>();
            _seatService = new Mock<ISeatService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Showtime, ShowtimeDTO>().ReverseMap();
                cfg.CreateMap<AddShowtimeDTO, Showtime>().ReverseMap();
                cfg.CreateMap<UpdateShowtimeDTO, Showtime>().ReverseMap();
                cfg.CreateMap<Seat, SeatDTO>().ReverseMap();
                cfg.CreateMap<Movie, MovieDTO>().ReverseMap();
                cfg.CreateMap<Theatre, TheatreDTO>().ReverseMap();
                cfg.CreateMap<Showtime, ShowtimeDetailsDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _showtimeService = new ShowtimeService(_repository, _mapper, _logger.Object, _movieRepository, _theatreRepository, _seatService.Object);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var movie = new Movie
            {
                Title = "Inception",
                Synopsis = "A mind-bending thriller by Christopher Nolan.",
                Genre = "Sci-Fi",
                Language = "English",
                ImageUrl = "inception.jpg",
                Duration = 148,
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now.AddMonths(1),
                Status = MovieStatus.Running
            };

            var theatre = new Theatre
            {
                Name = "Grand Cinema",
                Location = "Downtown"
            };

            _context.Movies.Add(movie);
            _context.Theatres.Add(theatre);
            _context.SaveChanges();

            var showtimes = new List<Showtime>
            {
                new Showtime
                {
                    MovieId = movie.Id,
                    TheatreId = theatre.Id,
                    StartTime = DateTime.Now.AddDays(1),
                    EndTime = DateTime.Now.AddDays(1).AddHours(2),
                    Status = ShowtimeStatus.Active,
                    TotalSeats = 100,
                    AvailableSeats = 100,
                    TicketPrice = 10.0f
                }
            };

            _context.Showtimes.AddRange(showtimes);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddShowtime_AddsNewShowtimeSuccessfully()
        {
            var addShowtimeDTO = new AddShowtimeDTO
            {
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(3),
                MovieId = 1,
                TheatreId = 1,
                TotalSeats = 120
            };

            var result = await _showtimeService.AddShowtime(addShowtimeDTO);

            Assert.NotNull(result);
        }

        [Test]
        public async Task DeleteShowtime_DeletesExistingShowtime()
        {
            var result = await _showtimeService.DeleteShowtime(1);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteShowtime_NoSuchShowtime_ThrowsNoSuchShowtimeException()
        {
            Assert.ThrowsAsync<NoSuchShowtimeException>(() => _showtimeService.DeleteShowtime(999));
        }

        [Test]
        public async Task GetAllShowtime_ReturnsAllShowtimes()
        {
            var result = await _showtimeService.GetAllShowtime();
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void GetAllShowtime_NoShowtimesFound_ThrowsNoShowtimesFoundException()
        {
            _context.Showtimes.RemoveRange(_context.Showtimes);
            _context.SaveChanges();

            Assert.ThrowsAsync<NoShowtimesFoundException>(() => _showtimeService.GetAllShowtime());
        }

        [Test]
        public async Task UpdateShowtime_UpdatesShowtimeSuccessfully()
        {
            var updateShowtimeDTO = new UpdateShowtimeDTO
            {
                Id = 1,
                EndTime = DateTime.Now.AddHours(5)
            };

            var result = await _showtimeService.UpdateShowtime(updateShowtimeDTO);
            Assert.AreEqual(updateShowtimeDTO.EndTime, result.EndTime);
        }

        [Test]
        public void UpdateShowtime_NoSuchShowtime_ThrowsNoSuchShowtimeException()
        {
            var updateShowtimeDTO = new UpdateShowtimeDTO
            {
                Id = 999,
                EndTime = DateTime.Now.AddHours(5)
            };

            Assert.ThrowsAsync<UnableToUpdateShowtimeException>(() => _showtimeService.UpdateShowtime(updateShowtimeDTO));
        }

        [Test]
        public async Task GetShowtimesForAMovie_SuccessTest()
        {
            var result = await _showtimeService.GetShowtimesForAMovie("Inception");
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void GetShowtimesForAMovie_NoShowtimesFound_ThrowsNoShowtimesFoundException()
        {
            Assert.ThrowsAsync<NoShowtimesFoundException>(() => _showtimeService.GetShowtimesForAMovie("Nonexistent Movie"));
        }

        [Test]
        public async Task GetShowtimesForATheatre_ReturnsShowtimesGroupedByMovie()
        {
            var result = await _showtimeService.GetShowtimesForATheatre("Grand Cinema");
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void GetShowtimesForATheatre_NoShowtimesFound_ThrowsNoShowtimesFoundException()
        {
            Assert.ThrowsAsync<NoShowtimesFoundException>(() => _showtimeService.GetShowtimesForATheatre("Nonexistent Theatre"));
        }

        [Test]
        public async Task GetSeatsByShowtime_ReturnsSeatsForShowtime()
        {
            var result = await _showtimeService.GetSeatsByShowtime(1);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetSeatsByShowtime_Failure()
        {
            Assert.ThrowsAsync<NoSeatsFoundException>(() => _showtimeService.GetSeatsByShowtime(999));
        }

        [Test]
        public async Task GetShowtimeDetailsById_ReturnsShowtimeDetails()
        {
            var result = await _showtimeService.GetShowtimeDetailsById(1);
            Assert.NotNull(result);
        }

        [Test]
        public async Task  GetShowtimeDetailsById_FailureTest()
        {
            var result = await _showtimeService.GetShowtimeDetailsById(999);
            Assert.IsNull(result);
        }
    }
}
