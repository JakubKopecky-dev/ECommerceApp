using MassTransit;
using ProductService.Api.Consumers;

namespace ProductService.Api.DependencyInjection
{
    public static class MassTransitServiceCollectionExtension
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services, IConfiguration configuration)
        {
            var transport = configuration["MessageBroker:Transport"]; // RabbitMq nebo AzureServiceBus

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                // register consumers
                x.AddConsumer<OrderItemReservedConsumer>();

                if (transport == "RabbitMq")
                {
                    var host = configuration["RabbitMq:Host"];
                    var virtualHost = configuration["RabbitMq:VirtualHost"];
                    var userName = configuration["RabbitMq:Username"]!;
                    var password = configuration["RabbitMq:Password"]!;

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(host, virtualHost, h =>
                        {
                            h.Username(userName);
                            h.Password(password);
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                }
                else if (transport == "AzureServiceBus")
                {
                    var connectionString = configuration["AzureServiceBus:ConnectionString"];

                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(connectionString);

                        cfg.ConfigureEndpoints(context);
                    });
                }
            });

            return services;
        }
    }
}
