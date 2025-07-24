using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entity;
using OrderService.Domain.Enum;
using static System.Net.Mime.MediaTypeNames;

namespace OrderService.Application.Services
{
    public class OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IMapper mapper, ILogger<OrderService> logger) : IOrderService
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<OrderService> _logger = logger;



        private static bool IsStatusChangeValid(OrderStatus current, OrderStatus next)
        {
            return (current, next) switch
            {
                (OrderStatus.Draft, OrderStatus.Created) => true,
                (OrderStatus.Created, OrderStatus.Paid) => true,
                (OrderStatus.Paid, OrderStatus.Accepted) => true,
                (OrderStatus.Accepted, OrderStatus.Shipped) => true,
                (OrderStatus.Shipped, OrderStatus.Completed) => true,
                (OrderStatus.Created, OrderStatus.Cancelled) => true,
                (OrderStatus.Paid, OrderStatus.Rejected) => true,
                _ => false
            };
        }




        public async Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync()
        {
            _logger.LogInformation("Retrieving all orders.");

            IReadOnlyList<Order> orders = await _orderRepository.GetAllAsync();
            _logger.LogInformation("Retrievid all orders. Count: {Count}.", orders.Count);

            return _mapper.Map<List<OrderDto>>(orders);
        }



        public async Task<OrderDto?> GetOrderAsync(Guid orderId)
        {
            _logger.LogInformation("Retrieving order. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindByIdAsync(orderId);
            if (order is null)
            {
                _logger.LogWarning("Order not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            _logger.LogInformation("Order found. OrderId: {OrderId}.", orderId);

            return _mapper.Map<OrderDto>(order);
        }



        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto)
        {
            _logger.LogInformation("Creating order. UserId: {UserId}.", createDto.UserId);

            Order order = _mapper.Map<Order>(createDto);
            order.Id = default;
            order.CreatedAt = DateTime.UtcNow;

            Order createdOrder = await _orderRepository.InsertAsync(order);
            _logger.LogInformation("Order created. OrderId: {OrderId}.", createdOrder.Id);

            return _mapper.Map<OrderDto>(createdOrder);
        }



        public async Task<OrderDto?> UpdateOrderNoteAsync(Guid orderId, UpdateOrderNoteDto updateDto)
        {
            _logger.LogInformation("Updating order note. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindByIdAsync(orderId);
            if (order is null)
            {
                _logger.LogInformation("Cannot update order note. Order not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            order.Note = updateDto.Note;
            order.UpdatedAt = DateTime.UtcNow;

            Order updatedOrder = await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Order updated. OrderId: {OrderId}.", orderId);

            return _mapper.Map<OrderDto>(updatedOrder);

        }



        public async Task<OrderDto?> DeleteOrderAsync(Guid orderId)
        {
            _logger.LogInformation("Deleting order. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindOrderByIdIncludeOrderItemAsync(orderId);
            if (order is null)
            {
                _logger.LogWarning("Cannot delete. Order not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            OrderDto deletedOrder = _mapper.Map<OrderDto>(order);

            if (order.Items.Count > 0)
            {
                _logger.LogInformation("Deleting all related orderItems before deleting Order. Count: {Count}.", order.Items.Count);

                var deletedTask = order.Items.Select(oi => _orderItemRepository.DeleteAsync(oi.Id));
                await Task.WhenAll(deletedTask);

            }

            await _orderRepository.DeleteAsync(orderId);
            _logger.LogInformation("Order deleted. OrderId: {OrderId}.", orderId);

            return deletedOrder;
        }



        public async Task<OrderDto?> ChangeOrderStatusAsync(Guid orderId, ChangeOrderStatusDto statusDto)
        {
            _logger.LogInformation("Changing orderStatus. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindOrderByIdIncludeOrderItemAsync(orderId);
            if (order is null)
            {
                _logger.LogWarning("Cannot change orderStatus. Order not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            OrderStatus currentStatus = order.Status;
            
            if (!IsStatusChangeValid(currentStatus, statusDto.Status))
            {
                _logger.LogWarning("Cannot change orderStatus. Bad validation. OrderId: {OrderId}.", orderId);
                return null;
            }

            Order updatedOrder = await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("OrderStatus changed. OrderId: {OrderId}.", orderId);

            return _mapper.Map<OrderDto>(updatedOrder);
        }





    }
}
