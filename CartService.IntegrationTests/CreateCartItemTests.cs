using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CartService.Application.DTOs.CartItem;
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
    public class CreateCartItemTests(CartServiceWebApplicationFactory factory) : IClassFixture<CartServiceWebApplicationFactory>
    {
        private readonly CartServiceWebApplicationFactory _factory = factory;

        private HttpClient CreateAuthenticatedClient(Guid userId)
        {
            TestAuthHandler.FixedUserId = userId;
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            return client;
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CreateCartItem_ReturnsCreatedAndPersists()
        {
            Guid userId = Guid.NewGuid();
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            ProductDto expectedProduct = new() { Title = "iPhone 16", Price = 1299, QuantityInStock = 10 };

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            db.Carts.Add(new Cart { Id = cartId, UserId = userId, Items = [] });
            db.SaveChanges();

            var productClient = scope.ServiceProvider.GetRequiredService<Mock<IProductReadClient>>();
            productClient
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProduct);

            var client = CreateAuthenticatedClient(userId);

            CreateCartItemDto request = new() { CartId = cartId, ProductId = productId, Quantity = 2 };
            var response = await client.PostAsJsonAsync("api/CartItem", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<CartItemDto>();
            created.Should().NotBeNull();
            created!.ProductName.Should().Be(expectedProduct.Title);
            created.Quantity.Should().Be(request.Quantity);

            using var verifyScope = _factory.Services.CreateScope();
            var dbVerify = verifyScope.ServiceProvider.GetRequiredService<CartDbContext>();
            var item = await dbVerify.CartItems.SingleOrDefaultAsync(i => i.Id == created.Id);
            item.Should().NotBeNull();
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CreateCartItem_ReturnsNotFound_WhenProductMissing()
        {
            Guid userId = Guid.NewGuid();
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            db.Carts.Add(new Cart { Id = cartId, UserId = userId, Items = [] });
            db.SaveChanges();

            var productClient = scope.ServiceProvider.GetRequiredService<Mock<IProductReadClient>>();
            productClient
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            var client = CreateAuthenticatedClient(userId);
            CreateCartItemDto request = new() { CartId = cartId, ProductId = productId, Quantity = 2 };

            var response = await client.PostAsJsonAsync("api/CartItem", request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            using var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            string? message = json!.RootElement.GetProperty("message").GetString();
            message.Should().Be("Product not found.");
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CreateCartItem_ReturnsBadRequest_WhenOutOfStock()
        {
            Guid userId = Guid.NewGuid();
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            ProductDto lowStockProduct = new() { Title = "iPhone 11", Price = 1000, QuantityInStock = 1 };

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            db.Carts.Add(new Cart { Id = cartId, UserId = userId, Items = [] });
            db.SaveChanges();

            var productClient = scope.ServiceProvider.GetRequiredService<Mock<IProductReadClient>>();
            productClient
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(lowStockProduct);

            var client = CreateAuthenticatedClient(userId);
            CreateCartItemDto request = new() { CartId = cartId, ProductId = productId, Quantity = 5 };

            var response = await client.PostAsJsonAsync("api/CartItem", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            using var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            string? message = json!.RootElement.GetProperty("message").GetString();
            message.Should().Be("Not enough stock available.");
        }



        [Fact]
        [Trait("Category", "Integration")]
        public async Task CreateCartItem_IncreasesQuantity_WhenAlreadyExists()
        {
            Guid userId = Guid.NewGuid();
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CartItem existingItem = new() { Id = Guid.NewGuid(), CartId = cartId, ProductId = productId, Quantity = 2, UnitPrice = 1200, ProductName = "iPhone 16" };

            ProductDto existingProduct = new() { Title = "iPhone 16", Price = 1200, QuantityInStock = 10 };

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            db.Carts.Add(new Cart { Id = cartId, UserId = userId, Items = { existingItem } });
            db.SaveChanges();

            var productClient = scope.ServiceProvider.GetRequiredService<Mock<IProductReadClient>>();
            productClient
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            var client = CreateAuthenticatedClient(userId);
            CreateCartItemDto request = new() { CartId = cartId, ProductId = productId, Quantity = 3 };

            var response = await client.PostAsJsonAsync("api/CartItem", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var updated = await response.Content.ReadFromJsonAsync<CartItemDto>();
            updated.Should().NotBeNull();
            updated!.Id.Should().Be(existingItem.Id);
            updated.Quantity.Should().Be(5); // 2 + 3
        }



    }
}
