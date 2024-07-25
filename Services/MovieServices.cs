using AutoMapper;
using MovieBookingBackend.Exceptions.Movie;
using MovieBookingBackend.Exceptions.Showtime;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Movies;
using MovieBookingBackend.Models.DTOs.Showtimes;
using MovieBookingBackend.Models.Enums;

namespace MovieBookingBackend.Services
{
    public class MovieServices : IMovieServices
    {
        private readonly IRepository<int, Movie> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieServices> _logger;

        public MovieServices(IRepository<int, Movie> repository, IMapper mapper, ILogger<MovieServices> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MovieDTO> AddMovie(MovieDTO movieDTO)
        {
            try
            {
                var alreadyExists = (await _repository.GetAll()).FirstOrDefault(m => m.Title == movieDTO.Title);
                if(alreadyExists != null)
                {
                    _logger.LogCritical($"Movie {movieDTO.Title} already exists");
                    throw new MovieAlreadyExistsException($"Movie {movieDTO.Title} already exists");
                }
                Movie movie = _mapper.Map<Movie>(movieDTO);
                movie.Status = (MovieStatus)Enum.Parse(typeof(MovieStatus), movieDTO.Status);
                var newMovie = await _repository.Add(movie);

                MovieDTO returnDTO = _mapper.Map<MovieDTO>(newMovie);
                return returnDTO;
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Unable to add movie");
                throw new UnableToAddMovieException($"Unable to add movie.{ex.Message}");
            }
        }

        public async Task<IEnumerable<MovieDTO>> GetAllMovies()
        {
            try
            {
                IEnumerable<Movie> movies = await _repository.GetAll();
                if(movies.Count() <= 0)
                {
                    _logger.LogCritical("No movies found");
                    throw new NoMoviesFoundException("No movies found");
                }

                List<MovieDTO> returnMovies = new List<MovieDTO>();
                foreach (var item in movies)
                {
                    var movieDTO = _mapper.Map<MovieDTO>(item);
                    returnMovies.Add(movieDTO);
                }

                return returnMovies;
            }
            catch(Exception ex)
            {
                _logger.LogCritical("Unable to fetch movies"+ex.Message);
                throw new NoMoviesFoundException("Unable to fetch movies"+ex.Message );
            }
        }

        public async Task<IEnumerable<MovieDTO>> GetAllRunningMovies()
        {
            try
            {
                IEnumerable<Movie> movies = (await _repository.GetAll()).Where(m => m.Status == MovieStatus.Running);
                if (movies.Count() <= 0)
                {
                    _logger.LogCritical("No running movies found");
                    throw new NoMoviesFoundException("No running movies found");
                }

                IList<MovieDTO> returnMovies = new List<MovieDTO>();
                foreach (var item in movies)
                {
                    returnMovies.Append(_mapper.Map<MovieDTO>(item));
                }

                return returnMovies;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unable to fetch movies" + ex.Message);
                throw new NoMoviesFoundException("Unable to fetch movies" + ex.Message);
            }
        }

        public async Task<MovieDTO> UpdateMovie(UpdateMovieDTO updateMovieDTO)
        {
            try
            {
                var movie = (await _repository.GetAll()).FirstOrDefault(m => m.Title == updateMovieDTO.Title);
                if (movie == null)
                {
                    throw new MovieNotFoundException($"Movie {updateMovieDTO.Title} doesnt exist");
                }

                _mapper.Map(updateMovieDTO, movie);

                if (!string.IsNullOrEmpty(updateMovieDTO.Title))
                {
                    movie.Title = updateMovieDTO.Title;
                }
                if (updateMovieDTO.Duration > 0)
                {
                    movie.Duration = updateMovieDTO.Duration;
                }
                if (updateMovieDTO.StartDate != DateTime.MinValue)
                {
                    movie.StartDate = (DateTime)updateMovieDTO.StartDate;
                }
                if (updateMovieDTO.EndDate != DateTime.MinValue)
                {
                    movie.EndDate = (DateTime)updateMovieDTO.EndDate;
                }
                if (!string.IsNullOrEmpty(updateMovieDTO.Status))
                {
                    if (Enum.TryParse(updateMovieDTO.Status, out MovieStatus status))
                    {
                        movie.Status = status;
                    }
                }

                var updatedMovie = await _repository.Update(movie);

                var updatedMovieDTO = _mapper.Map<MovieDTO>(updatedMovie);
                return updatedMovieDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the movie");
                throw new UnableToUpdateMovieException("Unable to update movie. "+ex.Message);
            }
        }
    }
}
