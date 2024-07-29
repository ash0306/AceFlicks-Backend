using AutoMapper;
using Microsoft.AspNetCore.Http;
using MovieBookingBackend.Exceptions.Seat;
using MovieBookingBackend.Exceptions.Showtime;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Seats;
using MovieBookingBackend.Models.DTOs.Showtimes;
using MovieBookingBackend.Models.Enums;

namespace MovieBookingBackend.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IRepository<int, Showtime> _repository;
        private readonly IRepository<int, Movie> _movieRepository;
        private readonly IRepository<int, Theatre> _theatreRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowtimeService> _logger;
        private readonly ISeatService _seatService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowtimeService"/> class.
        /// </summary>
        /// <param name="repository">The repository for showtimes.</param>
        /// <param name="mapper">The mapper for DTOs.</param>
        /// <param name="logger">The logger for the service.</param>
        /// <param name="movieRepository">The repository for movies.</param>
        /// <param name="theatreRepository">The repository for theatres.</param>
        /// <param name="seatService">The seat service for managing seats.</param>
        public ShowtimeService(IRepository<int, Showtime> repository,
            IMapper mapper,
            ILogger<ShowtimeService> logger,
            IRepository<int, Movie> movieRepository,
            IRepository<int, Theatre> theatreRepository,
            ISeatService seatService)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _movieRepository = movieRepository;
            _theatreRepository = theatreRepository;
            _seatService = seatService;
        }

        /// <summary>
        /// Adds a new showtime.
        /// </summary>
        /// <param name="addShowtimeDTO">The DTO containing showtime details.</param>
        /// <returns>The added showtime DTO.</returns>
        /// <exception cref="ShowtimeAlreadyExistsException">Thrown when the showtime already exists.</exception>
        /// <exception cref="UnableToAddShowtimeException">Thrown when there is an error adding the showtime.</exception>
        public async Task<ShowtimeDTO> AddShowtime(AddShowtimeDTO addShowtimeDTO)
        {
            try
            {
                var existingShowtime = (await _repository.GetAll())
                    .Where(s => s.StartTime == addShowtimeDTO.StartTime
                             && s.EndTime == addShowtimeDTO.EndTime
                             && s.MovieId == addShowtimeDTO.MovieId
                             && s.TheatreId == addShowtimeDTO.TheatreId
                             && s.TotalSeats == addShowtimeDTO.TotalSeats
                             && s.TicketPrice == addShowtimeDTO.TicketPrice)
                    .FirstOrDefault();

                if (existingShowtime != null)
                {
                    _logger.LogInformation("Showtime already exists.");
                    throw new ShowtimeAlreadyExistsException($"Showtime already exists.");
                }

                var newShowtime = _mapper.Map<Showtime>(addShowtimeDTO);

                if (addShowtimeDTO.StartTime < DateTime.Now)
                {
                    newShowtime.Status = ShowtimeStatus.Inactive;
                }
                else
                {
                    newShowtime.Status = ShowtimeStatus.Active;
                }
                newShowtime.AvailableSeats = newShowtime.TotalSeats;

                var result = await _repository.Add(newShowtime);
                await _seatService.AddSeats(newShowtime);

                var showtimeDTO = _mapper.Map<ShowtimeDTO>(result);
                return showtimeDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding showtime");
                throw new UnableToAddShowtimeException($"Unable to add showtime exception. {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all active showtimes.
        /// </summary>
        /// <returns>A list of active showtime DTOs.</returns>
        /// <exception cref="NoShowtimesFoundException">Thrown when no showtimes are found.</exception>
        public async Task<IEnumerable<ShowtimeDTO>> GetAllShowtime()
        {
            try
            {
                var results = await _repository.GetAll();
                var upcomigShowtimes = results.Where(s => s.Status == ShowtimeStatus.Active);
                IList<ShowtimeDTO> showtimes = new List<ShowtimeDTO>();
                foreach (var item in upcomigShowtimes)
                {
                    var showtimeDTO = _mapper.Map<ShowtimeDTO>(item);
                    showtimeDTO.Status = item.Status.ToString();
                    showtimes.Add(showtimeDTO);
                }
                return showtimes;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No showtimes were found. {ex.Message}");
                throw new NoShowtimesFoundException($"No showtimes were found. {ex.Message}");
            }
        }

        /// <summary>
        /// Updates a showtime.
        /// </summary>
        /// <param name="updateShowtimeDTO">The DTO containing showtime update details.</param>
        /// <returns>The updated showtime DTO.</returns>
        /// <exception cref="NoSuchShowtimeException">Thrown when the showtime is not found.</exception>
        /// <exception cref="UnableToUpdateShowtimeException">Thrown when there is an error updating the showtime.</exception>
        public async Task<ShowtimeDTO> UpdateShowtime(UpdateShowtimeDTO updateShowtimeDTO)
        {
            try
            {
                var showtime = await _repository.GetById(updateShowtimeDTO.Id);
                if (showtime == null)
                {
                    throw new NoSuchShowtimeException($"No showtime with the given details was found");
                }
                if (showtime.AvailableSeats < showtime.TotalSeats)
                {
                    throw new UnableToUpdateShowtimeException("Unable to update showtime because tickets have already been booked for this showtime");
                }
                _mapper.Map(updateShowtimeDTO, showtime);

                var newShowtime = await _repository.Update(showtime);
                await _seatService.DeleteSeats(newShowtime.Id);
                await _seatService.AddSeats(newShowtime);

                ShowtimeDTO returnDTO = _mapper.Map<ShowtimeDTO>(newShowtime);
                return returnDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message, "Unable to update at the moment.");
                throw new UnableToUpdateShowtimeException($"Unable to update showtime at the moment. {ex.Message}");
            }
        }

        /// <summary>
        /// Gets showtimes for a specific movie.
        /// </summary>
        /// <param name="movieName">The name of the movie.</param>
        /// <returns>A list of showtime DTOs grouped by theatre ID.</returns>
        /// <exception cref="NoShowtimesFoundException">Thrown when no showtimes are found for the movie.</exception>
        public async Task<IEnumerable<ShowtimeGroupDTO>> GetShowtimesForAMovie(string movieName)
        {
            try
            {
                var movie = (await _movieRepository.GetAll()).FirstOrDefault(m => m.Title == movieName);
                if (movie == null)
                {
                    throw new NoShowtimesFoundException($"No showtimes for movie {movieName} were found");
                }
                var showtimes = movie.Showtimes.ToList();
                var upcomigShowtimes = showtimes.Where(s => s.Status == ShowtimeStatus.Active);
                IList<ShowtimeDTO> showtimeDTOs = new List<ShowtimeDTO>();
                foreach (var item in upcomigShowtimes)
                {
                    var showtime = _mapper.Map<ShowtimeDTO>(item);
                    showtime.Theatre = item.Theatre.Name;
                    showtime.Movie = item.Movie.Title;
                    showtimeDTOs.Add(showtime);
                }
                var groupedShowtimes = upcomigShowtimes
                    .GroupBy(s => s.Theatre.Name)
                    .Select(g => new ShowtimeGroupDTO
                    {
                        Name = g.Key,
                        Showtimes = g.Select(s => new ShowtimeDTO
                        {
                            Id = s.Id,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime,
                            Status = s.Status.ToString(),
                            Movie = s.Movie.Title,
                            Theatre = s.Theatre.Name,
                            TotalSeats = s.TotalSeats,
                            AvailableSeats = s.AvailableSeats,
                            TicketPrice = s.TicketPrice
                        }).ToList()
                    })
                    .ToList();
                return groupedShowtimes;
            }
            catch (Exception ex)
            {
                throw new NoShowtimesFoundException($"No showtimes for movie {movieName} were found");
            }
        }

        /// <summary>
        /// Gets showtimes for a specific theatre.
        /// </summary>
        /// <param name="theatreName">The name of the theatre.</param>
        /// <returns>A list of showtime DTOs grouped by movie ID.</returns>
        /// <exception cref="NoShowtimesFoundException">Thrown when no showtimes are found for the theatre.</exception>
        public async Task<IEnumerable<ShowtimeGroupDTO>> GetShowtimesForATheatre(string theatreName)
        {
            try
            {
                var theatre = (await _theatreRepository.GetAll()).FirstOrDefault(t => t.Name == theatreName);
                if (theatre == null)
                {
                    throw new NoShowtimesFoundException($"No showtimes for theatre {theatreName} were found");
                }
                var showtimes = theatre.Showtimes.ToList();
                var upcomigShowtimes = showtimes.Where(s => s.Status == ShowtimeStatus.Active);
                IList<ShowtimeDTO> showtimeDTOs = new List<ShowtimeDTO>();
                foreach (var item in upcomigShowtimes)
                {
                    var showtime = _mapper.Map<ShowtimeDTO>(item);
                    showtime.Theatre = item.Theatre.Name;
                    showtime.Movie = item.Movie.Title;
                    showtimeDTOs.Add(showtime);
                }
                var groupedShowtimes = upcomigShowtimes
                    .GroupBy(s => s.Movie.Title)
                    .Select(g => new ShowtimeGroupDTO
                    {
                        Name = g.Key,
                        Showtimes = g.Select(s => new ShowtimeDTO
                        {
                            Id = s.Id,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime,
                            Status = s.Status.ToString(),
                            Movie = s.Movie.Title,
                            Theatre = s.Theatre.Name,
                            TotalSeats = s.TotalSeats,
                            AvailableSeats = s.AvailableSeats,
                            TicketPrice = s.TicketPrice
                        }).ToList()
                    })
                    .ToList();
                return groupedShowtimes;
            }
            catch (Exception ex)
            {
                throw new NoShowtimesFoundException($"No showtimes for theatre {theatreName} were found");
            }
        }

        /// <summary>
        /// Gets seats for a specific showtime.
        /// </summary>
        /// <param name="showtimeId">The showtime ID.</param>
        /// <returns>A list of seat DTOs.</returns>
        /// <exception cref="NoSuchShowtimeException">Thrown when the showtime is not found.</exception>
        /// <exception cref="NoSeatsFoundException">Thrown when no seats are found for the showtime.</exception>
        public async Task<IEnumerable<SeatDTO>> GetSeatsByShowtime(int showtimeId)
        {
            try
            {
                var showtime = await _repository.GetById(showtimeId);
                if (showtime == null)
                {
                    throw new NoSuchShowtimeException($"No showtime with ID {showtimeId} found");
                }
                var seats = showtime.Seats.ToList();
                IList<SeatDTO> seatDTOs = new List<SeatDTO>();

                foreach (var seat in seats)
                {
                    var seatDTO = _mapper.Map<SeatDTO>(seat);
                    seatDTOs.Add(seatDTO);
                }
                return seatDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No seats found for the showtime. {ex}");
                throw new NoSeatsFoundException($"No seats for the given showtime. {ex.Message}");
            }
        }
    }
}
