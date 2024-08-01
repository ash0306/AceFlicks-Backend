using MovieBookingBackend.Models.DTOs.Seats;
using MovieBookingBackend.Models.DTOs.Showtimes;

namespace MovieBookingBackend.Interfaces
{
    public interface IShowtimeService
    {
        public Task<ShowtimeDTO> AddShowtime(AddShowtimeDTO addShowtimeDTO);
        public Task<ShowtimeDTO> UpdateShowtime(UpdateShowtimeDTO updateShowtimeDTO);
        public Task<IEnumerable<ShowtimeDTO>> GetAllShowtime();
        public Task<ShowtimeDetailsDTO> GetShowtimeDetailsById(int id);
        public Task<IEnumerable<ShowtimeGroupDTO>> GetShowtimesForAMovie(string movieName);
        public Task<IEnumerable<ShowtimeGroupDTO>> GetShowtimesForATheatre(string theatreName);
        public Task<IEnumerable<SeatDTO>> GetSeatsByShowtime(int showtimeId);

        public Task<bool> DeleteShowtime(int showtimeId);
    }
}
