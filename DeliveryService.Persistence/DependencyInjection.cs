using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryService.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<DeliveryDbContext>(opt =>
            opt.UseSqlServer(connectionString));


            // Register repository services for DI
            services.AddScoped<ICourierRepository, CourierRepository>();
            services.AddScoped<IDeliveryRepository, DeliveryRepository>();


            return services;
        }



    }
}
