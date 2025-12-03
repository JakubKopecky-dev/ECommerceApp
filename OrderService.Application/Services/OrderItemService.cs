using AutoMapper;
using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entities;
using Shared.Contracts.Events;

namespace OrderService.Application.Services
{
    public class OrderItemService(IOrderItemRepository orderItemRepository, IMapper mapper, ILogger<OrderItemService> logger) : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<OrderItemService> _logger = logger;



        public async Task<IReadOnlyList<OrderItemDto>> GetAllOrderItemsByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all orderItems by orderId. OrderId: {OrderId}.",orderId);

            IReadOnlyList<OrderItem> orderItems = await _orderItemRepository.GetAllOrderItemsByOrderId(orderId,ct);
            _logger.LogInformation("Retrieved all orderItems. Count: {Count}, OrderId: {OrderId}.", orderItems.Count,orderId);

            return _mapper.Map<List<OrderItemDto>>(orderItems);
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
            return _mapper.Map<OrderItemDto>(orderItem);
        }



        public async Task<OrderItemDto> CreateOrderItemAsync(CreateOrderItemDto createDto,CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new orderItem. ProductId: {ProductId}, OrderId: {OrderId}.", createDto.ProductId, createDto.OrderId);

            OrderItem orderItem = _mapper.Map<OrderItem>(createDto);
            orderItem.Id = Guid.Empty;
            orderItem.CreatedAt = DateTime.UtcNow;

            OrderItem createdOrderItem = await _orderItemRepository.InsertAsync(orderItem, ct);
            _logger.LogInformation("OrderItem created. OrderItemId: {OrderItemId}.",createdOrderItem.Id);

            return _mapper.Map<OrderItemDto>(createdOrderItem);
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

            orderItem.Quantity = changeDto.Quantity;
            orderItem.UpdatedAt = DateTime.UtcNow;

            await _orderItemRepository.SaveChangesAsync(ct);
            _logger.LogInformation("OrderItem quantity changed. OrderItemId: {OrderItemId}.", orderItemId);

            return _mapper.Map<OrderItemDto>(orderItem);
        }



        public async Task<OrderItemDto?> DeleteOrderItemAsync(Guid orderItemId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting orderItem. OrderItemId: {OrderItemId}.", orderItemId);

            OrderItem? orderItem = await _orderItemRepository.FindByIdAsync(orderItemId,ct);
            if (orderItem is null)
            {
                _logger.LogWarning("Cannot delete. OrderItem not found. OrderItemId: {OrderItemId}.", orderItemId);
                return null;
            }

            OrderItemDto deletedOrderItem = _mapper.Map<OrderItemDto>(orderItem);

            _orderItemRepository.Remove(orderItem);
            await _orderItemRepository.SaveChangesAsync(ct);
            _logger.LogInformation("OrderItem deleted. OrderItemId: {OrderItemId}.", orderItemId);

            return deletedOrderItem;
        }



    }
}
