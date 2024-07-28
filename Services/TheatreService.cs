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

        /// <summary>
        /// Initializes a new instance of the <see cref="TheatreService"/> class.
        /// </summary>
        /// <param name="repository">The repository for theatres.</param>
        /// <param name="logger">The logger for the service.</param>
        /// <param name="mapper">The mapper for DTOs.</param>
        public TheatreService(IRepository<int, Theatre> repository, ILogger<TheatreService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds a new theatre.
        /// </summary>
        /// <param name="theatreDTO">The DTO containing theatre details.</param>
        /// <returns>The added theatre DTO.</returns>
        /// <exception cref="TheatreAlreadyExistsException">Thrown when the theatre already exists with the same location.</exception>
        /// <exception cref="UnableToAddTheatreException">Thrown when there is an error adding the theatre.</exception>
        public async Task<TheatreDTO> AddTheatre(TheatreDTO theatreDTO)
        {
            try
            {
                var alreadyExists = (await _repository.GetAll()).Where(t => t.Name == theatreDTO.Name).ToList();
                if (alreadyExists.Any())
                {
                    foreach (var item in alreadyExists)
                    {
                        if (item.Location == theatreDTO.Location)
                        {
                            _logger.LogCritical($"Theatre {theatreDTO.Name} already exists with location {theatreDTO.Location}");
                            throw new TheatreAlreadyExistsException($"Theatre {theatreDTO.Name} already exists with location {theatreDTO.Location}");
                        }
                    }
                }

                Theatre theatre = _mapper.Map<Theatre>(theatreDTO);
                var newTheatre = await _repository.Add(theatre);

                if (newTheatre == null)
                {
                    _logger.LogCritical("Unable to add theatre.");
                    throw new UnableToAddTheatreException("Unable to add theatre at the moment.");
                }

                return theatreDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unable to add theatre. {ex}");
                throw new UnableToAddTheatreException($"Unable to add theatre at the moment. {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all theatres.
        /// </summary>
        /// <returns>A list of theatre DTOs.</returns>
        /// <exception cref="NoTheatresFoundException">Thrown when no theatres are found.</exception>
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
            catch (Exception ex)
            {
                _logger.LogCritical("Unable to fetch theatres" + ex.Message);
                throw new NoTheatresFoundException("Unable to fetch theatres" + ex.Message);
            }
        }

        /// <summary>
        /// Gets a theatre by ID.
        /// </summary>
        /// <param name="id">The ID of the theatre.</param>
        /// <returns>The theatre DTO.</returns>
        /// <exception cref="NoSuchTheatreException">Thrown when the theatre is not found.</exception>
        /// <exception cref="NoTheatresFoundException">Thrown when an error occurs while fetching the theatre.</exception>
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

        /// <summary>
        /// Gets locations for theatres by name.
        /// </summary>
        /// <param name="theatreName">The name of the theatre.</param>
        /// <returns>A list of locations.</returns>
        /// <exception cref="NoTheatresFoundException">Thrown when no theatres are found with the given name.</exception>
        public async Task<IEnumerable<string>> GetTheatreLocationsByName(string theatreName)
        {
            try
            {
                IList<string> locations = new List<string>();
                var theatres = (await _repository.GetAll()).Where(t => t.Name == theatreName);
                if (theatres.Count() <= 0)
                {
                    _logger.LogCritical($"No theatres with name {theatreName} were found!");
                    throw new NoTheatresFoundException($"No theatres with name {theatreName} were found!");
                }

                foreach (var item in theatres)
                {
                    locations.Add(item.Location);
                }
                return locations;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message, "Unable to fetch details at the moment");
                throw new NoTheatresFoundException($"No theatres with name {theatreName} were found!");
            }
        }

        /// <summary>
        /// Updates a theatre.
        /// </summary>
        /// <param name="updateTheatreDTO">The DTO containing updated theatre details.</param>
        /// <returns>The updated theatre DTO.</returns>
        /// <exception cref="TheatreAlreadyExistsException">Thrown when a theatre with the same name and new location already exists.</exception>
        /// <exception cref="NoTheatresFoundException">Thrown when no theatres with the given details are found.</exception>
        /// <exception cref="UnableToUpdateTheatreException">Thrown when there is an error updating the theatre.</exception>
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
                        if (item.Location == updateTheatreDTO.OldLocation)
                        {
                            oldTheatre = item;
                            break;
                        }
                    }
                }
                if (oldTheatre != null)
                {
                    oldTheatre.Location = updateTheatreDTO.NewLocation;

                    var result = await _repository.Update(oldTheatre);
                    TheatreDTO theatreDTO = _mapper.Map<TheatreDTO>(result);

                    return theatreDTO;
                }
                throw new NoTheatresFoundException("No theatres with the details are found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the theatre details");
                throw new UnableToUpdateTheatreException("Unable to update theatre. " + ex.Message);
            }
        }
    }
}
