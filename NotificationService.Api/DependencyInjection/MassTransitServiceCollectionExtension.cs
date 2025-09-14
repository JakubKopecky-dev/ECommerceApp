using MassTransit;
using NotificationService.Api.Consumers;

namespace NotificationService.Api.DependencyInjection
{
    public static class MassTransitServiceCollectionExtension
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services, IConfiguration configuration)
        {
            var host = configuration["RabbitMq:Host"];
            var virtualHost = configuration["RabbitMq:VirtualHost"];
            var userName = configuration["RabbitMq:Username"]!;
            var password = configuration["RabbitMq:Password"]!;

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                // register consumers
                x.AddConsumer<DeliveryCanceledConsumer>();
                x.AddConsumer<OrderCreatedConsumer>();
                x.AddConsumer<OrderStatusChangedConsumer>();



                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, virtualHost, h =>
                    {
                        h.Username(userName);
                        h.Password(password);

                    });

                    cfg.ConfigureEndpoints(context);

                });
            });



            return services;
        }
    }
}
