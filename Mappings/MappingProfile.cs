using AutoMapper;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Movies;
using MovieBookingBackend.Models.DTOs.Showtimes;
using MovieBookingBackend.Models.DTOs.Theatres;
using MovieBookingBackend.Models.DTOs.Users;

namespace MovieBookingBackend.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region User mappings
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserLoginDTO>().ReverseMap();
            CreateMap<User, UserLoginReturnDTO>().ReverseMap();
            #endregion

            #region Movie Mappings
            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<Movie, UpdateMovieDTO>().ReverseMap();
            #endregion

            #region Theatre Mappings
            CreateMap<Theatre, TheatreDTO>().ReverseMap();
            #endregion

            #region Showtime Mappings
            CreateMap<Showtime, ShowtimeDTO>().ReverseMap();
            CreateMap<Showtime, UpdateShowtimeDTO>().ReverseMap();
            CreateMap<Showtime, AddShowtimeDTO>().ReverseMap();
            #endregion
        }
    }
}
