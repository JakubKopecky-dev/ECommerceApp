using MassTransit;

namespace OrderService.Api.DependencyInjection
{
    public static class MassTransitServiceCollectionExtesion
    {
        public static IServiceCollection AddMassTransitService(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");

                    });

                });
            });



            return services;
        }
    }
}
