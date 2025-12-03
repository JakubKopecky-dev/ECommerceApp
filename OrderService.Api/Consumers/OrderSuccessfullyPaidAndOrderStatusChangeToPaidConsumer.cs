using MassTransit;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces.Services;
using Shared.Contracts.Events;
using OrderService.Domain.Enums;

namespace OrderService.Api.Consumers
{
    public class OrderSuccessfullyPaidAndOrderStatusChangeToPaidConsumer(IOrderService orderService, ILogger<OrderSuccessfullyPaidAndOrderStatusChangeToPaidConsumer> logger) : IConsumer<OrderSuccessfullyPaidAndOrderStatusChangeToPaidEvent>
    {
        private readonly IOrderService _orderService  = orderService;
        private readonly ILogger<OrderSuccessfullyPaidAndOrderStatusChangeToPaidConsumer> _logger = logger;



        public async Task Consume(ConsumeContext<OrderSuccessfullyPaidAndOrderStatusChangeToPaidEvent> context)
        {
            var ct = context.CancellationToken;

            OrderSuccessfullyPaidAndOrderStatusChangeToPaidEvent message = context.Message;
            _logger.LogInformation("Recieved  OrderSuccessfullyPaidAndOrderStatusChangeToPaidEvent. OrderId: {OrderId}.", message.OrderId);

            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Paid };
            await _orderService.ChangeOrderStatusAsync(message.OrderId, changeDto, ct);
        }



    }
}
