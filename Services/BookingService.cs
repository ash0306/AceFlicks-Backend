using AutoMapper;
using MovieBookingBackend.Exceptions.Booking;
using MovieBookingBackend.Exceptions.Seat;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Bookings;
using MovieBookingBackend.Models.DTOs.Seats;
using MovieBookingBackend.Models.DTOs.Showtimes;
using MovieBookingBackend.Models.Enums;

namespace MovieBookingBackend.Services
{
    public class BookingService : IBookingService
    {
        private readonly IRepository<int, Booking> _repository;
        private readonly IRepository<int, User> _userRepository;
        private readonly ILogger<BookingService> _logger;
        private readonly IMapper _mapper;
        private readonly ISeatService _seatService;

        public BookingService(IRepository<int, Booking> repository, IRepository<int, User> userRepository, ILogger<BookingService> logger, IMapper mapper, ISeatService seatService)
        {
            _repository = repository;
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
            _seatService = seatService;
        }

        public async Task<BookingDTO> AddBooking(AddBookingDTO addBookingDTO)
        {
            try
            {
                await _userRepository.GetById(addBookingDTO.UserId);
                if(addBookingDTO.Seats.Count() > 5 )
                {
                    throw new MaxSeatLimitException("Cannot place order as the maximum number of tickets has to be less than 5");
                }
                Booking booking = AddBookingDTOtoBooking(addBookingDTO);
                
                foreach (var seatId in addBookingDTO.Seats)
                {
                    SeatDTO seat = await _seatService.GetSeatById(seatId);
                    if(seat.SeatStatus == "Reserved")
                    {
                        throw new SeatHasBeenReservedException($"Seat with ID {seat.Id} has been reserved/blocked by another user. Please reselect your seats and try again.");
                    }
                }
                booking.Status = BookingStatus.Booked;

                var newBooking = await _repository.Add(booking);
                foreach (var seatId in addBookingDTO.Seats)
                {
                    UpdateSeatDTO updateSeatDTO = new UpdateSeatDTO()
                    {
                        Id = seatId,
                        SeatStatus = SeatStatus.Booked.ToString(),
                        IsAvailable = false,
                        BookingId = newBooking.Id
                    };
                    var result = await _seatService.UpdateSeat(updateSeatDTO);
                    if (!result)
                    {
                        newBooking.Status = BookingStatus.Failed;
                        await _repository.Update(newBooking);
                        throw new UnableToUpdateSeatException("Could not update seat while making the booking.");
                    }
                }
                BookingDTO bookingDTO = _mapper.Map<BookingDTO>(newBooking);
                bookingDTO.ShowtimeDetails = GetShowtimeDetails(newBooking.Showtime);
                bookingDTO.Seats = GetSeatNumbers(newBooking.Seats);

                return bookingDTO;
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Unable to add booking. {ex}");
                throw new UnableToAddBookingException($"Unable to add booking at the moment. {ex.Message}");
            }
        }

        private Booking AddBookingDTOtoBooking(AddBookingDTO addBookingDTO)
        {
            Booking booking = new Booking()
            {
                UserId = addBookingDTO.UserId,
                ShowtimeId = addBookingDTO.ShowtimeId,
                TotalPrice = addBookingDTO.TotalPrice
            };
            return booking;
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookings()
        {
            try
            {
                var bookings = await _repository.GetAll();
                if(bookings.Count() <=0)
                {
                    throw new NoBookingsFoundException($"No bookings found");
                }

                IList<BookingDTO> bookingDTOs = new List<BookingDTO>();
                foreach(var booking in bookings)
                {
                    BookingDTO bookingDTO = _mapper.Map<BookingDTO>(booking);
                    bookingDTO.ShowtimeDetails = GetShowtimeDetails(booking.Showtime);
                    bookingDTO.Seats = GetSeatNumbers(booking.Seats);
                    bookingDTOs.Add(bookingDTO);
                }

                return bookingDTOs;
            }
            catch(Exception ex )
            {
                _logger.LogCritical($"No bookings found. {ex}");
                throw new NoBookingsFoundException($"No bookings found. {ex.Message}");
            }
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsByUserId(int userId)
        {
            try
            {
                var bookings = (await _repository.GetAll()).Where(b => b.UserId == userId);
                if (bookings.Count() <= 0)
                {
                    throw new NoBookingsFoundException($"No bookings found for user with Id {userId}");
                }

                IList<BookingDTO> bookingDTOs = new List<BookingDTO>();
                foreach (var booking in bookings)
                {
                    BookingDTO bookingDTO = _mapper.Map<BookingDTO>(booking);
                    bookingDTO.ShowtimeDetails = GetShowtimeDetails(booking.Showtime);
                    bookingDTO.Seats = GetSeatNumbers(booking.Seats);
                    bookingDTOs.Add(bookingDTO);
                }

                return bookingDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No bookings found for user with Id {userId}. {ex}");
                throw new NoBookingsFoundException($"No bookings found for user with Id {userId}. {ex.Message}");
            }
        }

        public async Task<BookingDTO> GetBookingById(int id)
        {
            try
            {
                var booking = await _repository.GetById(id);
                if (booking == null)
                {
                    throw new NoSuchBookingException($"No booking found with ID {id}");
                }
                BookingDTO bookingDTO = _mapper.Map<BookingDTO>(booking);
                bookingDTO.ShowtimeDetails = GetShowtimeDetails(booking.Showtime);
                bookingDTO.Seats = GetSeatNumbers(booking.Seats);

                return bookingDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"No booking found with ID {id}. {ex}");
                throw new NoBookingsFoundException($"No booking found with ID {id}. {ex.Message}");
            }
        }

        public async Task<BookingDTO> UpdateBookingStatus(BookingStatusDTO bookingStatusDTO)
        {
            try
            {
                var booking = await _repository.GetById(bookingStatusDTO.Id);
                var seats = booking.Seats.ToList();
                if(bookingStatusDTO.Status == "Cancelled"){
                    if (booking.Showtime.StartTime < DateTime.Now.AddHours(-6)){
                        CancelBookingSeats(seats);
                    }
                    throw new UnableToDeleteBookingException("Unable to cancel booking as the booking is scheduled in less than 6 hours");
                }
                else if (bookingStatusDTO.Status == "Failed"){
                    CancelBookingSeats(seats);
                }
                else{
                    ResetSeatsStatus(seats, bookingStatusDTO);
                }

                booking.Status = (BookingStatus)Enum.Parse(typeof(BookingStatus), bookingStatusDTO.Status);
                var result = _repository.Update(booking);

                BookingDTO bookingDTO = _mapper.Map<BookingDTO>(booking);
                bookingDTO.ShowtimeDetails = GetShowtimeDetails(booking.Showtime);
                bookingDTO.Seats = GetSeatNumbers(booking.Seats);

                return bookingDTO;
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Could not update booking with ID {bookingStatusDTO.Id}. {ex}");
                throw new UnableToUpdateBookingException($"Could not update booking with ID {bookingStatusDTO.Id}. {ex.Message}");
            }
        }

        private async void CancelBookingSeats(List<Seat> seats)
        {
            foreach (var seat in seats)
            {
                UpdateSeatDTO updateSeatDTO = new UpdateSeatDTO()
                {
                    Id = seat.Id,
                    SeatStatus = SeatStatus.Available.ToString(),
                    IsAvailable = true,
                    BookingId = null
                };
                await _seatService.UpdateSeat(updateSeatDTO);
            }
        }
        private async void ResetSeatsStatus(List<Seat> seats, BookingStatusDTO bookingStatusDTO)
        {
            foreach (var seat in seats)
            {
                UpdateSeatDTO updateSeatDTO = new UpdateSeatDTO()
                {
                    Id = seat.Id,
                    SeatStatus = bookingStatusDTO.Status,
                    IsAvailable = true,
                    BookingId = null
                };
                await _seatService.UpdateSeat(updateSeatDTO);
            }
        }
        private ICollection<string> GetShowtimeDetails(Showtime showtime)
        {
            IList<string> showtimeDetails = new List<string>()
            {
                $"Show Start Time: {showtime.StartTime}",
                $"Movie: {showtime.Movie?.Title}",
                $"Theatre: {showtime.Theatre?.Name}"
            };

            return showtimeDetails;
        }
        private ICollection<string> GetSeatNumbers(ICollection<Seat> seats)
        {
            IList<string> seatNumbers = new List<string>();
            foreach(var seat in seats)
            {
                seatNumbers.Add($"{seat.Row}{seat.SeatNumber}");
            }
            return seatNumbers;
        }
    }
}
