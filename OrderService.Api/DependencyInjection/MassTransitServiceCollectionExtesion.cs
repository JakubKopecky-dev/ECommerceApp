using MassTransit;
using OrderService.Api.Consumers;

namespace OrderService.Api.DependencyInjection
{
    public static class MassTransitServiceCollectionExtesion
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services, IConfiguration configuration)
        {
            var transport = configuration["MessageBroker:Transport"]; // RabbitMq or AzureServiceBus

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                // register consumers
                x.AddConsumer<DeliveryDeliveredConsumer>();
                x.AddConsumer<OrderSuccessfullyPaidAndOrderStatusChangeToPaidConsumer>();

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
                    var queueName = configuration["AzureServiceBus:QueueName"]!;

                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(connectionString);

                        cfg.ReceiveEndpoint(queueName, e =>
                        {
                            e.ConfigureConsumer<DeliveryDeliveredConsumer>(context);
                            e.ConfigureConsumer<OrderSuccessfullyPaidAndOrderStatusChangeToPaidConsumer>(context);
                        });
                    });
                }
            });

            return services;
        }
    }
}
