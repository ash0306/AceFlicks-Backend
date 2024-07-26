using MovieBookingBackend.Models.DTOs.Seats;
using MovieBookingBackend.Models.DTOs.Showtimes;

namespace MovieBookingBackend.Interfaces
{
    public interface IShowtimeService
    {
        public Task<ShowtimeDTO> AddShowtime(AddShowtimeDTO addShowtimeDTO);
        public Task<ShowtimeDTO> UpdateShowtime(UpdateShowtimeDTO updateShowtimeDTO);
        public Task<IEnumerable<ShowtimeDTO>> GetAllShowtime();
        public Task<IEnumerable<IGrouping<int, ShowtimeDTO>>> GetShowtimesForAMovie(string movieName);
        public Task<IEnumerable<IGrouping<int, ShowtimeDTO>>> GetShowtimesForATheatre(string theatreName);
        public Task<IEnumerable<SeatDTO>> GetSeatsByShowtime(int showtimeId);
    }
}
