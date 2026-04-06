using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderService.Application.Interfaces.External;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.IntegrationTests.Common;
using OrderService.Persistence;
using Shared.Contracts.Events;

namespace OrderService.IntegrationTests
{
    public class ChangeOrderStatusIntegrationTests(OrderServiceWebApplicationFactory factory) : IClassFixture<OrderServiceWebApplicationFactory>
    {
        private readonly OrderServiceWebApplicationFactory _factory = factory;

        private HttpClient CreateAdminClient()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            return client;
        }

        private static HttpRequestMessage PatchJson(string url, object payload) =>
            new(HttpMethod.Patch, url) { Content = JsonContent.Create(payload) };

        private static async Task<Guid> SeedOrderAsync(OrderDbContext db, OrderStatus status)
        {
            Order order = OrderTestHelper.CreateOrder(Guid.NewGuid(), status);
            OrderItem item = OrderTestHelper.CreateOrderItem(order.Id, Guid.NewGuid(), "iPhone 16", 1200m, 1);
            OrderTestHelper.AddItems(order, [item]);

            db.Orders.Add(order);
            await db.SaveChangesAsync();
            return order.Id;
        }


        [Fact]
        [Trait("Category", "Integration")]
        public async Task ChangeStatus_ReturnsOk_AndPublishesEvent_WhenValidTransition_Created_To_Paid()
        {
            Guid orderId;
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            orderId = await SeedOrderAsync(db, OrderStatus.Created);

            var client = CreateAdminClient();
            var payload = new { status = (int)OrderStatus.Paid };

            var response = await client.SendAsync(PatchJson($"api/Order/{orderId}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var scopeVerify = _factory.Services.CreateScope();
            var dbv = scopeVerify.ServiceProvider.GetRequiredService<OrderDbContext>();
            var updated = await dbv.Orders.SingleAsync(o => o.Id == orderId);
            ((int)updated.Status).Should().Be((int)OrderStatus.Paid);

            var harness = scopeVerify.ServiceProvider.GetRequiredService<ITestHarness>();
            (await harness.Published.Any<OrderStatusChangedEvent>()).Should().BeTrue();
        }


        [Fact]
        [Trait("Category", "Integration")]
        public async Task ChangeStatus_ReturnsNotFound_WhenOrderMissing()
        {
            var client = CreateAdminClient();
            var payload = new { status = (int)OrderStatus.Paid };

            var response = await client.SendAsync(PatchJson($"api/Order/{Guid.NewGuid()}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        [Trait("Category", "Integration")]
        public async Task ChangeStatus_ReturnsNotFound_WhenInvalidTransition_Completed_To_Accepted()
        {
            Guid orderId;
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            orderId = await SeedOrderAsync(db, OrderStatus.Completed);

            var client = CreateAdminClient();
            var payload = new { status = (int)OrderStatus.Accepted };

            var response = await client.SendAsync(PatchJson($"api/Order/{orderId}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        [Trait("Category", "Integration")]
        public async Task ChangeStatus_ReturnsNotFound_WhenTargetCompleted_ButDeliveryNotFound()
        {
            Guid orderId;
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            orderId = await SeedOrderAsync(db, OrderStatus.Shipped);

            var deliveryMock = scope.ServiceProvider.GetRequiredService<Mock<IDeliveryReadClient>>();
            deliveryMock
                .Setup(c => c.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Shared.Contracts.Enums.DeliveryStatus?)null);

            var client = CreateAdminClient();
            var payload = new { status = (int)OrderStatus.Completed };

            var response = await client.SendAsync(PatchJson($"api/Order/{orderId}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        [Trait("Category", "Integration")]
        public async Task ChangeStatus_ReturnsNotFound_WhenTargetCompleted_ButDeliveryNotDelivered()
        {
            Guid orderId;
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            orderId = await SeedOrderAsync(db, OrderStatus.Shipped);

            var deliveryMock = scope.ServiceProvider.GetRequiredService<Mock<IDeliveryReadClient>>();
            deliveryMock
                .Setup(c => c.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Shared.Contracts.Enums.DeliveryStatus.InProgress);

            var client = CreateAdminClient();
            var payload = new { status = (int)OrderStatus.Completed };

            var response = await client.SendAsync(PatchJson($"api/Order/{orderId}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        [Trait("Category", "Integration")]
        public async Task ChangeStatus_ReturnsOk_AndPublishesEvent_WhenTargetCompleted_AndDeliveryDelivered()
        {
            Guid orderId;
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            orderId = await SeedOrderAsync(db, OrderStatus.Shipped);

            var deliveryMock = scope.ServiceProvider.GetRequiredService<Mock<IDeliveryReadClient>>();
            deliveryMock
                .Setup(c => c.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Shared.Contracts.Enums.DeliveryStatus.Delivered);

            var client = CreateAdminClient();
            var payload = new { status = (int)OrderStatus.Completed };

            var response = await client.SendAsync(PatchJson($"api/Order/{orderId}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var scopeVerify = _factory.Services.CreateScope();
            var dbv = scopeVerify.ServiceProvider.GetRequiredService<OrderDbContext>();
            var updated = await dbv.Orders.SingleAsync(o => o.Id == orderId);
            ((int)updated.Status).Should().Be((int)OrderStatus.Completed);

            var harness = scopeVerify.ServiceProvider.GetRequiredService<ITestHarness>();
            (await harness.Published.Any<OrderStatusChangedEvent>()).Should().BeTrue();
        }
    }
}