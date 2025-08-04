using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Persistence.Repository;

namespace NotificationService.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<NotificationDbContext>(opt =>
            opt.UseSqlServer(connectionString));


            services.AddScoped<INotificationRepository, NotificationRepository>();

            return services;
        }
    }
}
