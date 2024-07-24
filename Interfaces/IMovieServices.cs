using MovieBookingBackend.Models.DTOs.Movie;

namespace MovieBookingBackend.Interfaces
{
    public interface IMovieServices
    {
        public Task<MovieDTO> AddMovie(MovieDTO movieDTO);
        public Task<MovieDTO> UpdateMovie(UpdateMovieDTO updateMovieDTO);
        public Task<IEnumerable<MovieDTO>> GetAllMovies();
        public Task<IEnumerable<MovieDTO>> GetAllRunningMovies();
    }
}
