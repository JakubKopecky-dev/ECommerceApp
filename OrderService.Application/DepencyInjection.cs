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
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IOrderService,OrderServiceService>();


            // Register AutoMapper
            services.AddAutoMapper(cfg => { }, typeof(AutomapperConfigurationProfile));


            return services;
        }
    }
}
