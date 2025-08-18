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
using Shared.Contracts.DTOs;
using OrderService.Application.Interfaces.External;
using Grpc.Core;





namespace OrderService.Application.Services
{
    public class OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IMapper mapper, ILogger<OrderService> logger, IPublishEndpoint publishEndpoint, IDeliveryReadClient deliveryReadClient) : IOrderService
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<OrderService> _logger = logger;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly IDeliveryReadClient _deliveryReadClient = deliveryReadClient;
    



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

            await _orderRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Order updated. OrderId: {OrderId}.", orderId);

            return _mapper.Map<OrderDto>(order);
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

            _orderRepository.Remove(order);
            await _orderRepository.SaveChangesAsync(ct);
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


                var status = await _deliveryReadClient.GetDeliveryStatusByOrderIdAsync(orderId, ct);

                if (status is null)
                {
                    _logger.LogWarning("Cannot change orderStatus to Complete. Delivery not found. OrderId: {OrderId}.", orderId);
                    return null;
                }


                if (status != Shared.Contracts.Enums.DeliveryStatus.Delivered)
                {
                    _logger.LogWarning("Cannot change orderStatus to Complete. DeliveryStatus is not delivered. OrderId: {OrderId}.", orderId);
                    return null;
                }
            }


            order.Status = statusDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.SaveChangesAsync(ct);
            _logger.LogInformation("OrderStatus changed. OrderId: {OrderId}.", orderId);


            OrderStatusChangedEvent orderStatusChangedEvent = new()
            {
                OrderId = orderId,
                UserId = order.UserId,
                NewStatus = (Shared.Contracts.Enums.OrderStatus)(int)order.Status,
                UpdatedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(orderStatusChangedEvent, ct);

            return _mapper.Map<OrderDto>(order);
        }



        public async Task<OrderDto?> CreateOrderAndDeliveryFromCartAsync(ExternalCreateOrderDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new order from cart. UserId: {UserId}.", createDto.UserId);

            Order order = new()
            {
                Id = Guid.Empty,
                UserId = createDto.UserId,
                TotalPrice = createDto.TotalPrice,
                Status = OrderStatus.Created,
                Note = createDto.Note,
                CreatedAt = DateTime.UtcNow,
                Items = _mapper.Map<List<OrderItem>>(createDto.Items)
            };

            Order createdOrder = await _orderRepository.InsertAsync(order, ct);
            _logger.LogInformation("Order from cart created. OrderId: {OrderId}.", createdOrder.Id);


            CreateDeliveryDto createDeliveryDto = new()
            {
                OrderId = createdOrder.Id,
                CourierId = createDto.CourierId,
                Email = createDto.Email,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                PhoneNumber = createDto.PhoneNumber,
                Street = createDto.Street,
                City = createDto.City,
                PostalCode = createDto.PostalCode,
                State = createDto.State
            };

            try
            {
                Guid createdDeliveryId = await _deliveryReadClient.CreateDeliveryAsync(createDeliveryDto, ct);
            }
            catch(RpcException ex)
            {
               _logger.LogError(ex, "Failed to create delivery for OrderId {OrderId}", createdOrder.Id);
                return null;
            }

            OrderCreatedEvent orderCreatedEvent = new()
            {
                OrderId = createdOrder.Id,
                UserId = createdOrder.UserId,
                TotalPrice = createdOrder.TotalPrice,
                CreatedAt = DateTime.UtcNow,
                Note = order.Note ?? "",
            };

            // Order created notification
            await _publishEndpoint.Publish(orderCreatedEvent, ct);

            OrderItemsReservedEvent orderItemsReservedEvent = new()
            {
                OrderId = createdOrder.Id,
                Items = [.. createdOrder.Items.Select(p => new OrderItemCreatedDto { ProductId = p.ProductId, Quantity = p.Quantity })]
            };

            // Reserve products for orderItems
            await _publishEndpoint.Publish(orderItemsReservedEvent, ct);

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

            await _orderRepository.SaveChangesAsync(ct);
            _logger.LogInformation("OrderStatus set to completed. OrderId: {OrderId}.", orderId);

            return _mapper.Map<OrderDto>(order);
        }



    }
}
