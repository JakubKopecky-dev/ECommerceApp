using Grpc.Core;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.External;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces.External;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Common;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Events;
using Shared.Contracts.DTOs;
using Shared.Contracts.Events;
using System.Net.Http.Headers;
using System.Net.Http.Json;





namespace OrderService.Application.Services
{
    public class OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger, IPublishEndpoint publishEndpoint, IDeliveryReadClient deliveryReadClient, IPaymentReadClient paymentReadClient) : IOrderService
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly ILogger<OrderService> _logger = logger;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly IDeliveryReadClient _deliveryReadClient = deliveryReadClient;
        private readonly IPaymentReadClient _paymentReadClient = paymentReadClient;


        public async Task<IReadOnlyList<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all orders by userId. UserId: {UserId}.", userId);

            IReadOnlyList<Order> orders = await _orderRepository.GetAllOrderByUserIdAsync(userId, ct);
            _logger.LogInformation("Retrieved all orders. Count: {Count}, UserId: {UserId}.", orders.Count, userId);

            return [.. orders.Select(o => o.OrderToOrderDto())];
        }



        public async Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all orders.");

            IReadOnlyList<Order> orders = await _orderRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all orders. Count: {Count}.", orders.Count);

            return [.. orders.Select(o => o.OrderToOrderDto())];
        }



        public async Task<OrderExtendedDto?> GetOrderByIdAsync(Guid orderId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving order. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindOrderByIdIncludeOrderItemAsync(orderId, ct);
            if (order is null)
            {
                _logger.LogWarning("Order not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            _logger.LogInformation("Order found. OrderId: {OrderId}.", orderId);

            return order.OrderToOrderExtendedDto();
        }



        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new order. UserId: {UserId}.", createDto.UserId);

            Order order = Order.Create(createDto.UserId, createDto.Note);

            await _orderRepository.AddAsync(order, ct);
            await _orderRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Order created. OrderId: {OrderId}.", order.Id);

            return order.OrderToOrderDto();
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

            order.NoteUpdate(updateDto.Note);

            await _orderRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Order updated. OrderId: {OrderId}.", orderId);

            return order.OrderToOrderDto();
        }



        public async Task<bool> DeleteOrderAsync(Guid orderId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting order. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindOrderByIdIncludeOrderItemAsync(orderId, ct);
            if (order is null)
            {
                _logger.LogWarning("Cannot delete. Order not found. OrderId: {OrderId}.", orderId);
                return false;
            }


            _orderRepository.Remove(order);
            await _orderRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Order deleted. OrderId: {OrderId}.", orderId);

            return true;
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


            try
            {
                order.ChangeStatus(statusDto.Status);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("Cannot change orderStatus. {Message}. OrderId: {OrderId}.", ex.Message, orderId);
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

            await _orderRepository.SaveChangesAsync(ct);
            _logger.LogInformation("OrderStatus changed. OrderId: {OrderId}.", orderId);

            foreach (var domainEvent in order.PopDomainEvents())
            {
                if (domainEvent is OrderStatusChangedDomainEvent e)
                {
                    await _publishEndpoint.Publish(new OrderStatusChangedEvent
                    {
                        OrderId = e.OrderId,
                        UserId = e.UserId,
                        NewStatus = (Shared.Contracts.Enums.OrderStatus)(int)e.NewStatus,
                        UpdatedAt = DateTime.UtcNow
                    }, ct);
                }
            }


            return order.OrderToOrderDto();
        }



        /// <summary>
        /// Creates a new order and associated delivery based on the specified cart information, and initiates the
        /// checkout process.
        /// </summary>
        /// <remarks>If delivery creation fails, the order is still created but its internal status is
        /// updated to indicate the delivery failure, and the delivery ID in the response will be null. The method also
        /// publishes events for order creation and item reservation, and creates a payment session for the
        /// order.</remarks>
        /// <param name="createDto">An object containing the details required to create the order and delivery, including user information, cart
        /// items, delivery address, and contact details. Cannot be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response object with the
        /// created order ID, delivery ID (if successful), and the checkout URL for payment.</returns>
        public async Task<CreateOrderFromCartResponseDto> CreateOrderAndDeliveryFromCartAsync(ExternalCreateOrderDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new order from cart. UserId: {UserId}.", createDto.UserId);

            CreateOrderFromCartResponseDto responseDto = new();

            Order order = Order.CreateFromCart(createDto.UserId, createDto.TotalPrice, createDto.Note, [..createDto.Items.Select(i =>
                OrderItem.CreateFromOrder(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity))]);

       

            await _orderRepository.AddAsync(order, ct);
            await _orderRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Order from cart created. OrderId: {OrderId}.", order.Id);

            responseDto.OrderId = order.Id;

            CreateDeliveryDto createDeliveryDto = new()
            {
                OrderId = order.Id,
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
                responseDto.DeliveryId = createdDeliveryId;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Failed to create delivery for OrderId {OrderId}", order.Id);
                responseDto.DeliveryId = null;

                order.ChangeInternalStatus(InternalOrderStatus.DeliveryFaild);
                await _orderRepository.SaveChangesAsync(ct);
            }

            OrderCreatedEvent orderCreatedEvent = new()
            {
                OrderId = order.Id,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice,
                CreatedAt = DateTime.UtcNow,
                Note = order.Note ?? "",
            };

            // Order created notification
            await _publishEndpoint.Publish(orderCreatedEvent, ct);

            OrderItemsReservedEvent orderItemsReservedEvent = new()
            {
                OrderId = order.Id,
                Items = [.. order.Items.Select(p => new OrderItemCreatedDto 
                { 
                    ProductId = p.ProductId,
                    Quantity = p.Quantity 
                })]
            };

            // Reserve products for orderItems
            await _publishEndpoint.Publish(orderItemsReservedEvent, ct);


            // Create payment session
            CreateCheckoutSessionRequestDto checkoutSessionrequestDto = new()
            {
                Id = order.Id,
                Items = [..order.Items.Select(i => new OrderItemForCreateCheckoutSessionRequestDto
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                })]
            };

            CreateCheckoutSessionResponseDto? checkoutSessionResponseDto = await _paymentReadClient.CreateCheckoutSessionAsync(checkoutSessionrequestDto, ct);
            responseDto.CheckoutUrl = checkoutSessionResponseDto?.CheckoutUrl;

            if (responseDto.CheckoutUrl is not null)
                _logger.LogInformation("Checkout url created for orderId: {OrderId}.", order.Id);
            else
                _logger.LogWarning("Checkout url not created for orderId: {OrderId}.", order.Id);

            return responseDto;
        }



        public async Task<OrderDto?> ChangeInternalOrderStatusAsync(Guid orderId, ChangeInternalOrderStatusDto changeDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Changing internal orderStatus. OrderId: {OrderId}.", orderId);

            Order? order = await _orderRepository.FindByIdAsync(orderId, ct);
            if (order is null)
            {
                _logger.LogInformation("Cannot change internal orderStatus. Order not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            order.ChangeInternalStatus(changeDto.InternalStatus);
            await _orderRepository.SaveChangesAsync(ct);

            return order.OrderToOrderDto();
        }



        public async Task<IReadOnlyList<OrderDto>> GetAllOrdersWithDeliveryFaildInternalStatusAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all orders with DeliveryFaild internal status.");

            IReadOnlyList<Order> orders = await _orderRepository.GetAllOrderStatusWithDeliveryFaildInternalStatus(ct);
            _logger.LogInformation("Retrieved all orders with DeliveryFaild internal status. Count: {Count}.", orders.Count);

            return [.. orders.Select(o => o.OrderToOrderDto())];
        }



    }
}
