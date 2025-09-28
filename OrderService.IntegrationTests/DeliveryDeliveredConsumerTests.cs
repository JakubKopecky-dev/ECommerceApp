using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderService.Api.Consumers;
using OrderService.Application.Interfaces.Services;
using OrderService.IntegrationTests.Common;
using Shared.Contracts.Events;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Order;
using OrderService.Domain.Enum;

namespace OrderService.IntegrationTests
{
    public class DeliveryDeliveredConsumerTests(DeliveryConsumerWebApplicationFactory factory) : IClassFixture<DeliveryConsumerWebApplicationFactory>
    {
        private readonly DeliveryConsumerWebApplicationFactory _factory = factory;

        [Fact]
        public async Task Consume_CallsOrderService_SetOrderStatusCompletedFromDelivery()
        {
            Guid orderId = Guid.NewGuid();

            using var scope = _factory.Services.CreateScope();
            var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

            var orderServiceMock = scope.ServiceProvider.GetRequiredService<Mock<IOrderService>>();

            await harness.Bus.Publish(new DeliveryDeliveredEvent { OrderId = orderId });

            (await harness.Consumed.Any<DeliveryDeliveredEvent>(cancellationToken: new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token)).Should().BeTrue();

            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Completed };
            orderServiceMock.Verify(s => s.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
