using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entities;

namespace OrderService.Application.Services
{
    public class OrderItemService(IOrderItemRepository orderItemRepository, ILogger<OrderItemService> logger) : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
        private readonly ILogger<OrderItemService> _logger = logger;



        public async Task<IReadOnlyList<OrderItemDto>> GetAllOrderItemsByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all orderItems by orderId. OrderId: {OrderId}.",orderId);

            IReadOnlyList<OrderItem> orderItems = await _orderItemRepository.GetAllOrderItemsByOrderId(orderId,ct);
            _logger.LogInformation("Retrieved all orderItems. Count: {Count}, OrderId: {OrderId}.", orderItems.Count,orderId);

            return [..orderItems.Select(x => x.OrderItemToOrderItemDto())];
        }



        public async Task<OrderItemDto?> GetOrderItemAsync(Guid orderItemId,CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving orderItem. OrderItemId: {OrderItemId}.", orderItemId);

            OrderItem? orderItem = await _orderItemRepository.FindByIdAsync(orderItemId,ct);
            if (orderItem is null)
            {
                _logger.LogWarning("OrderItem not found. OrderItemId: {OrderItemId}.", orderItemId);
                return null;
            }

            _logger.LogInformation("OrderItem found. OrderItemId: {OrderItemId}.", orderItemId);
            return orderItem.OrderItemToOrderItemDto();
        }



        public async Task<OrderItemDto> CreateOrderItemAsync(CreateOrderItemDto createDto,CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new orderItem. ProductId: {ProductId}, OrderId: {OrderId}.", createDto.ProductId, createDto.OrderId);

            OrderItem orderItem = OrderItem.Create(createDto.ProductId, createDto.ProductName, createDto.UnitPrice, createDto.Quantity, createDto.OrderId);

             await _orderItemRepository.AddAsync(orderItem, ct);
            await _orderItemRepository.SaveChangesAsync(ct);
            _logger.LogInformation("OrderItem created. OrderItemId: {OrderItemId}.",orderItem.Id);

            return orderItem.OrderItemToOrderItemDto();
        }



        public async Task<OrderItemDto?> ChangeOrderItemQuantityAsync(Guid orderItemId, ChangeOrderItemQuantityDto changeDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Changing orderItem quantity. OrderItemId: {OrderItemId}.", orderItemId);

            OrderItem? orderItem = await _orderItemRepository.FindByIdAsync(orderItemId, ct);
            if (orderItem is null)
            {
                _logger.LogWarning("Cannot change orderItem quantity. OrderItem not found. OrderItemId: {OrderItemId}.", orderItemId);
                return null;
            }

            orderItem.ChangeQuantity(changeDto.Quantity);

            await _orderItemRepository.SaveChangesAsync(ct);
            _logger.LogInformation("OrderItem quantity changed. OrderItemId: {OrderItemId}.", orderItemId);

            return orderItem.OrderItemToOrderItemDto();
        }



        public async Task<bool> DeleteOrderItemAsync(Guid orderItemId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting orderItem. OrderItemId: {OrderItemId}.", orderItemId);

            OrderItem? orderItem = await _orderItemRepository.FindByIdAsync(orderItemId,ct);
            if (orderItem is null)
            {
                _logger.LogWarning("Cannot delete. OrderItem not found. OrderItemId: {OrderItemId}.", orderItemId);
                return false;
            }

            _orderItemRepository.Remove(orderItem);
            await _orderItemRepository.SaveChangesAsync(ct);
            _logger.LogInformation("OrderItem deleted. OrderItemId: {OrderItemId}.", orderItemId);

            return true;
        }



    }
}
