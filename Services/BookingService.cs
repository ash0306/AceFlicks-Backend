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
        private readonly IRepository<int, Showtime> _showtimeRepository;
        private readonly ILogger<BookingService> _logger;
        private readonly IMapper _mapper;
        private readonly ISeatService _seatService;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IQRCodeService _qRCodeService;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="repository">Repository for Booking</param>
        /// <param name="userRepository">Repository for User</param>
        /// <param name="showtimeRepository">Repository for Showtime</param>
        /// <param name="logger">Logger for BookingService</param>
        /// <param name="mapper">Mapper for DTOs</param>
        /// <param name="seatService">Service for Seat operations</param>
        public BookingService(IRepository<int, Booking> repository, IQRCodeService qRCodeService, IEmailVerificationService emailVerificationService, IRepository<int, User> userRepository, IRepository<int, Showtime> showtimeRepository, ILogger<BookingService> logger, IMapper mapper, ISeatService seatService)
        {
            _repository = repository;
            _userRepository = userRepository;
            _showtimeRepository = showtimeRepository;
            _logger = logger;
            _mapper = mapper;
            _seatService = seatService;
            _emailVerificationService = emailVerificationService;
            _qRCodeService = qRCodeService;
        }

        /// <summary>
        /// Adds a new booking
        /// </summary>
        /// <param name="addBookingDTO">DTO containing booking details to be added</param>
        /// <returns>The newly added booking</returns>
        /// <exception cref="UnableToAddBookingException">Thrown if the booking cannot be added</exception>
        public async Task<BookingDTO> AddBooking(AddBookingDTO addBookingDTO)
        {
            try
            {
                await _userRepository.GetById(addBookingDTO.UserId);
                if (addBookingDTO.Seats.Count() > 5)
                {
                    throw new MaxSeatLimitException("Cannot place order as the maximum number of tickets has to be less than 5");
                }
                
                var showtime = await _showtimeRepository.GetById(addBookingDTO.ShowtimeId);
                showtime.AvailableSeats -= addBookingDTO.Seats.Count();
                await _showtimeRepository.Update(showtime);

                Booking booking = await AddBookingDTOtoBooking(addBookingDTO);
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

                int bookingsThisWeek = await GetBookingsCountThisWeek(addBookingDTO.UserId);
                BookingDTO bookingDTO = _mapper.Map<BookingDTO>(newBooking);
                bookingDTO.ShowtimeDetails = GetShowtimeDetails(newBooking.Showtime);
                bookingDTO.Seats = GetSeatNumbers(newBooking.Seats);

                byte[] bookingQR = await _qRCodeService.CreateQRCode(bookingDTO);
                await _emailVerificationService.SendBookingConfirmationEmail(bookingDTO.UserId, bookingDTO, bookingQR);
                
                if (bookingsThisWeek >= 3)
                {
                    string offerCode = GenerateOfferCode();
                    bookingDTO.OfferMessage = $"Congratulations! You have earned a free popcorn. Use code {offerCode} at the theatre to avail this offer.";
                    await _emailVerificationService.SendOfferCodeEmail(bookingDTO.UserId, offerCode);
                }

                return bookingDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unable to add booking. {ex}");
                throw new UnableToAddBookingException($"Unable to add booking at the moment. {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a random offer code
        /// </summary>
        /// <returns>A random 6-character offer code</returns>
        private string GenerateOfferCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] code = new char[6];

            for (int i = 0; i < 6; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            return new string(code);
        }

        /// <summary>
        /// Gets the count of bookings made by a user in the current week
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>Number of bookings made by the user in the current week</returns>
        private async Task<int> GetBookingsCountThisWeek(int userId)
        {
            var bookings = await _repository.GetAll();
            var currentWeekStart = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            var currentWeekEnd = currentWeekStart.AddDays(7);
            int bookingsThisWeek = bookings.Count(b => b.UserId == userId && b.BookingTime >= currentWeekStart && b.BookingTime < currentWeekEnd);
            return bookingsThisWeek;
        }

        /// <summary>
        /// Converts AddBookingDTO to Booking
        /// </summary>
        /// <param name="addBookingDTO">DTO containing booking details</param>
        /// <returns>A Booking object</returns>
        private async Task<Booking> AddBookingDTOtoBooking(AddBookingDTO addBookingDTO)
        {
            float totalPrice = await CalculateTotalPrice(addBookingDTO);
            Booking booking = new Booking()
            {
                UserId = addBookingDTO.UserId,
                ShowtimeId = addBookingDTO.ShowtimeId,
                TotalPrice = totalPrice
            };
            return booking;
        }

        /// <summary>
        /// Calculates the total price of the booking
        /// </summary>
        /// <param name="addBookingDTO">DTO containing booking details</param>
        /// <returns>Total price of the booking</returns>
        private async Task<float> CalculateTotalPrice(AddBookingDTO addBookingDTO)
        {
            int noOfSeats = addBookingDTO.Seats.Count();
            Showtime showtime = await _showtimeRepository.GetById(addBookingDTO.ShowtimeId);

            float totalPrice = showtime.TicketPrice * noOfSeats;
            float convenienceFee = (totalPrice * 10) / 100;
            float gst = (convenienceFee * 18) / 100;
            totalPrice = totalPrice * convenienceFee + gst;
            return totalPrice;
        }

        /// <summary>
        /// Gets a list of all bookings
        /// </summary>
        /// <returns>List of all bookings</returns>
        /// <exception cref="NoBookingsFoundException">Thrown if no bookings are found</exception>
        public async Task<IEnumerable<BookingDTO>> GetAllBookings()
        {
            try
            {
                var bookings = await _repository.GetAll();
                if (bookings.Count() <= 0)
                {
                    throw new NoBookingsFoundException($"No bookings found");
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
                _logger.LogCritical($"No bookings found. {ex}");
                throw new NoBookingsFoundException($"No bookings found. {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a list of all bookings for a user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>List of all bookings for the user</returns>
        /// <exception cref="NoBookingsFoundException">Thrown if no bookings are found for the user</exception>
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

        /// <summary>
        /// Gets a booking by its ID
        /// </summary>
        /// <param name="id">ID of the booking to be fetched</param>
        /// <returns>Booking with the specified ID</returns>
        /// <exception cref="NoSuchBookingException">Thrown if no booking is found with the specified ID</exception>
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

        /// <summary>
        /// Updates the status of a booking
        /// </summary>
        /// <param name="bookingStatusDTO">DTO containing booking status details</param>
        /// <returns>Updated booking</returns>
        /// <exception cref="UnableToUpdateBookingException">Thrown if the booking cannot be updated</exception>
        public async Task<BookingDTO> UpdateBookingStatus(BookingStatusDTO bookingStatusDTO)
        {
            try
            {
                var booking = await _repository.GetById(bookingStatusDTO.Id);
                var seats = booking.Seats.ToList();
                if (bookingStatusDTO.Status == "Cancelled")
                {
                    if (booking.Showtime.StartTime < DateTime.Now.AddHours(-6))
                    {
                        CancelBookingSeats(seats);
                    }
                    throw new UnableToDeleteBookingException("Unable to cancel booking as the booking is scheduled in less than 6 hours");
                }
                else if (bookingStatusDTO.Status == "Failed")
                {
                    CancelBookingSeats(seats);
                }
                else
                {
                    ResetSeatsStatus(seats, bookingStatusDTO);
                }

                booking.Status = (BookingStatus)Enum.Parse(typeof(BookingStatus), bookingStatusDTO.Status);
                var result = _repository.Update(booking);

                BookingDTO bookingDTO = _mapper.Map<BookingDTO>(booking);
                bookingDTO.ShowtimeDetails = GetShowtimeDetails(booking.Showtime);
                bookingDTO.Seats = GetSeatNumbers(booking.Seats);

                return bookingDTO;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Could not update booking with ID {bookingStatusDTO.Id}. {ex}");
                throw new UnableToUpdateBookingException($"Could not update booking with ID {bookingStatusDTO.Id}. {ex.Message}");
            }
        }

        /// <summary>
        /// Cancels the seats for a booking
        /// </summary>
        /// <param name="seats">List of seats to be cancelled</param>
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

        /// <summary>
        /// Resets the status of seats for a booking
        /// </summary>
        /// <param name="seats">List of seats to be reset</param>
        /// <param name="bookingStatusDTO">DTO containing booking status details</param>
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

        /// <summary>
        /// Gets the showtime details for a booking
        /// </summary>
        /// <param name="showtime">Showtime object</param>
        /// <returns>Collection of showtime details</returns>
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

        /// <summary>
        /// Gets the seat numbers for a booking
        /// </summary>
        /// <param name="seats">Collection of seats</param>
        /// <returns>Collection of seat numbers</returns>
        private ICollection<string> GetSeatNumbers(ICollection<Seat> seats)
        {
            IList<string> seatNumbers = new List<string>();
            foreach (var seat in seats)
            {
                seatNumbers.Add($"{seat.Row}{seat.SeatNumber}");
            }
            return seatNumbers;
        }

        public async Task<bool> ReserveSeats(IEnumerable<int> seats)
        {
            try
            {
                foreach (var id in seats)
                {
                    var seat = await _seatService.GetSeatById(id);
                    if(seat.SeatStatus == "Reserved")
                    {
                        throw new SeatHasBeenReservedException("The seat(s) you are trying to book has been already reserved by another user. Please try again!!");
                    }
                    UpdateSeatDTO newDto = new UpdateSeatDTO()
                    {
                        Id = seat.Id,
                        SeatStatus = SeatStatus.Reserved.ToString(),
                        IsAvailable = false
                    };
                    await _seatService.UpdateSeat(newDto);
                }
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogCritical("Unable to update seat. " + ex);
                throw new UnableToUpdateSeatException("Unable to update seat. " + ex.Message);
            }
        }

        public async Task<bool> FreeSeats(IEnumerable<int> seats)
        {
            try
            {
                foreach (var id in seats)
                {
                    var seat = await _seatService.GetSeatById(id);
                    UpdateSeatDTO newDto = new UpdateSeatDTO()
                    {
                        Id = seat.Id,
                        SeatStatus = SeatStatus.Available.ToString(),
                        IsAvailable = true
                    };
                    await _seatService.UpdateSeat(newDto);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unable to update seat. " + ex);
                throw new UnableToUpdateSeatException("Unable to update seat. " + ex.Message);
            }
        }
    }

    /// <summary>
    /// Extension methods for DateTime
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the start of the week for a given DateTime
        /// </summary>
        /// <param name="dt">The DateTime</param>
        /// <param name="startOfWeek">The start day of the week</param>
        /// <returns>The start of the week</returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}