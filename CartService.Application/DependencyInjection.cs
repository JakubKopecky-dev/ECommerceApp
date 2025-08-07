using CartService.Application.Interfaces.Services;
using CartService.Application.Mapping;
using CartService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using CartServiceService = CartService.Application.Services.CartService;


namespace CartService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<ICartItemService, CartItemService>();
            services.AddScoped<ICartService, CartServiceService>();


            // Register AutoMapper
            services.AddAutoMapper(cfg => { }, typeof(AutomapperConfigurationProfile));

            return services;
        }
    }
}
