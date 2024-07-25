using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Movies;
using MovieBookingBackend.Models.DTOs.Showtimes;

namespace MovieBookingBackend.Interfaces
{
    public interface IMovieServices
    {
        public Task<MovieDTO> AddMovie(MovieDTO movieDTO);
        public Task<MovieDTO> UpdateMovie(UpdateMovieDTO updateMovieDTO);
        public Task<IEnumerable<MovieDTO>> GetAllMovies();
        public Task<IEnumerable<MovieDTO>> GetAllRunningMovies();
        public Task<IEnumerable<IGrouping<int, ShowtimeDTO>>> GetShowtimesForAMovie(string movieName);
    }
}
