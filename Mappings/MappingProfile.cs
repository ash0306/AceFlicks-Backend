using AutoMapper;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Bookings;
using MovieBookingBackend.Models.DTOs.Movies;
using MovieBookingBackend.Models.DTOs.Seats;
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
            CreateMap<Showtime, ShowtimeDetailsDTO>().ReverseMap();
            #endregion

            #region Seat Mappings
            CreateMap<Seat, SeatDTO>().ReverseMap();
            CreateMap<Seat, AddSeatDTO>().ReverseMap();
            CreateMap<Seat, UpdateSeatDTO>().ReverseMap();
            CreateMap<Seat, UpdateSeatStatusDTO>().ReverseMap();
            #endregion

            #region Booking Mappings
            CreateMap<Booking, BookingDTO>().ReverseMap();
            CreateMap<Booking, AddBookingDTO>().ReverseMap();
            CreateMap<Booking, BookingStatusDTO>().ReverseMap();
            #endregion
        }
    }
}
