using System.Linq;
using CartService.Application.DTOs.External;
using CartService.Application.Interfaces.External;
using CartService.Persistence;
using DeliveryService.Persistence;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NotificationService.Persistence;
using OrderService.Application.DTOs.External;
using OrderService.Application.Interfaces.External;
using OrderService.Persistence;
using CartApiProgram = CartService.Api.Program;

namespace E2E.IntegrationTests.Common
{
    public class CompositeWebAppFactory : WebApplicationFactory<CartApiProgram>
    {
        private readonly SqliteConnection _cartConn = new("DataSource=:memory:");
        private readonly SqliteConnection _orderConn = new("DataSource=:memory:");
        private readonly SqliteConnection _deliveryConn = new("DataSource=:memory:");
        private readonly SqliteConnection _notifConn = new("DataSource=:memory:");

        public CompositeWebAppFactory()
        {
            _cartConn.Open();
            _orderConn.Open();
            _deliveryConn.Open();
            _notifConn.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // Nejprve vyhodíme VŠECHNY DbContextOptions registrace
                var toRemove = services
                    .Where(d => d.ServiceType.Name.Contains("DbContextOptions"))
                    .ToList();

                foreach (var d in toRemove)
                    services.Remove(d);

                // Registrujeme každý DbContext jen se Sqlite
                services.AddDbContext<CartDbContext>(opt => opt.UseSqlite(_cartConn));
                services.AddDbContext<OrderDbContext>(opt => opt.UseSqlite(_orderConn));
                services.AddDbContext<DeliveryDbContext>(opt => opt.UseSqlite(_deliveryConn));
                services.AddDbContext<NotificationDbContext>(opt => opt.UseSqlite(_notifConn));

                // Mock payment client (Stripe)
                var paymentMock = new Mock<IPaymentReadClient>();
                paymentMock
                    .Setup(c => c.CreateCheckoutSessionAsync(
                        It.IsAny<CreateCheckoutSessionRequestDto>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new CreateCheckoutSessionResponseDto
                    {
                        CheckoutUrl = "https://mock.stripe/checkout/123"
                    });
                services.AddSingleton(paymentMock.Object);

                // Mock product client → always available
                var productMock = new Mock<IProductReadClient>();
                productMock
                    .Setup(c => c.CheckProductAvailabilityAsync(
                        It.IsAny<List<ProductQuantityCheckRequestDto>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<ProductQuantityCheckResponseDto>());
                services.AddSingleton(productMock.Object);

                // ✅ Mock order client → simuluje gRPC OrderService
                var orderMock = new Mock<IOrderReadClient>();
                orderMock
                    .Setup(c => c.CreateOrderAndDeliveryAsync(
                        It.IsAny<CreateOrderAndDeliveryDto>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new CreateOrderFromCartResponseDto
                    {
                        OrderId = Guid.NewGuid(),
                        DeliveryId = Guid.NewGuid(),
                        CheckoutUrl = "https://mock.stripe/checkout/123"
                    });
                services.AddSingleton(orderMock.Object);

                // Add MassTransit Test Harness
                services.AddMassTransitTestHarness();

                // Add test authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

                // Ensure databases created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                scope.ServiceProvider.GetRequiredService<CartDbContext>().Database.EnsureCreated();
                scope.ServiceProvider.GetRequiredService<OrderDbContext>().Database.EnsureCreated();
                scope.ServiceProvider.GetRequiredService<DeliveryDbContext>().Database.EnsureCreated();
                scope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.EnsureCreated();
            });
        }

        public async Task SeedCartAsync(Guid userId)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();

            db.Carts.Add(new CartService.Domain.Entity.Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Items =
                [
                    new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 100 }
                ]
            });

            await db.SaveChangesAsync();
        }
    }
}
