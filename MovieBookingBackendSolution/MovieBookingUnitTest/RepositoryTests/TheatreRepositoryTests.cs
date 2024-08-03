using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Theatre;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTest.RepositoryTests
{
    public class TheatreRepositoryTests
    {
        private MovieBookingContext _context;
        private TheatreRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new TheatreRepository(_context);

            SeedDatabase();
        }

        private async void SeedDatabase()
        {
            // Seed initial data
            var theatres = new Theatre
            {
                Name = "Grand Cinema",
                Location = "Downtown"
            };

            await _repository.Add(theatres);
        }

        [Test, Order(1)]
        public async Task AddTheatre_Success()
        {
            var newTheatre = new Theatre
            {
                Name = "New Theatre",
                Location = "Suburb"
            };

            var addedTheatre = await _repository.Add(newTheatre);

            Assert.NotNull(addedTheatre);
            Assert.AreEqual(newTheatre.Name, addedTheatre.Name);
            Assert.AreEqual(2, _context.Theatres.Count()); // Adjust count based on seed data
        }

        [Test, Order(2)]
        public void AddTheatre_Failure_NullTheatre()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.Add(null));
        }

        [Test, Order(3)]
        public async Task GetAll_Success()
        {
            var theatres = await _repository.GetAll();
            Assert.AreEqual(4, theatres.Count()); // Adjust based on seed data
        }

        [Test, Order(4)]
        public async Task GetAll_Failure_NoTheatres()
        {
            _context.Theatres.RemoveRange(_context.Theatres);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<NoTheatresFoundException>(async () => await _repository.GetAll());
        }

        [Test, Order(5)]
        public async Task GetById_Success()
        {
            var theatre = await _repository.GetById(1); // Use an existing ID

            Assert.NotNull(theatre);
        }

        [Test, Order(6)]
        public void GetById_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchTheatreException>(async () => await _repository.GetById(-1)); // Use a non-existing ID
        }

        [Test, Order(7)]
        public async Task UpdateTheatre_Success()
        {
            var theatre = await _repository.GetById(1);
            theatre.Name = "Updated Theatre Name";

            var updatedTheatre = await _repository.Update(theatre);

            Assert.NotNull(updatedTheatre);
        }

        [Test, Order(8)]
        public void UpdateTheatre_Failure_NotFound()
        {
            var theatre = new Theatre
            {
                Id = 100, // Non-existing ID
                Name = "Nonexistent Theatre"
            };

            Assert.ThrowsAsync<NoSuchTheatreException>(async () => await _repository.Update(theatre));
        }

        [Test, Order(9)]
        public async Task DeleteTheatre_Success()
        {
            var deletedTheatre = await _repository.Delete(1); // Use an existing ID

            Assert.NotNull(deletedTheatre);
        }

        [Test, Order(10)]
        public void DeleteTheatre_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchTheatreException>(async () => await _repository.Delete(100)); // Use a non-existing ID
        }

        [Test, Order(11)]
        public async Task DeleteRange_Success()
        {
            var ids = _context.Theatres.Select(t => t.Id).ToList(); // Get existing IDs
            var result = await _repository.DeleteRange(ids);

            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.Theatres.Count()); // All theatres should be deleted
        }

        [Test, Order(12)]
        public void DeleteRange_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchTheatreException>(async () => await _repository.DeleteRange(new List<int> { 100, 101 })); // Use non-existing IDs
        }
    }
}
