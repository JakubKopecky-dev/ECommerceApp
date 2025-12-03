using System.Net.Http.Headers;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NotificationService.Domain.Entities;
using NotificationService.IntegrationTests.Common;
using NotificationService.Persistence;
using Shared.Contracts.Events;

namespace NotificationService.IntegrationTests.Consumers
{
    public class DeliveryCanceledConsumerTests(NotificationServiceWebApplicationFactory factory) : IClassFixture<NotificationServiceWebApplicationFactory>
    {
        private readonly NotificationServiceWebApplicationFactory _factory = factory;


        [Fact]
        [Trait("Category", "Integration")]
        public async Task Consume_CreatesNotification_AndSendsSignalR()
        {
            Guid userId = Guid.NewGuid();
            Guid orderId = Guid.NewGuid();

            using var scope = _factory.Services.CreateScope();
            var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

            var clientProxyMock = scope.ServiceProvider.GetRequiredService<Mock<IClientProxy>>();

            await harness.Bus.Publish(new DeliveryCanceledEvent{ UserId = userId,OrderId = orderId });

            (await harness.Consumed.Any<DeliveryCanceledEvent>(cancellationToken: new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token)).Should().BeTrue();

            var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            var notification = db.Notifications.SingleOrDefault(n => n.UserId == userId);
            notification.Should().NotBeNull();
            notification!.Message.Should().Contain(orderId.ToString());

            clientProxyMock.Verify(c => c.SendCoreAsync("ReceiveNotification",It.Is<object[]>(args => args.Any()),It.IsAny<CancellationToken>()),Times.Once);
        }


    }
}
