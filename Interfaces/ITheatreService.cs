using MovieBookingBackend.Models.DTOs.Showtimes;
using MovieBookingBackend.Models.DTOs.Theatres;

namespace MovieBookingBackend.Interfaces
{
    public interface ITheatreService
    {
        public Task<TheatreDTO> AddTheatre(TheatreDTO theatreDTO);
        public Task<IEnumerable<TheatreDTO>> GetAllTheatres();
        public Task<IEnumerable<string>> GetAllTheatreNames();
        public Task<TheatreDTO> GetTheatreById(int id);
        public Task<IEnumerable<string>> GetTheatreLocationsByName(string theatreName);
        public Task<TheatreDTO> UpdateTheatre(UpdateTheatreDTO updateTheatreDTO);
    }
}
