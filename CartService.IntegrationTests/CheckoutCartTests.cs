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

        private HttpClient CreateAuthenticatedClient(Guid userId)
        {
            TestAuthHandler.FixedUserId = userId;
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            return client;
        }

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

            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            db.Carts.Add(new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Items = [new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 999 }]
            });
            db.SaveChanges();


            var client = CreateAuthenticatedClient(userId);

            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<CheckoutResult>();
            result!.Success.Should().BeTrue();
            result.BadProducts.Should().BeEmpty();

            using var scope2 = _factory.Services.CreateScope();
            var db2 = scope2.ServiceProvider.GetRequiredService<CartDbContext>();
            var deletedCart = await db2.Carts.SingleOrDefaultAsync(c => c.UserId == userId);
            deletedCart.Should().BeNull();
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsCartNotFound_WhenCartMissing()
        {
            Guid userId = Guid.NewGuid();
            var client = CreateAuthenticatedClient(userId);

            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsCartNotFound_WhenCartEmpty()
        {
            Guid userId = Guid.NewGuid();

            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            db.Carts.Add(new Cart { Id = Guid.NewGuid(), UserId = userId, Items = [] });
            db.SaveChanges();


            var client = CreateAuthenticatedClient(userId);

            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsOkFalse_WhenProductsUnavailable()
        {
            Guid userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            db.Carts.Add(new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Items = [new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 999 }]
            });
            db.SaveChanges();

            var productClient = scope.ServiceProvider.GetRequiredService<Mock<IProductReadClient>>();
            productClient
                .Setup(c => c.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([new(){ Id = productId }]);


            var client = CreateAuthenticatedClient(userId);

            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            json.Should().NotBeNull();

            var root = json!.RootElement;

            root.TryGetProperty("message", out var message).Should().BeTrue();
            message.GetString().Should().Be("Some products are out of stock.");

            root.TryGetProperty("products", out var products).Should().BeTrue();
            products.ValueKind.Should().Be(JsonValueKind.Array);
            products.EnumerateArray().Should().NotBeEmpty();
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckoutCart_ReturnsDeliveryNotCreated_WhenOrderDeliveryIdNull()
        {
            Guid userId = Guid.NewGuid();

            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            db.Carts.Add(new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Items = [new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 999 }]
            });
            db.SaveChanges();

            var orderClient = scope.ServiceProvider.GetRequiredService<Mock<IOrderReadClient>>();
            orderClient
                .Setup(c => c.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderFromCartResponseDto { OrderId = Guid.NewGuid(), DeliveryId = null });


            var client = CreateAuthenticatedClient(userId);

            var response = await client.PostAsJsonAsync("api/Cart/checkout", CreateValidRequest());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }



    }
}
