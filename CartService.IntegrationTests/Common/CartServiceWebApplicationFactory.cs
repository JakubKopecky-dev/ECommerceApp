using System;
using System.Linq;
using CartService.Application.Interfaces.External;
using CartService.Persistence;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using CartService.Application.DTOs.External;

namespace CartService.IntegrationTests.Common
{
    public class CartServiceWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        public CartServiceWebApplicationFactory()
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
                    .Where(d => d.ServiceType == typeof(DbContextOptions<CartDbContext>)
                             || d.ServiceType == typeof(CartDbContext)
                             || d.ServiceType.FullName!.Contains("IDbContextOptions"))
                    .ToList();

                foreach (var d in descriptors)
                {
                    services.Remove(d);
                }

                // Register SQLite in-memory DbContext
                services.AddDbContext<CartDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                // Ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
                db.Database.EnsureCreated();

                // Mock IProductReadClient – necháme jako defaultní (happy path)
                services.AddScoped(_ =>
                {
                    var productClientMock = new Mock<IProductReadClient>();
                    productClientMock
                        .Setup(c => c.CheckProductAvailabilityAsync(
                            It.IsAny<List<ProductQuantityCheckRequestDto>>(),
                            It.IsAny<CancellationToken>()))
                        .ReturnsAsync([]);
                    return productClientMock;
                });
                services.AddScoped<IProductReadClient>(sp => sp.GetRequiredService<Mock<IProductReadClient>>().Object);

                // Add MassTransit Test Harness
                services.AddMassTransitTestHarness();

                // Add test authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test", options => { });
            });
        }


    }
}
