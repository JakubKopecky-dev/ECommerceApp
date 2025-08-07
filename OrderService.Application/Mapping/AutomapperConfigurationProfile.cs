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
            CreateMap<ExternalCreateOrderDto,Order>();
            CreateMap<CreateOrderDto, Order>();
            CreateMap<Order,OrderDto>();
            CreateMap<Order, OrderExtendedDto>();


            CreateMap<ExternalCreateOrderItemDto, OrderItem>();
            CreateMap<CreateOrderItemDto, OrderItem>();
            CreateMap<OrderItem,OrderItemDto>();
            CreateMap<OrderItem,OrderItemForExtendedDto>();

        
        
        
        
        
        
        
        
        
        
        }
    }
}
