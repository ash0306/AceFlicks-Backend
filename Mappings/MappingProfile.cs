using AutoMapper;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.User;

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
        }
    }
}
