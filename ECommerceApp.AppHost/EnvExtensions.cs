using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceApp.AppHost
{
    public static class EnvExtensions
    {
        public static IResourceBuilder<ProjectResource> WithJwtEnv(
            this IResourceBuilder<ProjectResource> service)
        {
            return service
                .WithEnvironment("Jwt__Key", "a851091109b0dad8b3834341577e990f")
                .WithEnvironment("Jwt__Issuer", "ECommerceApp.AuthService")
                .WithEnvironment("Jwt__Audience", "ECommerceApp.Frontend");
        }

        public static IResourceBuilder<ProjectResource> WithRabbitEnv(
            this IResourceBuilder<ProjectResource> service)
        {
            return service
                .WithEnvironment("RabbitMq__Host", "rabbitmq")
                .WithEnvironment("RabbitMq__VirtualHost", "/")
                .WithEnvironment("RabbitMq__Username", "guest")
                .WithEnvironment("RabbitMq__Password", "guest")
                .WithEnvironment("MessageBroker__Transport", "RabbitMq");
        }
    }

}
