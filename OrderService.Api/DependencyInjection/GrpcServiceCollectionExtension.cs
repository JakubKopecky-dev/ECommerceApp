using OrderService.Api.Grpc.GrpcClients;
using OrderService.Application.Interfaces.External;
using System.Net.Security;

namespace OrderService.Api.DependencyInjection
{
    public static class GrpcServiceCollectionExtension
    {
        public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<DeliveryService.Grpc.DeliveryService.DeliveryServiceClient>(o =>
            {
                o.Address = new Uri(configuration["DeliveryService:GrpcAddress"]!);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true
                };
            });

            services.AddGrpcClient<PaymentService.Grpc.PaymentService.PaymentServiceClient>(o =>
            {
                o.Address = new Uri(configuration["PaymentService:GrpcAddress"]!);         
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true
                };
            });

            services.AddScoped<IDeliveryReadClient, GrpcDeliveryReadClient>();
            services.AddScoped<IPaymentReadClient, GrpcPaymentReadClient>();


            return services;
        }
    }
}
