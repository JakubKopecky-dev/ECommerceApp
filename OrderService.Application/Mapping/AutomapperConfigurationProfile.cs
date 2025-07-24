using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            CreateMap<CreateOrderDto, Order>();
            CreateMap<Order,OrderDto>();


            CreateMap<CreateOrderItemDto, OrderItem>();
            CreateMap<OrderItem,OrderItemDto>();
        
        
        
        
        
        
        
        
        
        
        }
    }
}
