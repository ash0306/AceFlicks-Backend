using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Movie;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieBookingBackend.Exceptions.Seat;

namespace MovieBookingUnitTest.RepositoryTests
{
    public class MovieRepositroyTest
    {
        private MovieBookingContext _context;
        private MovieRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestMovieDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new MovieRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var movies = new List<Movie>
            {
                new Movie
                {
                    Title = "Movie 1",
                    Synopsis = "Synopsis 1",
                    Genre = "Action",
                    Language = "English",
                    ImageUrl = "url1",
                    Duration = 120,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1),
                    Status = MovieStatus.Running
                },
                new Movie
                {
                    Title = "Movie 2",
                    Synopsis = "Synopsis 2",
                    Genre = "Comedy",
                    Language = "Hindi",
                    ImageUrl = "url2",
                    Duration = 100,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1),
                    Status = MovieStatus.Running
                }
            };

            _context.Movies.AddRange(movies);
            _context.SaveChanges();
        }

        [Test, Order(1)]
        public async Task AddMovie_Success()
        {
            var movie = new Movie
            {
                Title = "New Movie",
                Synopsis = "New Synopsis",
                Genre = "Drama",
                Language = "French",
                ImageUrl = "url_new",
                Duration = 90,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Status = MovieStatus.Running
            };

            var addedMovie = await _repository.Add(movie);

            Assert.NotNull(addedMovie);
        }

        [Test, Order(2)]
        public void AddMovie_Failure_NullMovie()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.Add(null));
        }

        [Test, Order(3)]
        public async Task GetAllMovies_Success()
        {
            var movies = await _repository.GetAll();
            Assert.AreEqual(2, movies.Count());
        }

        [Test, Order(4)]
        public async Task GetMovieById_Success()
        {
            var movie = await _repository.GetById(1);

            Assert.NotNull(movie);
        }

        [Test, Order(5)]
        public void GetMovieById_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchMovieException>(async () => await _repository.GetById(-1));
        }

        [Test, Order(6)]
        public async Task DeleteMovie_Success()
        {
            var movie = await _repository.GetById(1);
            var deletedMovie = await _repository.Delete(movie.Id);

            Assert.NotNull(deletedMovie);
        }

        [Test, Order(7)]
        public void DeleteMovie_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchMovieException>(async () => await _repository.Delete(-1));
        }

        [Test, Order(8)]
        public async Task DeleteRangeMovies_Success()
        {
            var movieIds = _context.Movies.Select(m => m.Id).ToList();
            var result = await _repository.DeleteRange(movieIds);

            Assert.IsTrue(result);
        }

        [Test, Order(9)]
        public void DeleteRangeMovies_Failure_NotFound()
        {
            Assert.ThrowsAsync<NoSuchMovieException>(async () => await _repository.DeleteRange(new List<int> { -1 }));
        }

        [Test, Order(10)]
        public async Task UpdateMovie_Success()
        {
            var movie = await _repository.GetById(2);
            movie.Title = "Updated Movie 2";
            var updatedMovie = await _repository.Update(movie);

            Assert.NotNull(updatedMovie);
        }

        [Test, Order(11)]
        public void UpdateMovie_Failure_NotFound()
        {
            var movie = new Movie { Id = -1, Title = "Non-existing Movie" };
            Assert.ThrowsAsync<NoSuchMovieException>(async () => await _repository.Update(movie));
        }

        [Test, Order(12)]
        public async Task GetAll_Failure_NoMovies()
        {
            _context.Movies.RemoveRange(_context.Movies);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<NoMoviesFoundException>(async () => await _repository.GetAll());
        }
    }
}
