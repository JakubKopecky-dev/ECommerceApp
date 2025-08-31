using System;
using System.Linq;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderService.Application.Interfaces.External;
using OrderService.Persistence;

namespace OrderService.IntegrationTests.Common
{
    public class OrderServiceWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        public OrderServiceWebApplicationFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registrations
                var descriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<OrderDbContext>)
                             || d.ServiceType == typeof(OrderDbContext)
                             || d.ServiceType.FullName!.Contains("IDbContextOptions"))
                    .ToList();

                foreach (var d in descriptors)
                {
                    services.Remove(d);
                }

                // Register SQLite in-memory DbContext
                services.AddDbContext<OrderDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                // Ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                db.Database.EnsureCreated();

                // Mock IDeliveryReadClient
                Mock<IDeliveryReadClient> deliveryClientMock = new();
                // By default → return Delivered for simplicity
                deliveryClientMock
                    .Setup(c => c.GetDeliveryStatusByOrderIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Shared.Contracts.Enums.DeliveryStatus.Delivered);

                // Register both Mock and Object
                services.AddSingleton(deliveryClientMock);
                services.AddSingleton(sp => deliveryClientMock.Object);

                // Add MassTransit Test Harness
                services.AddMassTransitTestHarness();

                // Add test authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });
        }
    }
}
