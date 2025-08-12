using AutoMapper;
using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entity;
using OrderService.Domain.Enum;
using Shared.Contracts.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using OrderService.Application.DTOs.External;





namespace OrderService.Application.Services
{
    public class OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IMapper mapper, ILogger<OrderService> logger, IPublishEndpoint publishEndpoint, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : IOrderService
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<OrderService> _logger = logger;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;



        private static bool IsStatusChangeValid(OrderStatus current, OrderStatus next)
        {
            return (current, next) switch
            {
                (OrderStatus.Created, OrderStatus.Paid) => true,
                (OrderStatus.Paid, OrderStatus.Accepted) => true,
                (OrderStatus.Accepted, OrderStatus.Shipped) => true,
                (OrderStatus.Shipped, OrderStatus.Completed) => true,
                (OrderStatus.Created, OrderStatus.Cancelled) => true,
                (OrderStatus.Paid, OrderStatus.Rejected) => true,
                _ => false
            };
        }



        public async Task<IReadOnlyList<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all orders by userId. UserId: {UserId}.", userId);

            IReadOnlyList<Order> orders = await _orderRepository.GetAllOrderByUserIdAsync(userId, ct);
            _logger.LogInformation("Retrieved all orders. Count: {Count}, UserId: {UserId}.", orders.Count, userId);

            return _mapper.Map<List<OrderDto>>(orders);
        }



        public async Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all orders.");

            IReadOnlyList<Order> orders = await _orderRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all orders. Count: {Count}.", orders.Count);

            return _mapper.Map<List<OrderDto>>(orders);
        }



        public async Task<OrderExtendedDto?> GetOrderAsync(Guid orderId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving order. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindOrderByIdIncludeOrderItemAsync(orderId, ct);
            if (order is null)
            {
                _logger.LogWarning("Order not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            _logger.LogInformation("Order found. OrderId: {OrderId}.", orderId);

            return _mapper.Map<OrderExtendedDto>(order);
        }



        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new order. UserId: {UserId}.", createDto.UserId);

            Order order = _mapper.Map<Order>(createDto);
            order.Id = Guid.Empty;
            order.CreatedAt = DateTime.UtcNow;
            order.Status = OrderStatus.Created;

            Order createdOrder = await _orderRepository.InsertAsync(order, ct);
            _logger.LogInformation("Order created. OrderId: {OrderId}.", createdOrder.Id);

            OrderCreatedEvent orderCreatedEvent = new()
            {
                OrderId = createdOrder.Id,
                UserId = createdOrder.UserId,
                TotalPrice = createdOrder.TotalPrice,
                CreatedAt = DateTime.UtcNow,
                Note = order.Note ?? ""
            };

            await _publishEndpoint.Publish(orderCreatedEvent, ct);

            return _mapper.Map<OrderDto>(createdOrder);
        }



        public async Task<OrderDto?> UpdateOrderNoteAsync(Guid orderId, UpdateOrderNoteDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating order note. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindByIdAsync(orderId, ct);
            if (order is null)
            {
                _logger.LogInformation("Cannot update order note. Order not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            order.Note = updateDto.Note;
            order.UpdatedAt = DateTime.UtcNow;

            Order updatedOrder = await _orderRepository.UpdateAsync(order, ct);
            _logger.LogInformation("Order updated. OrderId: {OrderId}.", orderId);

            return _mapper.Map<OrderDto>(updatedOrder);
        }



        public async Task<OrderDto?> DeleteOrderAsync(Guid orderId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting order. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindOrderByIdIncludeOrderItemAsync(orderId, ct);
            if (order is null)
            {
                _logger.LogWarning("Cannot delete. Order not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            OrderDto deletedOrder = _mapper.Map<OrderDto>(order);

            if (order.Items.Count > 0)
            {
                _logger.LogInformation("Deleting all related orderItems before deleting Order. Count: {Count}.", order.Items.Count);

                var deletedTask = order.Items.Select(oi => _orderItemRepository.DeleteAsync(oi.Id, ct));
                await Task.WhenAll(deletedTask);

            }

            await _orderRepository.DeleteAsync(orderId, ct);
            _logger.LogInformation("Order deleted. OrderId: {OrderId}.", orderId);

            return deletedOrder;
        }



        public async Task<OrderDto?> ChangeOrderStatusAsync(Guid orderId, ChangeOrderStatusDto statusDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Changing orderStatus. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindOrderByIdIncludeOrderItemAsync(orderId, ct);
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


            if (statusDto.Status == OrderStatus.Completed)
            {
                var httpClient = _httpClientFactory.CreateClient("DeliveryService");
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                if (!string.IsNullOrWhiteSpace(token))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                DeliveryExternalDto? delivery = await httpClient.GetFromJsonAsync<DeliveryExternalDto>($"api/Delivery/{order.Id}", ct);

                if (delivery is null)
                {
                    _logger.LogWarning("Cannot change orderStatus to Complete. Delivery not found. OrderId: {OrderId}.", orderId);
                    return null;
                }


                if (delivery.Status != DeliveryStatus.Delivered)
                {
                    _logger.LogWarning("Cannot change orderStatus to Complete. DeliveryStatus is not delivered. OrderId: {OrderId}.", orderId);
                    return null;
                }
            }


            order.Status = statusDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            Order updatedOrder = await _orderRepository.UpdateAsync(order,ct);
            _logger.LogInformation("OrderStatus changed. OrderId: {OrderId}.", orderId);


            OrderStatusChangedEvent orderStatusChangedEvent = new()
            {
                OrderId = orderId,
                UserId = order.UserId,
                NewStatus = (Shared.Contracts.Enums.OrderStatus)(int)order.Status,
                UpdatedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(orderStatusChangedEvent,ct);

            return _mapper.Map<OrderDto>(updatedOrder);
        }



        public async Task<OrderDto> CreateOrderFromCartAsync(ExternalCreateOrderDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new order from cart. UserId: {UserId}.", createDto.UserId);

            Order order = _mapper.Map<Order>(createDto);
            order.Id = Guid.Empty;
            order.CreatedAt = DateTime.UtcNow;
            order.Status = OrderStatus.Created;

            Order createdOrder = await _orderRepository.InsertAsync(order,ct);
            _logger.LogInformation("Order from cart created. OrderId: {OrderId}.", createdOrder.Id);

            return _mapper.Map<OrderDto>(createdOrder);
        }



        public async Task<OrderDto?> SetOrderStatusCompletedFromDelivery(Guid orderId, CancellationToken ct = default)
        {
            _logger.LogInformation("Setting orderStatus to completed from DeliveryDeliveredEvent. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindByIdAsync(orderId, ct);
            if (order is null)
            {
                _logger.LogWarning("Cannot set orderStatus to completed. Order not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            order.Status = OrderStatus.Completed;
            order.UpdatedAt = DateTime.UtcNow;

            Order updatedOrder = await _orderRepository.UpdateAsync(order, ct);
            _logger.LogInformation("OrderStatus set to completed. OrderId: {OrderId}.", orderId);

            return _mapper.Map<OrderDto>(updatedOrder);
        }



    }
}
