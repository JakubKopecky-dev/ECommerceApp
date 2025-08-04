using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Application.Mapping;
using DeliveryService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using DeliveryServiceService = DeliveryService.Application.Services.DeliveryService;

namespace DeliveryService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Service services for Di
            services.AddScoped<IDeliveryService, DeliveryServiceService>();
            services.AddScoped<ICourierService, CourierService>();


            // Register AutoMapper
            services.AddAutoMapper(config => { }, typeof(AutomapperConfigurationProfile));

            return services;
        }

    }
}
