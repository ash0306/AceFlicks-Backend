using AutoMapper;
using Microsoft.AspNetCore.Http;
using MovieBookingBackend.Exceptions.Seat;
using MovieBookingBackend.Exceptions.Showtime;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Seats;
using MovieBookingBackend.Models.DTOs.Showtimes;

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

        public async Task<IEnumerable<ShowtimeDTO>> GetAllShowtime()
        {
            try
            {
                var results = await _repository.GetAll();
                var upcomigShowtimes = results.Where(s => s.StartTime > DateTime.Now);
                IList<ShowtimeDTO> showtimes = new List<ShowtimeDTO>();
                foreach (var item in upcomigShowtimes)
                {
                    var showtimeDTO = _mapper.Map<ShowtimeDTO>(item);
                    showtimes.Add(showtimeDTO);
                }
                return showtimes;
            }
            catch (Exception ex )
            {
                _logger.LogCritical($"No showtimes were found. {ex.Message}");
                throw new NoShowtimesFoundException($"No showtimes were found. {ex.Message}");
            }
        }

        public async Task<ShowtimeDTO> UpdateShowtime(UpdateShowtimeDTO updateShowtimeDTO)
        {
            try
            {
                var showtime = await _repository.GetById(updateShowtimeDTO.Id);
                if(showtime == null)
                {
                    throw new NoSuchShowtimeException($"No showtime with the given details was found");
                }
                if(showtime.AvailableSeats < showtime.TotalSeats)
                {
                    throw new UnableToUpdateShowtimeException("Unable to update showtime because tickets have already been booked for this showtime");
                }
                _mapper.Map(updateShowtimeDTO, showtime);

                var newShowtime = await _repository.Update(showtime);
                _seatService.DeleteSeats(newShowtime.Id);
                _seatService.AddSeats(newShowtime);

                ShowtimeDTO returnDTO = _mapper.Map<ShowtimeDTO>(newShowtime);
                return returnDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message, "Unable to update at the moment.");
                throw new UnableToUpdateShowtimeException($"Unable to update showtime at the moment. {ex.Message}");
            }
        }

        public async Task<IEnumerable<IGrouping<int, ShowtimeDTO>>> GetShowtimesForAMovie(string movieName)
        {
            try
            {
                var movie = (await _movieRepository.GetAll()).FirstOrDefault(m => m.Title == movieName);
                var showtimes = movie.Showtimes.ToList();
                var upcomigShowtimes = showtimes.Where(s => s.StartTime > DateTime.Now);
                IList<ShowtimeDTO> showtimeDTOs = new List<ShowtimeDTO>();
                foreach (var item in upcomigShowtimes)
                {
                    var showtime = _mapper.Map<ShowtimeDTO>(item);
                    showtimeDTOs.Add(showtime);
                }
                var result = showtimeDTOs.GroupBy(s => s.TheatreId);
                return result;
            }
            catch (Exception ex)
            {
                throw new NoShowtimesFoundException($"No showtimes for movie {movieName} were found");
            }
        }

        public async Task<IEnumerable<IGrouping<int, ShowtimeDTO>>> GetShowtimesForATheatre(string theatreName)
        {
            try
            {
                var theatre = (await _theatreRepository.GetAll()).FirstOrDefault(t => t.Name == theatreName);
                var showtimes = theatre.Showtimes.ToList();
                var upcomigShowtimes = showtimes.Where(s => s.StartTime > DateTime.Now);
                IList<ShowtimeDTO> showtimeDTOs = new List<ShowtimeDTO>();
                foreach (var item in upcomigShowtimes)
                {
                    var showtime = _mapper.Map<ShowtimeDTO>(item);
                    showtimeDTOs.Add(showtime);
                }
                var result = showtimeDTOs.GroupBy(s => s.MovieId);
                return result;
            }
            catch (Exception ex)
            {
                throw new NoShowtimesFoundException($"No showtimes for theatre {theatreName} were found");
            }
        }

        public async Task<IEnumerable<SeatDTO>> GetSeatsByShowtime(int showtimeId)
        {
            try
            {
                var showtime = await _repository.GetById(showtimeId);
                if(showtime == null)
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
                _logger.LogCritical("No seats found for the showtime");
                throw new NoSeatsFoundException("No seats for the given shwotime");
            }
        }
    }
}
