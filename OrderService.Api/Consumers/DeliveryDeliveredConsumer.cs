using MassTransit;
using Microsoft.AspNetCore.SignalR;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces.Services;
using Shared.Contracts.Events;
using OrderService.Domain.Enum;

namespace OrderService.Api.Consumers
{
    public class DeliveryDeliveredConsumer(IOrderService orderService, ILogger<DeliveryDeliveredConsumer> logger) : IConsumer<DeliveryDeliveredEvent>
    {
        private readonly IOrderService _orderService = orderService;
        private readonly ILogger<DeliveryDeliveredConsumer> _logger = logger;



        public async Task Consume(ConsumeContext<DeliveryDeliveredEvent> context)
        {
            var ct = context.CancellationToken;

            DeliveryDeliveredEvent message = context.Message;
            _logger.LogInformation("Received DeliveryDeliveredEvent. OrderId: {OrderId}", message.OrderId);

            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Completed };
            await _orderService.ChangeOrderStatusAsync(message.OrderId,changeDto,ct);
        }



    }
}
