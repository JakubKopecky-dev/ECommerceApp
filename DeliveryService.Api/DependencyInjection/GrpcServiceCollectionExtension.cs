using DeliveryService.Api.Grpc.GrpcClients;
using DeliveryService.Application.Interfaces.External;

namespace DeliveryService.Api.DependencyInjection
{
    public static class GrpcServiceCollectionExtension
    {
        public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<OrderService.Grpc.OrderService.OrderServiceClient>(o =>
            {
                o.Address = new Uri(configuration["OrderService:GrpcAddress"]!);
            })
            .ConfigureChannel(o =>
            {
                o.HttpHandler = new SocketsHttpHandler { EnableMultipleHttp2Connections = true };
            });

            services.AddScoped<IOrderReadClient, GrpcOrderReadClient>();

            return services;
        }
    }
}
