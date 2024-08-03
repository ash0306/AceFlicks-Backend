using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Seats;

namespace MovieBookingBackend.Interfaces
{
    public interface ISeatService
    {
        public Task AddSeats(Showtime showtime);
        public Task<SeatDTO> UpdateSeatStatus(UpdateSeatStatusDTO updateSeatStatusDTO);
        public Task DeleteSeats(int showtimeId);
        public Task<bool> UpdateSeat(UpdateSeatDTO updateSeatDTO);
        public Task<SeatDTO> GetSeatById(int seatId);
    }
}
