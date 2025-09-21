using MassTransit;

namespace DeliveryService.Api.DependencyInjection
{
    public static class MassTransitServiceCollectionExtesion
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services, IConfiguration configuration)
        {
            var transport = configuration["MessageBroker:Transport"]; // "RabbitMq" or "AzureServiceBus"

            services.AddMassTransit(x =>
            {
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
                    });
                }

                else if (transport == "AzureServiceBus")
                {
                    var connectionString = configuration["AzureServiceBus:ConnectionString"];

                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(connectionString);
                    });
                }
            });

            return services;
        }
    }
}
