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

        public SeatService(IRepository<int, Seat> repository, ILogger<SeatService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task AddSeats(Showtime showtime)
        {
            try
            {
                var seatsPerRow = 10;
                var noOfRows = showtime.TotalSeats / seatsPerRow;

                for (int i = 0; i < noOfRows; i++)
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
            }
            catch (Exception ex)
            {
                throw new UnableToAddSeatException($"Unable to add seat. {ex.Message}");
            }
        }

        public async Task DeleteSeats(int showtimeId)
        {
            try
            {
                var seats = (await _repository.GetAll()).Where(s => s.ShowetimeId == showtimeId);
                if (seats.Count() <= 0)
                {
                    throw new NoSeatsFoundException($"No seats found for showtime ID {showtimeId}");
                }
                await _repository.DeleteRange(showtimeId);
            }
            catch(Exception ex)
            {
                throw new UnableToDeleteSeatException($"Unable to delete seats. {ex.Message}");
            }
        }

        public async Task<SeatDTO> GetSeatById(int seatId)
        {
            try
            {
                var seat = await _repository.GetById(seatId);
                if(seat == null)
                {
                    throw new NoSuchSeatException($"No seat with ID {seatId} was found");
                }
                SeatDTO seatDTO = _mapper.Map<SeatDTO>(seat);
                return seatDTO;
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"No seat found. {ex}");
                throw new NoSuchSeatException($"No seat found. {ex.Message}");
            }
        }

        public async Task<bool> UpdateSeat(UpdateSeatDTO updateSeatDTO)
        {
            try
            {
                var seat = await _repository.GetById(updateSeatDTO.Id);
                if(seat == null)
                {
                    throw new NoSuchSeatException($"No seat with ID {updateSeatDTO.Id} was found");
                }
                seat.SeatStatus = (SeatStatus)Enum.Parse(typeof(SeatStatus), updateSeatDTO.SeatStatus);
                seat.IsAvailable = updateSeatDTO.IsAvailable;
                seat.BookingId = updateSeatDTO.BookingId;

                var newSeat = await _repository.Update(seat);
                if(newSeat == null)
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

        public async Task<SeatDTO> UpdateSeatStatus(UpdateSeatStatusDTO updateSeatStatusDTO)
        {
            var seat = await _repository.GetById(updateSeatStatusDTO.Id);
            if(seat == null)
            {
                throw new NoSuchSeatException($"No seat with ID {updateSeatStatusDTO.Id} found");
            }

            seat.SeatStatus = (SeatStatus)Enum.Parse(typeof(SeatStatus), updateSeatStatusDTO.SeatStatus);
            if(updateSeatStatusDTO.BookingId != null)
            {
                seat.BookingId = updateSeatStatusDTO.BookingId;
            }

            var result = await _repository.Update(seat);

            SeatDTO seatDTO = _mapper.Map<SeatDTO>(result);
            return seatDTO;
        }
    }
}
