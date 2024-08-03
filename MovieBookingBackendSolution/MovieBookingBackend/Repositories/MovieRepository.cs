using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Movie;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingBackend.Repositories
{
    public class MovieRepository : IRepository<int, Movie>
    {
        private readonly MovieBookingContext _context;

        /// <summary>
        /// Parameterised Constructor
        /// </summary>
        /// <param name="context">DB Context for MovieBooking</param>
        public MovieRepository(MovieBookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new Movie
        /// </summary>
        /// <param name="item">Movie to be added</param>
        /// <returns>The newly added movie</returns>
        /// <exception cref="ArgumentNullException">Thrown if the movie to added is null</exception>
        /// <exception cref="UnableToAddMovieException">Thrown if movie cannot be added</exception>
        public async Task<Movie> Add(Movie item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.Movies.Add(item);
            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToAddMovieException($"Unable to add the movie with ID: {item.Id}");
            }
            return item;
        }

        /// <summary>
        /// Deletes a movie
        /// </summary>
        /// <param name="key">ID of the movie to be deleted</param>
        /// <returns>The deleted movie</returns>
        /// <exception cref="NoSuchMovieException">Thrown if no movie with the given ID exists</exception>
        /// <exception cref="UnableToDeleteMovieException">Thrown if the movie cannot be deleted</exception>
        public async Task<Movie> Delete(int key)
        {
            var movie = await GetById(key);
            if (movie == null)
            {
                throw new NoSuchMovieException($"No movie with ID {key} was found");
            }
            _context.Remove(movie);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteMovieException($"Unable to delete movie with ID {key}");
            }

            return movie;
        }

        /// <summary>
        /// Deletes a range of movies
        /// </summary>
        /// <param name="key">Id's of the movies to be deleted</param>
        /// <returns>Boolean result of the delete operation</returns>
        /// <exception cref="NoSuchMovieException"></exception>
        /// <exception cref="UnableToDeleteMovieException"></exception>
        public async Task<bool> DeleteRange(IList<int> key)
        {
            IList<Movie> movies = new List<Movie>();
            foreach (var id in key)
            {
                var result = await GetById(id);
                movies.Add(result);
            }
            if (movies.Count() <= 0)
            {
                throw new NoMoviesFoundException($"No movies were found");
            }
            _context.RemoveRange(movies);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteMovieException($"Unable to delete movie with ID {key}");
            }

            return true;
        }

        /// <summary>
        /// Gets a list of the movies
        /// </summary>
        /// <returns>List of all movies</returns>
        /// <exception cref="NoMoviesFoundException">If no movies are found</exception>
        public async Task<IEnumerable<Movie>> GetAll()
        {
            var movies = await _context.Movies.Include(m => m.Showtimes).ThenInclude(s =>  s.Theatre).ToListAsync();

            if (movies.Count() <= 0)
            {
                throw new NoMoviesFoundException("No movies found");
            }
            return movies;
        }

        /// <summary>
        /// Gets a movie given its ID
        /// </summary>
        /// <param name="key">ID of the movie to be fetched</param>
        /// <returns>Movie with the given ID</returns>
        /// <exception cref="NoSuchMovieException">If movie with the ID doesn't exist</exception>
        public async Task<Movie> GetById(int key)
        {
            var movie = await _context.Movies.Include(m => m.Showtimes).ThenInclude(s => s.Theatre).FirstOrDefaultAsync(m => m.Id == key);
            if (movie == null)
            {
                throw new NoSuchMovieException($"No movie with ID {key} was found");
            }
            return movie;
        }

        /// <summary>
        /// Updates the movie details
        /// </summary>
        /// <param name="item">Movie with the details to be updated</param>
        /// <returns>Updated movie</returns>
        /// <exception cref="NoSuchMovieException">Thrown if movie with the ID doesn't exist</exception>
        /// <exception cref="UnableToUpdateMovieException">Thrown if movie cannot be updated</exception>
        public async Task<Movie> Update(Movie item)
        {
            var movie = await GetById(item.Id);
            if (movie == null)
            {
                throw new NoSuchMovieException($"No movie with ID {item.Id} was found");
            }
            _context.Update(movie);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToUpdateMovieException($"Unable to update movie with ID {item.Id}");
            }

            return movie;
        }
    }
}
