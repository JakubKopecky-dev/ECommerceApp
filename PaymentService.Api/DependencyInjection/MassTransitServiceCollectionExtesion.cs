using MassTransit;
using Stripe;

namespace PaymentService.Api.DependencyInjection
{
    public static class MassTransitServiceCollectionExtesion
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services, IConfiguration configuration)
        {
            var host = configuration["RabbitMq:Host"];
            var virtualHost = configuration["RabbitMq:VirtualHost"];
            var userName = configuration["RabbitMq:Username"]!;
            var password = configuration["RabbitMq:Password"]!;

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, virtualHost, h =>
                    {
                        h.Username(userName);
                        h.Password(password);

                    });

                });
            });



            return services;
        }
    }
}
