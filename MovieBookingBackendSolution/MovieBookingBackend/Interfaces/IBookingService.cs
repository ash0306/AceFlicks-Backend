using MovieBookingBackend.Models.DTOs.Bookings;
using MovieBookingBackend.Models.DTOs.Seats;

namespace MovieBookingBackend.Interfaces
{
    public interface IBookingService
    {
        public Task<BookingDTO> AddBooking(AddBookingDTO addBookingDTO);
        public Task<bool> ResendBookingEmail(int bookingId);
        public Task<IEnumerable<BookingDTO>> GetAllBookings();
        public Task<IEnumerable<BookingDTO>> GetAllBookingsByUserId(int userId);
        public Task<BookingDTO> GetBookingById(int id);
        public Task<BookingDTO> UpdateBookingStatus(BookingStatusDTO bookingStatusDTO);
        public Task<bool> ReserveSeats(IEnumerable<int> seats);
        public Task<bool> FreeSeats(IEnumerable<int> seats);
    }
}
