using MassTransit;
using ProductService.Application.Interfaces.Services;
using Shared.Contracts.DTOs;
using Shared.Contracts.Events;

namespace ProductService.Api.Consumers
{
    public class OrderItemReservedConsumer(IProductService productService, ILogger<OrderItemReservedConsumer> logger) : IConsumer<OrderItemsReservedEvent>
    {
        private readonly IProductService _productService = productService;
        private readonly ILogger<OrderItemReservedConsumer> _logger = logger;



        public async Task Consume(ConsumeContext<OrderItemsReservedEvent> context)
        {
            OrderItemsReservedEvent message = context.Message;

            _logger.LogInformation("Received OrderItemsReservedEvent. OrderId: {OrderId}, ProductIds: {ProductIds}.", message.OrderId, string.Join(", ", message.Items.Select(i => i.ProductId)));

            var ct = context.CancellationToken;

            List<OrderItemCreatedDto> items = [.. message.Items];

            await _productService.ProductQuantityReserved(items, ct);
        }



    }
}
