using OrderService.Api.Grpc.GrpcClients;
using OrderService.Application.Interfaces.External;

namespace OrderService.Api.DependencyInjection
{
    public static class GrpcServiceCollectionExtension
    {
        public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<DeliveryService.Grpc.DeliveryService.DeliveryServiceClient>(o =>
            {
                o.Address = new Uri(configuration["DeliveryService:GrpcAddress"]!);
            });

            services.AddScoped<IDeliveryReadClient, GrpcDeliveryReadClient>();


            return services;
        }
    }
}
