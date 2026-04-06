using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DeliveryService.Application.Interfaces.External;
using DeliveryService.Domain.Entities;
using DeliveryService.Domain.Enums;
using DeliveryService.Domain.ValueObjects;
using DeliveryService.IntegrationTests.Common;
using DeliveryService.Persistence;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.Contracts.Events;

namespace DeliveryService.IntegrationTests
{
    public class ChangeDeliveryStatusIntegrationTests(DeliveryServiceWebApplicationFactory factory) : IClassFixture<DeliveryServiceWebApplicationFactory>
    {
        private readonly DeliveryServiceWebApplicationFactory _factory = factory;

        private HttpClient CreateAdminClient()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            return client;
        }

        private static HttpRequestMessage PatchJson(string url, object payload) =>
            new(HttpMethod.Patch, url) { Content = JsonContent.Create(payload) };

        private static async Task<Delivery> SeedDeliveryAsync(DeliveryDbContext db, DeliveryStatus status)
        {
            Courier courier = Courier.Create($"DHL-{Guid.NewGuid()}", null, null);
            Delivery delivery = Delivery.Create(Guid.NewGuid(), courier.Id, new Email( "aa@b.com"), "Petr", "Novak", "123456789",new Address( "Nusle 50", "Prague 4", "140 00", "Czech Republic"), null);

            // Posuneme status pokud není Pending
            if (status == DeliveryStatus.InProgress)
                delivery.ChangeStatus(DeliveryStatus.InProgress);
            else if (status == DeliveryStatus.Delivered)
            {
                delivery.ChangeStatus(DeliveryStatus.InProgress);
                delivery.ChangeStatus(DeliveryStatus.Delivered);
            }
            else if (status == DeliveryStatus.Canceled)
                delivery.ChangeStatus(DeliveryStatus.Canceled);

            db.Couriers.Add(courier);
            db.Deliveries.Add(delivery);
            await db.SaveChangesAsync();
            return delivery;
        }



        [Fact]
        public async Task ChangeStatus_ReturnsOk_AndPublishesEvent_WhenPending_To_InProgress()
        {
            Guid deliveryId;
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            var delivery = await SeedDeliveryAsync(db, DeliveryStatus.Pending);
            deliveryId = delivery.Id;

            var client = CreateAdminClient();
            var payload = new { status = (int)DeliveryStatus.InProgress };

            var response = await client.SendAsync(PatchJson($"api/Delivery/{deliveryId}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var scopeVerify = _factory.Services.CreateScope();
            var dbv = scopeVerify.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            var updated = await dbv.Deliveries.SingleAsync(d => d.Id == deliveryId);

            updated.Status.Should().Be(DeliveryStatus.InProgress);
        }



        [Fact]
        public async Task ChangeStatus_ReturnsNotFound_WhenDeliveryMissing()
        {
            var client = CreateAdminClient();
            var payload = new { status = (int)DeliveryStatus.InProgress };

            var response = await client.SendAsync(PatchJson($"api/Delivery/{Guid.NewGuid()}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }



        [Fact]
        public async Task ChangeStatus_ReturnsInternalServerError_WhenInvalidTransition()
        {
            Guid deliveryId;
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            db.Couriers.RemoveRange(db.Couriers);
            db.Deliveries.RemoveRange(db.Deliveries);
            await db.SaveChangesAsync();

            var delivery = await SeedDeliveryAsync(db, DeliveryStatus.Delivered);
            deliveryId = delivery.Id;

            var client = CreateAdminClient();
            var payload = new { status = (int)DeliveryStatus.InProgress }; // invalid backwards

            var response = await client.SendAsync(PatchJson($"api/Delivery/{deliveryId}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }



        [Fact]
        public async Task ChangeStatus_PublishesDeliveredEvent_WhenDelivered()
        {
            Guid deliveryId;
            Guid orderId;
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            var delivery = await SeedDeliveryAsync(db, DeliveryStatus.InProgress);
            deliveryId = delivery.Id;
            orderId = delivery.OrderId;

            var client = CreateAdminClient();
            var payload = new { status = (int)DeliveryStatus.Delivered };

            var response = await client.SendAsync(PatchJson($"api/Delivery/{deliveryId}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var harness = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<ITestHarness>();

            (await harness.Published.Any<DeliveryDeliveredEvent>()).Should().BeTrue();
        }



        [Fact]
        public async Task ChangeStatus_PublishesCanceledEvent_WhenCanceled_AndUserExists()
        {
            Guid deliveryId;
            Guid orderId;
            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            var delivery = await SeedDeliveryAsync(db, DeliveryStatus.Pending);
            deliveryId = delivery.Id;
            orderId = delivery.OrderId;

            var orderClientMock = scope.ServiceProvider.GetRequiredService<Mock<IOrderReadClient>>();
            orderClientMock
                .Setup(c => c.GetUserIdByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            var client = CreateAdminClient();
            var payload = new { status = (int)DeliveryStatus.Canceled };

            var response = await client.SendAsync(PatchJson($"api/Delivery/{deliveryId}/status", payload));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var harness = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<ITestHarness>();
            (await harness.Published.Any<DeliveryCanceledEvent>()).Should().BeTrue();
        }


    }
}