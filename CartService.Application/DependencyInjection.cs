using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartService.Application.Interfaces.Services;
using CartService.Application.Mapping;
using CartService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<ICartItemService, CartItemService>();



            // Register AutoMapper
            services.AddAutoMapper(cfg => { }, typeof(AutomapperConfigurationProfile));

            return services;
        }
    }
}
