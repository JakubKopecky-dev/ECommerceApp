using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces.Services;
using OrderService.Application.Mapping;
using OrderService.Application.Services;
using OrderServiceService = OrderService.Application.Services.OrderService;

namespace OrderService.Application
{
    public static class DepencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<IOrderService, OrderServiceService>();
            services.AddScoped<IOrderItemService, OrderItemService>();



            // Register AutoMapper
            services.AddAutoMapper(cfg => { }, typeof(AutomapperConfigurationProfile));


            return services;
        }
    }
}
