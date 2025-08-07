using AutoMapper;
using UserService.Application.DTOs.User;
using UserService.Infrastructure.Identity;

namespace UserService.Infrastructure.Mapping
{
    public class AutomapperConfigurationProfile : Profile
    {
        public AutomapperConfigurationProfile()
        {

            CreateMap<CreateUserDto,ApplicationUser>();
            CreateMap<UpdateUserDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserDto>();






        }

    }
}
