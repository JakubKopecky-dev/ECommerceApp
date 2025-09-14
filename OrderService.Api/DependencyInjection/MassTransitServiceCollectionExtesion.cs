using MassTransit;
using OrderService.Api.Consumers;

namespace OrderService.Api.DependencyInjection
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
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<DeliveryDeliveredConsumer>();
                x.AddConsumer<OrderSuccessfullyPaidAndOrderStatusChangeToPaidConsumer>();

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
