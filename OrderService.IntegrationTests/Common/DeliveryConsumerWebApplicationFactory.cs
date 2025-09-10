using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderService.Application.Interfaces.Services;
using OrderService.Persistence;
using OrderService.Api.Consumers;

namespace OrderService.IntegrationTests.Common
{
    public class DeliveryConsumerWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        public DeliveryConsumerWebApplicationFactory()
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
                    services.Remove(d);

                // Register SQLite in-memory DbContext
                services.AddDbContext<OrderDbContext>(options => options.UseSqlite(_connection));

                // Ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                db.Database.EnsureCreated();

                // Mock IOrderService
                var orderServiceMock = new Mock<IOrderService>();
                services.AddSingleton(orderServiceMock);
                services.AddSingleton(sp => orderServiceMock.Object);

                // MassTransit Test Harness
                services.AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddConsumer<DeliveryDeliveredConsumer>();
                });
            });
        }




    }
}
