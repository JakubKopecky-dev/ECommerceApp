using AutoMapper;
using OrderService.Application.DTOs.Order;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Domain.Entity;

namespace OrderService.Application.Mapping
{
    public class AutomapperConfigurationProfile : Profile
    {
        public AutomapperConfigurationProfile()
        {
            CreateMap<ExternalCreateOrderDto, Order>();
            CreateMap<CreateOrderDto, Order>();
            CreateMap<Order, OrderDto>();
            CreateMap<Order, OrderExtendedDto>();

            CreateMap<ExternalCreateOrderItemDto, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Order, opt => opt.Ignore()) 
                .ForMember(dest => dest.OrderId, opt => opt.Ignore()); 

            CreateMap<CreateOrderItemDto, OrderItem>();
            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<OrderItem, OrderItemForExtendedDto>();











        }
    }
}
