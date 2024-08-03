using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Theatre;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models.DTOs.Theatres;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using MovieBookingBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTest.ServiceTests
{
    public class TheatreServiceTest
    {
        private MovieBookingContext _context;
        private IRepository<int, Theatre> _repository;
        private ITheatreService _theatreService;
        private IMapper _mapper;
        private Mock<ILogger<TheatreService>> _logger;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new TheatreRepository(_context); // Assuming this exists
            _logger = new Mock<ILogger<TheatreService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Theatre, TheatreDTO>();
                cfg.CreateMap<TheatreDTO, Theatre>();
                cfg.CreateMap<UpdateTheatreDTO, Theatre>();
            });
            _mapper = config.CreateMapper();

            _theatreService = new TheatreService(_repository, _logger.Object, _mapper);

            // Seed initial data
            var theatres = new List<Theatre>
            {
                new Theatre { Id = 1, Name = "Theatre A", Location = "Location 1" },
                new Theatre { Id = 2, Name = "Theatre B", Location = "Location 2" }
            };
            await _context.Theatres.AddRangeAsync(theatres);
            await _context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllTheatres_ReturnsAllTheatres()
        {
            var result = await _theatreService.GetAllTheatres();
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAllTheatres_NoTheatresFound_ThrowsNoTheatresFoundException()
        {
            _context.Theatres.RemoveRange(_context.Theatres);
            _context.SaveChanges();

            Assert.ThrowsAsync<NoTheatresFoundException>(() => _theatreService.GetAllTheatres());
        }

        [Test]
        public async Task GetAllTheatreNames_ReturnsUniqueNames()
        {
            var result = await _theatreService.GetAllTheatreNames();
            Assert.AreEqual(2, result.Count());
            Assert.Contains("Theatre A", result.ToList());
            Assert.Contains("Theatre B", result.ToList());
        }

        [Test]
        public void GetAllTheatreNames_NoTheatresFound_ThrowsNoTheatresFoundException()
        {
            _context.Theatres.RemoveRange(_context.Theatres);
            _context.SaveChanges();

            Assert.ThrowsAsync<NoTheatresFoundException>(() => _theatreService.GetAllTheatreNames());
        }

        [Test]
        public async Task GetTheatreById_ReturnsTheatre_WhenTheatreExists()
        {
            var result = await _theatreService.GetTheatreById(1);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Theatre A", result.Name);
        }

        [Test]
        public void GetTheatreById_NoTheatreFound_ThrowsNoSuchTheatreException()
        {
            Assert.ThrowsAsync<NoTheatresFoundException>(() => _theatreService.GetTheatreById(99));
        }

        [Test]
        public async Task GetTheatreLocationsByName_ReturnsLocations()
        {
            var result = await _theatreService.GetTheatreLocationsByName("Theatre A");
            Assert.AreEqual(1, result.Count());
            Assert.Contains("Location 1", result.ToList());
        }

        [Test]
        public void GetTheatreLocationsByName_NoTheatresFound_ThrowsNoTheatresFoundException()
        {
            _context.Theatres.RemoveRange(_context.Theatres.Where(t => t.Name == "Theatre A"));
            _context.SaveChanges();

            Assert.ThrowsAsync<NoTheatresFoundException>(() => _theatreService.GetTheatreLocationsByName("Theatre A"));
        }

        [Test]
        public async Task AddTheatre_AddsTheatreSuccessfully()
        {
            var theatreDTO = new TheatreDTO { Name = "Theatre C", Location = "Location 3" };
            var result = await _theatreService.AddTheatre(theatreDTO);
            Assert.AreEqual("Theatre C", result.Name);
            Assert.AreEqual("Location 3", result.Location);
        }

        [Test]
        public void AddTheatre_TheatreAlreadyExists_ThrowsTheatreAlreadyExistsException()
        {
            var theatreDTO = new TheatreDTO { Name = "Theatre A", Location = "Location 1" };
            Assert.ThrowsAsync<UnableToAddTheatreException>(() => _theatreService.AddTheatre(theatreDTO));
        }

        [Test]
        public async Task UpdateTheatre_UpdatesTheatreDetailsSuccessfully()
        {
            var updateDTO = new UpdateTheatreDTO
            {
                Name = "Theatre A",
                OldLocation = "Location 1",
                NewLocation = "New Location"
            };

            var result = await _theatreService.UpdateTheatre(updateDTO);
            Assert.AreEqual("New Location", result.Location);
        }

        [Test]
        public void UpdateTheatre_NoTheatreFound_ThrowsNoTheatresFoundException()
        {
            var updateDTO = new UpdateTheatreDTO
            {
                Name = "Nonexistent Theatre",
                OldLocation = "Old Location",
                NewLocation = "New Location"
            };

            Assert.ThrowsAsync<UnableToUpdateTheatreException>(() => _theatreService.UpdateTheatre(updateDTO));
        }

        [Test]
        public async Task DeleteTheatre_DeletesTheatreSuccessfully()
        {
            var result = await _theatreService.DeleteTheatre(1);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteTheatre_NoTheatreFound_ThrowsNoSuchTheatreException()
        {
            Assert.ThrowsAsync<NoSuchTheatreException>(() => _theatreService.DeleteTheatre(99));
        }
    }
}
