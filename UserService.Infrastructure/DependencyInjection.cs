using Microsoft.Extensions.DependencyInjection;
using UserService.Infrastructure.Mapping;
using UserService.Application.Interfaces.Services;
using UserService.Infrastructure.Services;

namespace UserService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IApplicationUserService, ApplicationUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();

            services.AddAutoMapper(cfg => { }, typeof(AutomapperConfigurationProfile).Assembly);

            return services;
        }
        
    }
}
