using MassTransit;
using OrderService.Api.Consumers;

namespace OrderService.Api.DependencyInjection
{
    public static class MassTransitServiceCollectionExtesion
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<DeliveryDeliveredConsumer>();

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
