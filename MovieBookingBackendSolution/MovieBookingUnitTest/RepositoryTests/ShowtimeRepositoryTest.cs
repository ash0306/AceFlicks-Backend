using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Showtime;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTest.RepositoryTests
{
    public class ShowtimeRepositoryTest
    {
        private MovieBookingContext _context;
        private ShowtimeRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestShowtimeDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new ShowtimeRepository(_context);

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

        [Test, Order(1)]
        public async Task AddShowtime_Success()
        {
            var showtime = new Showtime
            {
                MovieId = _context.Movies.First().Id,
                TheatreId = _context.Theatres.First().Id,
                StartTime = DateTime.Now.AddDays(2),
                EndTime = DateTime.Now.AddDays(2).AddHours(2),
                Status = ShowtimeStatus.Active,
                TotalSeats = 120,
                AvailableSeats = 120,
                TicketPrice = 12.5f
            };

            var addedShowtime = await _repository.Add(showtime);

            Assert.NotNull(addedShowtime);
            Assert.AreEqual(showtime.StartTime, addedShowtime.StartTime);
            Assert.AreEqual(2, _context.Showtimes.Count()); // Since we seeded with one and added another
        }

        [Test, Order(2)]
        public void AddShowtime_Failure_NullShowtime()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.Add(null));
        }

        [Test, Order(3)]
        public async Task GetAllShowtimes_Success()
        {
            var showtimes = await _repository.GetAll();
            Assert.AreEqual(1, showtimes.Count());
        }

        [Test, Order(4)]
        public async Task GetAllShowtimes_Failure_NoShowtimes()
        {
            _context.Showtimes.RemoveRange(_context.Showtimes);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<NoShowtimesFoundException>(async () => await _repository.GetAll());
        }

        [Test, Order(5)]
        public async Task GetShowtimeById_Success()
        {
            var showtime = await _repository.GetById(1);

            Assert.NotNull(showtime);
            Assert.AreEqual(1, showtime.Id);
        }

        [Test, Order(6)]
        public void GetShowtimeById_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchShowtimeException>(async () => await _repository.GetById(-1));
        }

        [Test, Order(7)]
        public async Task UpdateShowtime_Success()
        {
            var showtime = await _repository.GetById(1);
            showtime.Status = ShowtimeStatus.Inactive;

            var updatedShowtime = await _repository.Update(showtime);

            Assert.NotNull(updatedShowtime);
        }

        [Test, Order(8)]
        public void UpdateShowtime_Failure_NotFound()
        {
            var showtime = new Showtime { Id = 100, Status = ShowtimeStatus.Inactive };

            Assert.ThrowsAsync<NoSuchShowtimeException>(async () => await _repository.Update(showtime));
        }

        [Test, Order(9)]
        public async Task DeleteShowtime_Success()
        {
            var deletedShowtime = await _repository.Delete(1);

            Assert.NotNull(deletedShowtime);
        }

        [Test, Order(10)]
        public void DeleteShowtime_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchShowtimeException>(async () => await _repository.Delete(100));
        }

        [Test, Order(11)]
        public async Task DeleteRange_Success()
        {
            var showtime = new Showtime
            {
                MovieId = _context.Movies.First().Id,
                TheatreId = _context.Theatres.First().Id,
                StartTime = DateTime.Now.AddDays(3),
                EndTime = DateTime.Now.AddDays(3).AddHours(2),
                Status = ShowtimeStatus.Active,
                TotalSeats = 120,
                AvailableSeats = 120,
                TicketPrice = 12.5f
            };
            await _repository.Add(showtime);

            var result = await _repository.DeleteRange(new List<int> { 1, 2 });

            Assert.IsTrue(result);
        }

        [Test, Order(12)]
        public void DeleteRange_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchShowtimeException>(async () => await _repository.DeleteRange(new List<int> { 100 }));
        }
    }
}
