using MovieBookingBackend.Models.DTOs.Showtimes;

namespace MovieBookingBackend.Interfaces
{
    public interface IShowtimeService
    {
        public Task<ShowtimeDTO> AddShowtime(AddShowtimeDTO addShowtimeDTO);
        public Task<ShowtimeDTO> UpdateShowtime(UpdateShowtimeDTO updateShowtimeDTO);
        public Task<IEnumerable<ShowtimeDTO>> GetAllShowtime();
    }
}
