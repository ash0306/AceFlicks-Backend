using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Movie;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models.DTOs.Movies;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using MovieBookingBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieBookingBackend.Mappings;

namespace MovieBookingUnitTest.ServiceTests
{
    public class MovieServiceTest
    {
        private MovieBookingContext _context;
        private IRepository<int, Movie> _movieRepository;
        private IMapper _mapper;
        private ILogger<MovieServices> _logger;
        private MovieServices _movieServices;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _movieRepository = new MovieRepository(_context);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _logger = new LoggerFactory().CreateLogger<MovieServices>();
            _movieServices = new MovieServices(_movieRepository, _mapper, _logger);

            var movie = new Movie
            {
                Title = "Tenet",
                Synopsis = "A mind-bending thriller by Christopher Nolan.",
                Genre = "Sci-Fi",
                Language = "English",
                ImageUrl = "tenet.jpg",
                Duration = 148,
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now.AddMonths(1),
                Status = MovieStatus.Running
            };
            _context.Movies.Add(movie);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddMovie_AddsMovieSuccessfully()
        {
            var movieDTO = new MovieDTO
            {
                Title = "Inception",
                Duration = 148,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Synopsis = "A mind-bending thriller by Christopher Nolan.",
                Genre = "Sci-Fi",
                Language = "English",
                ImageUrl = "tenet.jpg",
                Status = MovieStatus.Running.ToString()
            };

            var result = await _movieServices.AddMovie(movieDTO);

            Assert.IsNotNull(result);
            Assert.AreEqual(movieDTO.Title, result.Title);
        }

        [Test]
        public void AddMovie_MovieAlreadyExists_ThrowsException()
        {
            var movie = new Movie
            {
                Title = "Inception",
                Duration = 148,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Synopsis = "A mind-bending thriller by Christopher Nolan.",
                Genre = "Sci-Fi",
                Language = "English",
                ImageUrl = "tenet.jpg",
                Status = MovieStatus.Running
            };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var movieDTO = new MovieDTO
            {
                Title = "Inception",
                Duration = 148,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Language = "English",
                Status = MovieStatus.Running.ToString()
            };

            Assert.ThrowsAsync<UnableToAddMovieException>(() => _movieServices.AddMovie(movieDTO));
        }

        [Test]
        public async Task DeleteMovie_DeletesMovieSuccessfully()
        {
            var movie = new Movie
            {
                Title = "Inception",
                Duration = 148,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Synopsis = "A mind-bending thriller by Christopher Nolan.",
                Genre = "Sci-Fi",
                Language = "English",
                ImageUrl = "tenet.jpg",
                Status = MovieStatus.Running
            };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var result = await _movieServices.DeleteMovie(movie.Id);

            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteMovie_NoSuchMovie_ThrowsException()
        {
            Assert.ThrowsAsync<NoSuchMovieException>(() => _movieServices.DeleteMovie(999));
        }

        [Test]
        public async Task GetAllMovies_ReturnsAllMovies()
        {
            var result = await _movieServices.GetAllMovies();

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task UpdateMovie_UpdatesMovieSuccessfully()
        {
            var movie = new Movie
            {
                Title = "Inception",
                Duration = 148,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Synopsis = "A mind-bending thriller by Christopher Nolan.",
                Genre = "Sci-Fi",
                Language = "English",
                ImageUrl = "tenet.jpg",
                Status = MovieStatus.Running
            };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var updateMovieDTO = new UpdateMovieDTO
            {
                Title = "Inception",
                Duration = 150,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10),
                Status = MovieStatus.Running.ToString()
            };

            var result = await _movieServices.UpdateMovie(updateMovieDTO);

            Assert.IsNotNull(result);
            Assert.AreEqual(updateMovieDTO.Duration, result.Duration);
            Assert.AreEqual(updateMovieDTO.EndDate, result.EndDate);
        }

        [Test]
        public void UpdateMovie_ThrowsException()
        {
            var updateMovieDTO = new UpdateMovieDTO
            {
                Title = "Nonexistent Movie",
                Duration = 150,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10),
                Status = MovieStatus.Running.ToString()
            };

            Assert.ThrowsAsync<UnableToUpdateMovieException>(() => _movieServices.UpdateMovie(updateMovieDTO));
        }
    }
}
