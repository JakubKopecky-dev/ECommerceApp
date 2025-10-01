using CartService.Api.Grpc.GrpcClients;
using CartService.Application.Interfaces.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Grpc;
using System.Net.Security;

namespace CartService.Api.DependencyInjection
{
    public static class GrpcServiceCollectionExtension
    {
        public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<ProductService.Grpc.ProductService.ProductServiceClient>(o =>
            {
                o.Address = new Uri(configuration["ProductService:GrpcAddress"]!);
            })
            .ConfigureChannel(o =>
            {
                o.HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    SslOptions = new SslClientAuthenticationOptions
                    {
                        ApplicationProtocols = [SslApplicationProtocol.Http2]
                    }
                };
            });

            services.AddGrpcClient<OrderService.Grpc.OrderService.OrderServiceClient>(o =>
            {
                o.Address = new Uri(configuration["OrderService:GrpcAddress"]!);
            })
            .ConfigureChannel(o =>
            {
                o.HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    SslOptions = new SslClientAuthenticationOptions
                    {
                        ApplicationProtocols = [SslApplicationProtocol.Http2]
                    }
                };
            });


            services.AddScoped<IProductReadClient, GrpcProductReadClient>();
            services.AddScoped<IOrderReadClient, GrpcOrderReadClient>();


            return services;
        }
    }
}
