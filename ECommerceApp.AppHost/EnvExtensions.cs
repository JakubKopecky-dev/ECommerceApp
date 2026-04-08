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
            .WithEnvironment("Jwt__PublicKey", "-----BEGIN PUBLIC KEY----- MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAElaSojkTx7x9ud4Q6H8RnTP22yNRs tksLUL6MXApsm7J/lC4AdCNYR422CnS3UUqpRPkxPESyULdl4Woc7bXKog== -----END PUBLIC KEY-----")
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
