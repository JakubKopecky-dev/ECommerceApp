using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
