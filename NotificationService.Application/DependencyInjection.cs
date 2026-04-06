using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces.Services;
using NotificationServiceService = NotificationService.Application.Services.NotificationService;

namespace NotificationService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<INotificationService, NotificationServiceService>();


            return services;
        }
    }
}
