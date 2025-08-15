using MassTransit;
using ProductService.Api.Consumers;

namespace ProductService.Api.DependencyInjection
{
    public static class MassTransitServiceCollectionExtension
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<OrderItemReservedConsumer>();

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
