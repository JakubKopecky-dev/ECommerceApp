using MassTransit;
using NotificationService.Api.Consumers;

namespace NotificationService.Api.DependencyInjection
{
    public static class MassTransitServiceCollectionExtesion
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                // register consumers
                x.AddConsumer<DeliveryCanceledConsumer>();
                x.AddConsumer<OrderCreatedConsumer>();
                x.AddConsumer<OrderStatusChangedConsumer>();



                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");

                    });

                    cfg.ConfigureEndpoints(context);

                });
            });



            return services;
        }
    }
}
