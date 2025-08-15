using MassTransit;
using Microsoft.AspNetCore.SignalR;
using OrderService.Application.Interfaces.Services;
using Shared.Contracts.Events;

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

            await _orderService.SetOrderStatusCompletedFromDelivery(message.OrderId,ct);
        }



    }
}
