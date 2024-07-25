using AutoMapper;
using MovieBookingBackend.Exceptions.Movie;
using MovieBookingBackend.Exceptions.Showtime;
using MovieBookingBackend.Exceptions.Theatre;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Showtimes;
using MovieBookingBackend.Models.DTOs.Theatres;

namespace MovieBookingBackend.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly IRepository<int, Theatre> _repository;
        private readonly ILogger<TheatreService> _logger;
        private readonly IMapper _mapper;

        public TheatreService(IRepository<int, Theatre> repository, ILogger<TheatreService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<TheatreDTO> AddTheatre(TheatreDTO theatreDTO)
        {
            try
            {
                var alreadyExists = (await _repository.GetAll()).Where(t => t.Name == theatreDTO.Name).ToList();
                if (alreadyExists.Any())
                {
                    foreach (var item in alreadyExists)
                    {
                        if(item.Location == theatreDTO.Location)
                        {
                            _logger.LogCritical($"Theatre {theatreDTO.Name} already exists with location {theatreDTO.Location}");
                            throw new TheatreAlreadyExistsException($"Theatre {theatreDTO.Name} already exists with location {theatreDTO.Location}");
                        }
                    }
                }
                Theatre theatre = _mapper.Map<Theatre>(theatreDTO);
                var newTheatre = await _repository.Add(theatre);

                if(newTheatre == null)
                {
                    _logger.LogCritical($"Unable to add theatre.");
                    throw new UnableToAddTheatreException("Unable to add theatre at the moment.");
                }
                return theatreDTO;
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Unable to add theatre. {ex}");
                throw new UnableToAddTheatreException($"Unable to add theatre at the moment. {ex.Message}");
            }
        }

        public async Task<IEnumerable<TheatreDTO>> GetAllTheatres()
        {
            try
            {
                IEnumerable<Theatre> theatres = await _repository.GetAll();
                if (theatres.Count() <= 0)
                {
                    _logger.LogCritical("No theatres found");
                    throw new NoTheatresFoundException("No theatres found");
                }

                List<TheatreDTO> returnTheatres = new List<TheatreDTO>();
                foreach (var item in theatres)
                {
                    var theatreDTO = _mapper.Map<TheatreDTO>(item);
                    returnTheatres.Add(theatreDTO);
                }

                return returnTheatres;
            }
            catch(Exception ex)
            {
                _logger.LogCritical("Unable to fetch theatres" + ex.Message);
                throw new NoTheatresFoundException("Unable to fetch theatres" + ex.Message);
            }
        }

        public async Task<IEnumerable<IGrouping<int, ShowtimeDTO>>> GetShowtimesForATheatre(string theatreName)
        {
            try
            {
                var theatre = (await _repository.GetAll()).FirstOrDefault(t => t.Name == theatreName);
                var showtimes = theatre.Showtimes.ToList();
                IList<ShowtimeDTO> showtimeDTOs = new List<ShowtimeDTO>();
                foreach (var item in showtimes)
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

        public async Task<TheatreDTO> GetTheatreById(int id)
        {
            try
            {
                Theatre theatre = await _repository.GetById(id);
                if (theatre == null)
                {
                    _logger.LogCritical($"No theatre with ID {id} found");
                    throw new NoSuchTheatreException($"No theatre with ID {id} found");
                }

                TheatreDTO theatreDTO = _mapper.Map<TheatreDTO>(theatre);
                
                return theatreDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unable to fetch theatres" + ex.Message);
                throw new NoTheatresFoundException("Unable to fetch theatres" + ex.Message);
            }
        }

        public async Task<IEnumerable<string>> GetTheatreLocationsByName(string theatreName)
        {
            try
            {
                IList<string> locations = new List<string>();
                var theatres = (await _repository.GetAll()).Where(t => t.Name == theatreName);
                if(theatres.Count() <= 0)
                {
                    _logger.LogCritical($"No threatres with name {theatreName} were found!!");
                    throw new NoTheatresFoundException($"No threatres with name {theatreName} were found!!");
                }

                foreach (var item in theatres)
                {
                    locations.Add(item.Location);
                }
                return locations;
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex, ex.Message, "Unable to fetch details at the moment");
                throw new NoTheatresFoundException($"No threatres with name {theatreName} were found!!");
            }
        }

        public async Task<TheatreDTO> UpdateTheatre(UpdateTheatreDTO updateTheatreDTO)
        {
            try
            {
                Theatre oldTheatre = null;
                var alreadyExists = (await _repository.GetAll()).Where(t => t.Name == updateTheatreDTO.Name).ToList();
                if (alreadyExists.Any())
                {
                    foreach (var item in alreadyExists)
                    {
                        if (item.Location == updateTheatreDTO.NewLocation)
                        {
                            _logger.LogCritical($"Theatre {updateTheatreDTO.Name} already exists with location {updateTheatreDTO.NewLocation}");
                            throw new TheatreAlreadyExistsException($"Theatre {updateTheatreDTO.Name} already exists with location {updateTheatreDTO.NewLocation}");
                        }
                        if(item.Location == updateTheatreDTO.OldLocation)
                        {
                            oldTheatre = item;
                            break;
                        }
                    }
                }
                if(oldTheatre != null)
                {
                    oldTheatre.Location = updateTheatreDTO.NewLocation;

                    var result = await _repository.Update(oldTheatre);
                    TheatreDTO theatreDTO = _mapper.Map<TheatreDTO>(result);

                    return theatreDTO;
                }
                throw new NoTheatresFoundException("No theatres with the details are found");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the theatre details");
                throw new UnableToUpdateMovieException("Unable to update theatre. " + ex.Message);
            }
        }
    }
}
