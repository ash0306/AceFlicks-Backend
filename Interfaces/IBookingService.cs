using MovieBookingBackend.Models.DTOs.Bookings;

namespace MovieBookingBackend.Interfaces
{
    public interface IBookingService
    {
        public Task<BookingDTO> AddBooking(AddBookingDTO addBookingDTO);
        public Task<IEnumerable<BookingDTO>> GetAllBookings();
        public Task<IEnumerable<BookingDTO>> GetAllBookingsByUserId(int userId);
        public Task<BookingDTO> GetBookingById(int id);
        public Task<BookingDTO> UpdateBookingStatus(BookingStatusDTO bookingStatusDTO);
    }
}
