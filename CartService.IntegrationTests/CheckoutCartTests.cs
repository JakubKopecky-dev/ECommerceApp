using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CartService.Application.Common;
using CartService.Application.DTOs.Cart;
using CartService.Application.DTOs.External;
using CartService.Application.Interfaces.External;
using CartService.Domain.Entity;
using CartService.IntegrationTests.Common;
using CartService.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CartService.IntegrationTests
{
    public class CheckoutCartTests(CartServiceWebApplicationFactory factory) : IClassFixture<CartServiceWebApplicationFactory>
    {
        private readonly CartServiceWebApplicationFactory _factory = factory;

        private static CartCheckoutRequestDto CreateValidRequest() =>
            new()
            {
                CourierId = Guid.NewGuid(),
                Email = "john.doe@gmail.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+420777123456",
                Street = "Vinohradska",
                City = "Prague",
                PostalCode = "12000",
                State = "CZ",
                Note = "As a gift"
            };



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsOk_AndDeletesCart()
        {
            Guid userId = Guid.NewGuid();

            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var orderClientMock = new Mock<IOrderReadClient>();
                    orderClientMock
                        .Setup(c => c.CreateOrderAndDeliveryAsync(
                            It.IsAny<CreateOrderAndDeliveryDto>(),
                            It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new CreateOrderFromCartResponseDto
                        {
                            OrderId = Guid.NewGuid(),
                            DeliveryId = Guid.NewGuid(),
                            CheckoutUrl = "www.url.cz"
                        });
                    services.AddScoped<IOrderReadClient>(_ => orderClientMock.Object);
                });
            });

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };
            cart.Items = [new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 999 }];
            db.Carts.Add(cart);
            db.SaveChanges();

            var client = factory.CreateClient();
            TestAuthHandler.FixedUserId = userId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");


            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<CheckoutResult>();
            result!.Success.Should().BeTrue();
            result.CheckoutUrl.Should().NotBeNull();
            result.BadProducts.Should().BeEmpty();

            using var scope2 = factory.Services.CreateScope();
            var db2 = scope2.ServiceProvider.GetRequiredService<CartDbContext>();
            var deletedCart = await db2.Carts.SingleOrDefaultAsync(c => c.UserId == userId);
            deletedCart.Should().BeNull();
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsCartNotFound_WhenCartMissing()
        {
            Guid userId = Guid.NewGuid();

            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var orderClientMock = new Mock<IOrderReadClient>();
                    orderClientMock
                        .Setup(c => c.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new CreateOrderFromCartResponseDto { OrderId = Guid.NewGuid(), DeliveryId = Guid.NewGuid(), CheckoutUrl = "www.url.cz" });
                    services.AddScoped<IOrderReadClient>(_ => orderClientMock.Object);
                });
            });

            var client = factory.CreateClient();
            TestAuthHandler.FixedUserId = userId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsCartNotFound_WhenCartEmpty()
        {
            Guid userId = Guid.NewGuid();

            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var orderClientMock = new Mock<IOrderReadClient>();
                    orderClientMock
                        .Setup(c => c.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new CreateOrderFromCartResponseDto { OrderId = Guid.NewGuid(), DeliveryId = Guid.NewGuid(), CheckoutUrl = "www.url.cz" });
                    services.AddScoped<IOrderReadClient>(_ => orderClientMock.Object);
                });
            });

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            db.Carts.Add(new Cart { Id = Guid.NewGuid(), UserId = userId, Items = [] });
            db.SaveChanges();

            var client = factory.CreateClient();
            TestAuthHandler.FixedUserId = userId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");


            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsOkFalse_WhenProductsUnavailable()
        {
            Guid userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Order client – happy path (všechno se vytvoří)
                    var orderClientMock = new Mock<IOrderReadClient>();
                    orderClientMock
                        .Setup(c => c.CreateOrderAndDeliveryAsync(
                            It.IsAny<CreateOrderAndDeliveryDto>(),
                            It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new CreateOrderFromCartResponseDto
                        {
                            OrderId = Guid.NewGuid(),
                            DeliveryId = Guid.NewGuid(),
                            CheckoutUrl = "www.url.cz"
                        });
                    services.AddScoped<IOrderReadClient>(_ => orderClientMock.Object);

                    // Product client – simulujeme, že 1 produkt není dostupný
                    var productClientMock = new Mock<IProductReadClient>();
                    productClientMock
                        .Setup(c => c.CheckProductAvailabilityAsync(
                            It.IsAny<List<ProductQuantityCheckRequestDto>>(),
                            It.IsAny<CancellationToken>()))
                        .ReturnsAsync([new() { Id = productId }]);
                    services.AddScoped<IProductReadClient>(_ => productClientMock.Object);
                });
            });

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };
            cart.Items = [new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 999 }];
            db.Carts.Add(cart);
            db.SaveChanges();

            var client = factory.CreateClient();
            TestAuthHandler.FixedUserId = userId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");


            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            json.Should().NotBeNull();

            var root = json!.RootElement;
            root.GetProperty("message").GetString().Should().Be("Some items in your cart are out of stock.");
            root.GetProperty("products").EnumerateArray().Should().NotBeEmpty();
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsDeliveryNotCreated_WhenOrderDeliveryIdNull()
        {
            Guid userId = Guid.NewGuid();

            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var orderClientMock = new Mock<IOrderReadClient>();
                    orderClientMock
                        .Setup(c => c.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new CreateOrderFromCartResponseDto { OrderId = Guid.NewGuid(), DeliveryId = null, CheckoutUrl = "www.url.cz" });
                    services.AddScoped<IOrderReadClient>(_ => orderClientMock.Object);
                });
            });

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };
            cart.Items = [new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 999 }];
            db.Carts.Add(cart);
            db.SaveChanges();

            var client = factory.CreateClient();
            TestAuthHandler.FixedUserId = userId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsFail_WhenDeliveryAndCheckoutNotCreated()
        {
            Guid userId = Guid.NewGuid();

            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var orderClientMock = new Mock<IOrderReadClient>();
                    orderClientMock
                        .Setup(c => c.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new CreateOrderFromCartResponseDto { OrderId = Guid.NewGuid(), DeliveryId = null, CheckoutUrl = null });
                    services.AddScoped<IOrderReadClient>(_ => orderClientMock.Object);
                });
            });

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };
            cart.Items = [new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 999}];
            db.Carts.Add(cart);
            db.SaveChanges();

            var client = factory.CreateClient();
            TestAuthHandler.FixedUserId = userId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");


            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsFail_WhenCheckoutUrlMissing()
        {
            Guid userId = Guid.NewGuid();

            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var orderClientMock = new Mock<IOrderReadClient>();
                    orderClientMock
                        .Setup(c => c.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new CreateOrderFromCartResponseDto { OrderId = Guid.NewGuid(), DeliveryId = Guid.NewGuid(), CheckoutUrl = null });
                    services.AddScoped<IOrderReadClient>(_ => orderClientMock.Object);
                });
            });

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };
            cart.Items = [new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 999 }];
            db.Carts.Add(cart);
            db.SaveChanges();

            var client = factory.CreateClient();
            TestAuthHandler.FixedUserId = userId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");


            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }



    }
}
