using AutoMapper;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entity;

namespace NotificationService.Application.Mapping
{
    public class AutomapperConfigurationProfile : Profile
    {
        AutomapperConfigurationProfile()
        {
            CreateMap<Notification, NotificationDto>();
            CreateMap<CreateNofiticationDto, Notification>();

        }
    }
}
