using DeliveryService.Api.Grpc.GrpcClients;
using DeliveryService.Application.Interfaces.External;
using System.Net.Security;

namespace DeliveryService.Api.DependencyInjection
{
    public static class GrpcServiceCollectionExtension
    {
        public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<OrderService.Grpc.OrderService.OrderServiceClient>(o =>
            {
                o.Address = new Uri(configuration["OrderService:GrpcAddress"]!);
            });


            services.AddScoped<IOrderReadClient, GrpcOrderReadClient>();

            return services;
        }
    }
}
