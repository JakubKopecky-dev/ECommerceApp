using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entity;

namespace OrderService.Application.Services
{
    public class OrderItemService(IOrderItemRepository orderItemRepository, IMapper mapper, ILogger<OrderItemService> logger) : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<OrderItemService> _logger = logger;



        public async Task<IReadOnlyList<OrderItemDto>> GetAllOrderItemsAsync()
        {
            _logger.LogInformation("Retrieving all orderItems.");

            IReadOnlyList<OrderItem> orderItems = await _orderItemRepository.GetAllAsync();
            _logger.LogInformation("Retrieved all orderItems. Count: {Couunt}.", orderItems.Count);

            return _mapper.Map<IReadOnlyList<OrderItemDto>>(orderItems);
        }



        public async Task<OrderItemDto?> GetOrderItemAsync(Guid orderItemId)
        {
            _logger.LogInformation("Retrieving orderItem. OrderItemId: {OrderItemId}.", orderItemId);

            OrderItem? orderItem = await _orderItemRepository.FindByIdAsync(orderItemId);
            if (orderItem is null)
            {
                _logger.LogWarning("OrderItem not found. OrderItemId: {OrderItemId}.", orderItemId);
                return null;
            }

            _logger.LogInformation("OrderItem found. OrderItemId: {OrderItemId}.", orderItemId);
            return _mapper.Map<OrderItemDto>(orderItem);
        }



        public async Task<OrderItemDto> CreateOrderItemAsync(CreateOrderItemDto createDto)
        {
            _logger.LogInformation("Creating orderItem. ProductId: {ProductId}, OrderId: {OrderId}.", createDto.ProductId, createDto.OrderId);

            OrderItem orderItem = _mapper.Map<OrderItem>(createDto);
            orderItem.Id = default;
            orderItem.CreatedAt = DateTime.UtcNow;

            OrderItem createdOrderItem = await _orderItemRepository.InsertAsync(orderItem);
            _logger.LogInformation("OrderItem created. OrderItemId: {OrderItemId}.",createdOrderItem.Id);

            return _mapper.Map<OrderItemDto>(createdOrderItem);
        }



        public async Task<OrderItemDto?> ChangeOrderItemQuantityAsync(Guid orderItemId, ChangeOrderItemQuantityDto changeDto)
        {
            _logger.LogInformation("Changing orderItem quantity. OrderItemId: {OrderItemId}.", orderItemId);

            OrderItem? orderItem = await _orderItemRepository.FindByIdAsync(orderItemId);
            if (orderItem is null)
            {
                _logger.LogWarning("Cannot change orderItem quantity. OrderItem not found. OrderItemId: {OrderItemId}.", orderItemId);
                return null;
            }

            orderItem.Quantity = changeDto.Quantity;
            orderItem.UpdatedAt = DateTime.UtcNow;

            OrderItem updatedOrderItem = await _orderItemRepository.UpdateAsync(orderItem);
            _logger.LogInformation("OrderItem quaintity changed. OrderItemId: {OrderItemId}.", orderItemId);

            return _mapper.Map<OrderItemDto>(updatedOrderItem);
        }



        public async Task<OrderItemDto?> DeleteOrderItemAsync(Guid orderItemId)
        {
            _logger.LogInformation("Deleting orderItem. OrderItemId: {OrderItemId}.", orderItemId);

            OrderItem? orderItem = await _orderItemRepository.FindByIdAsync(orderItemId);
            if (orderItem is null)
            {
                _logger.LogWarning("Cannot delete. OrderItem not found. OrderItemId: {OrderItemId}.", orderItemId);
                return null;
            }

            OrderItemDto deletedOrderItem = _mapper.Map<OrderItemDto>(orderItem);

            await _orderItemRepository.DeleteAsync(orderItemId);
            _logger.LogInformation("OrderItem deleted. OrderItemId: {OrderItemId}.", orderItemId);

            return deletedOrderItem;
        }



    }
}
