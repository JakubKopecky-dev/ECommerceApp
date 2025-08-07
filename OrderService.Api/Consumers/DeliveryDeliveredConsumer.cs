using MassTransit;
using Microsoft.AspNetCore.SignalR;
using OrderService.Application.Interfaces.Services;
using Shared.Contracts.Events;

namespace OrderService.Api.Consumers
{
    public class DeliveryDeliveredConsumer(IOrderService orderService, ILogger<DeliveryDeliveredConsumer> logger)
    {
        private readonly IOrderService _orderService = orderService;
        private readonly ILogger<DeliveryDeliveredConsumer> _logger = logger;



        public async Task Consume(ConsumeContext<DeliveryDeliveredEvent> context)
        {
            DeliveryDeliveredEvent message = context.Message;
            _logger.LogInformation("Consuming DeliveryDeliveredEvent. OrderId: {OrderId}", message.OrderId);

            await _orderService.SetOrderStatusCompletedFromDelivery(message.OrderId);
        }



    }
}
