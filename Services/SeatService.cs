using AutoMapper;
using MovieBookingBackend.Exceptions.Seat;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Seats;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Repositories;

namespace MovieBookingBackend.Services
{
    public class SeatService : ISeatService
    {
        private readonly IRepository<int, Seat> _repository;
        private readonly ILogger<SeatService> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeatService"/> class.
        /// </summary>
        /// <param name="repository">The repository for seats.</param>
        /// <param name="logger">The logger for the service.</param>
        /// <param name="mapper">The mapper for DTOs.</param>
        public SeatService(IRepository<int, Seat> repository, ILogger<SeatService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds seats for a given showtime.
        /// </summary>
        /// <param name="showtime">The showtime for which seats are being added.</param>
        /// <exception cref="UnableToAddSeatException">Thrown when there is an error adding seats.</exception>
        public async Task AddSeats(Showtime showtime)
        {
            try
            {
                var seatsPerRow = 10;
                var totalSeats = showtime.TotalSeats;
                var fullRows = totalSeats / seatsPerRow;
                var remainingSeats = totalSeats % seatsPerRow;

                for (int i = 0; i < fullRows; i++)
                {
                    char row = (char)('A' + i);
                    for (int j = 1; j <= seatsPerRow; j++)
                    {
                        Seat seat = new Seat()
                        {
                            Row = row.ToString(),
                            SeatNumber = j,
                            SeatStatus = SeatStatus.Available,
                            IsAvailable = true,
                            ShowetimeId = showtime.Id
                        };
                        await _repository.Add(seat);
                    }
                }

                if (remainingSeats > 0)
                {
                    char row = (char)('A' + fullRows);
                    for (int j = 1; j <= remainingSeats; j++)
                    {
                        Seat seat = new Seat()
                        {
                            Row = row.ToString(),
                            SeatNumber = j,
                            SeatStatus = SeatStatus.Available,
                            IsAvailable = true,
                            ShowetimeId = showtime.Id
                        };
                        await _repository.Add(seat);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unable to add seat. {ex.Message}");
                throw new UnableToAddSeatException($"Unable to add seat. {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes seats for a given showtime.
        /// </summary>
        /// <param name="showtimeId">The showtime ID for which seats are being deleted.</param>
        /// <exception cref="NoSeatsFoundException">Thrown when no seats are found for the showtime.</exception>
        /// <exception cref="UnableToDeleteSeatException">Thrown when there is an error deleting seats.</exception>
        public async Task DeleteSeats(int showtimeId)
        {
            try
            {
                var seats = (await _repository.GetAll()).Where(s => s.ShowetimeId == showtimeId);
                if (!seats.Any())
                {
                    throw new NoSeatsFoundException($"No seats found for showtime ID {showtimeId}");
                }
                await _repository.DeleteRange(showtimeId);
            }
            catch (Exception ex)
            {
                throw new UnableToDeleteSeatException($"Unable to delete seats. {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a seat by its ID.
        /// </summary>
        /// <param name="seatId">The seat ID.</param>
        /// <returns>The seat DTO.</returns>
        /// <exception cref="NoSuchSeatException">Thrown when the seat is not found.</exception>
        public async Task<SeatDTO> GetSeatById(int seatId)
        {
            try
            {
                var seat = await _repository.GetById(seatId);
                if (seat == null)
                {
                    throw new NoSuchSeatException($"No seat with ID {seatId} was found");
                }
                SeatDTO seatDTO = _mapper.Map<SeatDTO>(seat);
                return seatDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No seat found. {ex}");
                throw new NoSuchSeatException($"No seat found. {ex.Message}");
            }
        }

        /// <summary>
        /// Updates a seat.
        /// </summary>
        /// <param name="updateSeatDTO">The DTO containing the seat update information.</param>
        /// <returns>True if the update is successful, otherwise false.</returns>
        /// <exception cref="NoSuchSeatException">Thrown when the seat is not found.</exception>
        /// <exception cref="UnableToUpdateSeatException">Thrown when there is an error updating the seat.</exception>
        public async Task<bool> UpdateSeat(UpdateSeatDTO updateSeatDTO)
        {
            try
            {
                var seat = await _repository.GetById(updateSeatDTO.Id);
                if (seat == null)
                {
                    throw new NoSuchSeatException($"No seat with ID {updateSeatDTO.Id} was found");
                }
                seat.SeatStatus = (SeatStatus)Enum.Parse(typeof(SeatStatus), updateSeatDTO.SeatStatus);
                seat.IsAvailable = updateSeatDTO.IsAvailable;
                seat.BookingId = updateSeatDTO.BookingId;

                var newSeat = await _repository.Update(seat);
                if (newSeat == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unable to update seat. {ex}");
                throw new UnableToUpdateSeatException($"Unable to update seat at the moment. {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the status of a seat.
        /// </summary>
        /// <param name="updateSeatStatusDTO">The DTO containing the seat status update information.</param>
        /// <returns>The updated seat DTO.</returns>
        /// <exception cref="NoSuchSeatException">Thrown when the seat is not found.</exception>
        public async Task<SeatDTO> UpdateSeatStatus(UpdateSeatStatusDTO updateSeatStatusDTO)
        {
            var seat = await _repository.GetById(updateSeatStatusDTO.Id);
            if (seat == null)
            {
                throw new NoSuchSeatException($"No seat with ID {updateSeatStatusDTO.Id} found");
            }

            seat.SeatStatus = (SeatStatus)Enum.Parse(typeof(SeatStatus), updateSeatStatusDTO.SeatStatus);
            if (updateSeatStatusDTO.BookingId != null)
            {
                seat.BookingId = updateSeatStatusDTO.BookingId;
            }

            var result = await _repository.Update(seat);

            SeatDTO seatDTO = _mapper.Map<SeatDTO>(result);
            return seatDTO;
        }
    }
}
