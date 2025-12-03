using AutoMapper;
using DeliveryService.Application.DTOs.Courier;
using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Domain.Entities;

namespace DeliveryService.Application.Mapping
{
    public class AutomapperConfigurationProfile : Profile
    {
        public AutomapperConfigurationProfile()
        {
            CreateMap<CreateUpdateCourierDto, Courier>();
            CreateMap<Courier,CourierDto>();


            CreateMap<CreateUpdateDeliveryDto, Delivery>();
            CreateMap<Delivery,DeliveryExtendedDto>();
            CreateMap<Delivery,DeliveryDto>();

        }
    }
}
