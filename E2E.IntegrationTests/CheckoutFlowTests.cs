using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CartService.Application.Common;
using CartService.Application.DTOs.Cart;
using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Domain.Enum;
using E2E.IntegrationTests.Common;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Persistence;
using OrderService.Application.DTOs.Order;
using OrderService.Domain.Enum;
using OrderService.Persistence;
using Shared.Contracts.Events;

namespace E2E.IntegrationTests
{
    public class CheckoutFlowTests(CompositeWebAppFactory factory) : IClassFixture<CompositeWebAppFactory>
    {
        private readonly CompositeWebAppFactory _factory = factory;
        private readonly HttpClient _client = factory.CreateClient();
        private readonly ITestHarness _harness = factory.Services.GetRequiredService<ITestHarness>();



        [Fact]
        [Trait("Category", "E2E")]
        public async Task HappyPath_CompletesSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            TestAuthHandler.FixedUserId = userId;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            await _factory.SeedCartAsync(userId);

            var checkoutDto = new CartCheckoutRequestDto
            {
                CourierId = Guid.NewGuid(),
                Email = "john.doe@gmail.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+420777123456",
                Street = "Main st",
                City = "Prague",
                PostalCode = "11000",
                State = "CZ"
            };

            // Act 1: Checkout
            var checkoutResponse = await _client.PostAsJsonAsync("/api/cart/checkout", checkoutDto);

            // Assert checkout
            checkoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var checkoutResult = await checkoutResponse.Content.ReadFromJsonAsync<CheckoutResult>();
            checkoutResult!.Success.Should().BeTrue();
            checkoutResult.CheckoutUrl.Should().Be("https://mock.stripe/checkout/123");

            // Notification: Order created
            using (var notifScope = _factory.Services.CreateScope())
            {
                var notifDb = notifScope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                notifDb.Notifications.Should().Contain(n =>
                    n.UserId == userId && n.Title.Contains("Order created"));
            }

            // Get Order + Delivery Ids
            Guid orderId;
            Guid deliveryId;
            using (var orderScope = _factory.Services.CreateScope())
            {
                var orderDb = orderScope.ServiceProvider.GetRequiredService<OrderDbContext>();
                var order = orderDb.Orders.First(o => o.UserId == userId);
                orderId = order.Id;
                deliveryId = order.DeliveryId!.Value;
            }

            // Act 2: Simulate payment (publish event)
            await _harness.Bus.Publish(new OrderSuccessfullyPaidAndOrderStatusChangeToPaidEvent { OrderId = orderId });
            await _harness.InactivityTask;

            // Assert Paid
            using (var orderScope = _factory.Services.CreateScope())
            {
                var orderDb = orderScope.ServiceProvider.GetRequiredService<OrderDbContext>();
                var order = await orderDb.Orders.FindAsync(orderId);
                order!.Status.Should().Be(OrderStatus.Paid);
            }

            // Act 3: Accepted
            var acceptedDto = new ChangeOrderStatusDto { Status = OrderStatus.Accepted };
            var acceptedResp = await _client.PatchAsJsonAsync($"/api/order/{orderId}/status", acceptedDto);
            acceptedResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act 4: Shipped
            var shippedDto = new ChangeOrderStatusDto { Status = OrderStatus.Shipped };
            var shippedResp = await _client.PatchAsJsonAsync($"/api/order/{orderId}/status", shippedDto);
            shippedResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act 5: Delivered (delivery triggers Completed)
            var deliveryDto = new ChangeDeliveryStatusDto { Status = DeliveryStatus.Delivered };
            var delResp = await _client.PatchAsJsonAsync($"/api/delivery/{deliveryId}/status", deliveryDto);
            delResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert Completed
            using (var orderScope = _factory.Services.CreateScope())
            {
                var orderDb = orderScope.ServiceProvider.GetRequiredService<OrderDbContext>();
                var order = await orderDb.Orders.FindAsync(orderId);
                order!.Status.Should().Be(OrderStatus.Completed);
            }

            // Assert final notification
            using (var notifScope = _factory.Services.CreateScope())
            {
                var notifDb = notifScope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                notifDb.Notifications.Should().Contain(n =>
                    n.UserId == userId && n.Title.Contains("Order status changed"));
            }
        }
    }
}
