using FluentAssertions;
using Grpc.Core;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.DTOs.External;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces.External;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using Shared.Contracts.Events;
using System.Reflection;
using DeliveryStatusContract = Shared.Contracts.Enums.DeliveryStatus;
using OrderServiceService = OrderService.Application.Services.OrderService;

namespace OrderService.UnitTests.Services
{
    public class OrderServiceTests
    {
        private static OrderServiceService CreateService(
            Mock<IOrderRepository> orderRepositoryMock,
            Mock<IPublishEndpoint> publishEndpointMock,
            Mock<IDeliveryReadClient> deliveryReadClientMock,
            Mock<IPaymentReadClient> paymentReadClientMock)
        {
            return new OrderServiceService(
                orderRepositoryMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                paymentReadClientMock.Object
            );
        }

        private static void AddItems(Order order, IEnumerable<OrderItem> items)
        {
            var field = typeof(Order).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (List<OrderItem>)field!.GetValue(order)!;
            list.AddRange(items);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersByUserIdAsync_ReturnsOrderDtoList_WhenExists()
        {
            Guid userId = Guid.NewGuid();

            List<Order> orders =
            [
                Order.Create(userId, null),
                Order.Create(userId, null)
            ];

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllOrderByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            IReadOnlyList<OrderDto> result = await service.GetAllOrdersByUserIdAsync(userId, It.IsAny<CancellationToken>());

            result.Should().HaveCount(2);
            result.Should().AllSatisfy(o => o.UserId.Should().Be(userId));

            orderRepositoryMock.Verify(o => o.GetAllOrderByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersByUserIdAsync_ReturnsEmptyList_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllOrderByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            IReadOnlyList<OrderDto> result = await service.GetAllOrdersByUserIdAsync(userId, It.IsAny<CancellationToken>());

            result.Should().BeEmpty();

            orderRepositoryMock.Verify(o => o.GetAllOrderByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersAsync_ReturnsOrderDtoList_WhenExists()
        {
            List<Order> orders =
            [
                Order.Create(Guid.NewGuid(), null),
                Order.Create(Guid.NewGuid(), null)
            ];

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            IReadOnlyList<OrderDto> result = await service.GetAllOrdersAsync(It.IsAny<CancellationToken>());

            result.Should().HaveCount(2);

            orderRepositoryMock.Verify(o => o.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersAsync_ReturnsEmptyList_WhenNotExists()
        {
            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            IReadOnlyList<OrderDto> result = await service.GetAllOrdersAsync(It.IsAny<CancellationToken>());

            result.Should().BeEmpty();

            orderRepositoryMock.Verify(o => o.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrderByIdAsync_ReturnsOrderExtendedDto_WhenExists()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            Guid orderId = order.Id;

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderExtendedDto? result = await service.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Id.Should().Be(orderId);

            orderRepositoryMock.Verify(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrderByIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderExtendedDto? result = await service.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateOrderAsync_ReturnsOrderDto()
        {
            CreateOrderDto createDto = new() { UserId = Guid.NewGuid(), Note = "Wrap as a gift please." };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto result = await service.CreateOrderAsync(createDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result.UserId.Should().Be(createDto.UserId);
            result.Note.Should().Be(createDto.Note);
            result.Status.Should().Be(OrderStatus.Created);

            orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateOrderNoteAsync_ReturnsOrderDto_WhenExists()
        {
            Order order = Order.Create(Guid.NewGuid(), "Old note");
            Guid orderId = order.Id;
            UpdateOrderNoteDto updateDto = new() { Note = "New note" };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.UpdateOrderNoteAsync(orderId, updateDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Note.Should().Be(updateDto.Note);

            orderRepositoryMock.Verify(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateOrderNoteAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();
            UpdateOrderNoteDto updateDto = new() { Note = "Note" };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.UpdateOrderNoteAsync(orderId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrderAsync_ReturnsTrue_WhenExists()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            Guid orderId = order.Id;

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            bool result = await service.DeleteOrderAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeTrue();

            orderRepositoryMock.Verify(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.Remove(order), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrderAsync_ReturnsFalse_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            bool result = await service.DeleteOrderAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeFalse();

            orderRepositoryMock.Verify(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.Remove(It.IsAny<Order>()), Times.Never);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_CreatedToPaid()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            Guid orderId = order.Id;
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Paid };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Status.Should().Be(OrderStatus.Paid);

            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_PaidToAccepted()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            order.ChangeStatus(OrderStatus.Paid);
            order.PopDomainEvents(); // vyčistíme eventy z předchozí změny
            Guid orderId = order.Id;
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Accepted };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Status.Should().Be(OrderStatus.Accepted);

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_AcceptedToShipped()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            order.ChangeStatus(OrderStatus.Paid);
            order.ChangeStatus(OrderStatus.Accepted);
            order.PopDomainEvents();
            Guid orderId = order.Id;
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Shipped };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Status.Should().Be(OrderStatus.Shipped);

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_ShippedToCompleted_WithDeliveredDelivery()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            order.ChangeStatus(OrderStatus.Paid);
            order.ChangeStatus(OrderStatus.Accepted);
            order.ChangeStatus(OrderStatus.Shipped);
            order.PopDomainEvents();
            Guid orderId = order.Id;
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Completed };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            deliveryReadClientMock
                .Setup(d => d.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(DeliveryStatusContract.Delivered);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Status.Should().Be(OrderStatus.Completed);

            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_CreatedToCancelled()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            Guid orderId = order.Id;
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Cancelled };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Status.Should().Be(OrderStatus.Cancelled);

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_PaidToRejected()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            order.ChangeStatus(OrderStatus.Paid);
            order.PopDomainEvents();
            Guid orderId = order.Id;
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Rejected };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Status.Should().Be(OrderStatus.Rejected);

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsNull_WhenOrderNotExists()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Paid };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsNull_WhenInvalidTransition()
        {
            Order order = Order.Create(Guid.NewGuid(), null); // Created
            Guid orderId = order.Id;
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Shipped }; // Created -> Shipped je invalid

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsNull_WhenCompletingAndDeliveryNotFound()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            order.ChangeStatus(OrderStatus.Paid);
            order.ChangeStatus(OrderStatus.Accepted);
            order.ChangeStatus(OrderStatus.Shipped);
            order.PopDomainEvents();
            Guid orderId = order.Id;
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Completed };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            deliveryReadClientMock
                .Setup(d => d.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeliveryStatusContract?)null);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsNull_WhenCompletingAndDeliveryNotDelivered()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            order.ChangeStatus(OrderStatus.Paid);
            order.ChangeStatus(OrderStatus.Accepted);
            order.ChangeStatus(OrderStatus.Shipped);
            order.PopDomainEvents();
            Guid orderId = order.Id;
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Completed };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            deliveryReadClientMock
                .Setup(d => d.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(DeliveryStatusContract.InProgress);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateOrderAndDeliveryFromCartAsync_ReturnsIdsAndPublishesEvents_WhenDeliveryCreated()
        {
            Guid userId = Guid.NewGuid();
            ExternalCreateOrderDto createDto = new()
            {
                UserId = userId,
                TotalPrice = 1999,
                Note = "Wrap as a gift",
                CourierId = Guid.NewGuid(),
                Email = "a@b.com",
                FirstName = "James",
                LastName = "Cook",
                PhoneNumber = "285421365",
                Street = "Vinohradska",
                City = "Prague",
                PostalCode = "12000",
                State = "CZ",
                Items = [new() { ProductId = Guid.NewGuid(), ProductName = "iPhone 16", UnitPrice = 1999, Quantity = 1 }]
            };

            Guid createdDeliveryId = Guid.NewGuid();

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            deliveryReadClientMock
                .Setup(d => d.CreateDeliveryAsync(It.IsAny<CreateDeliveryDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDeliveryId);

            paymentReadClientMock
                .Setup(p => p.CreateCheckoutSessionAsync(It.IsAny<CreateCheckoutSessionRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateCheckoutSessionResponseDto { CheckoutUrl = "www.url.com" });

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            CreateOrderFromCartResponseDto result = await service.CreateOrderAndDeliveryFromCartAsync(createDto, It.IsAny<CancellationToken>());

            result.DeliveryId.Should().Be(createdDeliveryId);
            result.CheckoutUrl.Should().Be("www.url.com");

            orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.CreateDeliveryAsync(It.IsAny<CreateDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderItemsReservedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            paymentReadClientMock.Verify(p => p.CreateCheckoutSessionAsync(It.IsAny<CreateCheckoutSessionRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateOrderAndDeliveryFromCartAsync_SetsInternalStatusAndPublishesEvents_WhenDeliveryCreationFails()
        {
            Guid userId = Guid.NewGuid();
            ExternalCreateOrderDto createDto = new()
            {
                UserId = userId,
                TotalPrice = 1999,
                Note = "Wrap as a gift",
                CourierId = Guid.NewGuid(),
                Email = "a@b.com",
                FirstName = "James",
                LastName = "Cook",
                PhoneNumber = "285421365",
                Street = "Vinohradska",
                City = "Prague",
                PostalCode = "12000",
                State = "CZ",
                Items = [new() { ProductId = Guid.NewGuid(), ProductName = "iPhone 16", UnitPrice = 1999, Quantity = 1 }]
            };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            deliveryReadClientMock
                .Setup(d => d.CreateDeliveryAsync(It.IsAny<CreateDeliveryDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "error")));

            paymentReadClientMock
                .Setup(p => p.CreateCheckoutSessionAsync(It.IsAny<CreateCheckoutSessionRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CreateCheckoutSessionResponseDto?)null);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            CreateOrderFromCartResponseDto result = await service.CreateOrderAndDeliveryFromCartAsync(createDto, It.IsAny<CancellationToken>());

            result.DeliveryId.Should().BeNull();

            orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            deliveryReadClientMock.Verify(d => d.CreateDeliveryAsync(It.IsAny<CreateDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderItemsReservedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeInternalOrderStatusAsync_ReturnsOrderDto_WhenExists()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            Guid orderId = order.Id;
            ChangeInternalOrderStatusDto changeDto = new() { InternalStatus = InternalOrderStatus.DeliveryFaild };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeInternalOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();

            orderRepositoryMock.Verify(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeInternalOrderStatusAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();
            ChangeInternalOrderStatusDto changeDto = new() { InternalStatus = InternalOrderStatus.DeliveryFaild };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            OrderDto? result = await service.ChangeInternalOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersWithDeliveryFaildInternalStatusAsync_ReturnsOrderDtoList_WhenExists()
        {
            List<Order> orders =
            [
                Order.Create(Guid.NewGuid(), null),
                Order.Create(Guid.NewGuid(), null)
            ];

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllOrderStatusWithDeliveryFaildInternalStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            IReadOnlyList<OrderDto> result = await service.GetAllOrdersWithDeliveryFaildInternalStatusAsync(It.IsAny<CancellationToken>());

            result.Should().HaveCount(2);

            orderRepositoryMock.Verify(o => o.GetAllOrderStatusWithDeliveryFaildInternalStatus(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersWithDeliveryFaildInternalStatusAsync_ReturnsEmptyList_WhenNotExists()
        {
            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllOrderStatusWithDeliveryFaildInternalStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(orderRepositoryMock, publishEndpointMock, deliveryReadClientMock, paymentReadClientMock);

            IReadOnlyList<OrderDto> result = await service.GetAllOrdersWithDeliveryFaildInternalStatusAsync(It.IsAny<CancellationToken>());

            result.Should().BeEmpty();

            orderRepositoryMock.Verify(o => o.GetAllOrderStatusWithDeliveryFaildInternalStatus(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}