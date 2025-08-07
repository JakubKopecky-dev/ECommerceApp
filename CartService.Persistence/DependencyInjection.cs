using CartService.Application.Interfaces.Repositories;
using CartService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Persistence
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration config)
        {

            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<CartDbContext>(options =>
            options.UseSqlServer(connectionString));


            // Register Repositories
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<ICartRepository, CartRepository>();



            return services;
        }


    }
}
