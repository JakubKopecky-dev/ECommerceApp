using AutoMapper;
using FluentAssertions;
using Grpc.Core;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.DTOs.External;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces.External;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entity;
using OrderService.Domain.Enum;
using Shared.Contracts.Events;
using DeliveryStatusContract = Shared.Contracts.Enums.DeliveryStatus;
using OrderServiceService = OrderService.Application.Services.OrderService;

namespace OrderService.UnitTests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersByUserIdAsync_ReturnsOrderDtoList_WhenExists()
        {
            Guid userId = Guid.NewGuid();

            List<Order> orders =
            [
                new() { Id = Guid.NewGuid(), UserId = userId },
                new() { Id = Guid.NewGuid(), UserId = userId }
            ];

            List<OrderDto> expectedDto =
            [
                new() { Id = orders[0].Id, UserId = userId },
                new() { Id = orders[1].Id, UserId = userId }
            ];

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllOrderByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            mapperMock
                .Setup(m => m.Map<List<OrderDto>>(orders))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            IReadOnlyList<OrderDto> result = await service.GetAllOrdersByUserIdAsync(userId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.GetAllOrderByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<OrderDto>>(orders), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersByUserIdAsync_ReturnsEmptyList_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            List<Order> orders = [];
            List<OrderDto> expectedDto = [];

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllOrderByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            mapperMock
                .Setup(m => m.Map<List<OrderDto>>(orders))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            IReadOnlyList<OrderDto> result = await service.GetAllOrdersByUserIdAsync(userId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.GetAllOrderByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<OrderDto>>(orders), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersAsync_ReturnsOrderDtoList_WhenExists()
        {
            List<Order> orders =
            [
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            ];

            List<OrderDto> expectedDto =
            [
                new() { Id = orders[0].Id },
                new() { Id = orders[1].Id }
            ];

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            mapperMock
                .Setup(m => m.Map<List<OrderDto>>(orders))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            IReadOnlyList<OrderDto> result = await service.GetAllOrdersAsync(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<OrderDto>>(orders), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersAsync_ReturnsEmptyList_WhenNotExists()
        {
            List<Order> orders = [];
            List<OrderDto> expectedDto = [];

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            mapperMock
                .Setup(m => m.Map<List<OrderDto>>(orders))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            IReadOnlyList<OrderDto> result = await service.GetAllOrdersAsync(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<OrderDto>>(orders), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrderByIdAsync_ReturnsOrderExtendedDto_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            Order order = new() { Id = orderId, Items = [] };
            OrderExtendedDto expectedDto = new() { Id = orderId, Items = [] };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            mapperMock
                .Setup(m => m.Map<OrderExtendedDto>(order))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderExtendedDto? result = await service.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderExtendedDto>(order), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrderAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderExtendedDto? result = await service.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderExtendedDto>(It.IsAny<Order>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateOrderAsync_ReturnsOrderDto()
        {
            CreateOrderDto createDto = new() { UserId = Guid.NewGuid(), Note = "Wrap as a gift please." };

            Order order = new() { Id = Guid.Empty, UserId = createDto.UserId, CreatedAt = DateTime.UtcNow, Status = OrderStatus.Created };
            Order createdOrder = new() { Id = Guid.NewGuid(), UserId = createDto.UserId, CreatedAt = order.CreatedAt, Status = OrderStatus.Created };
            OrderDto expectedDto = new() { Id = createdOrder.Id, UserId = createDto.UserId, Status = OrderStatus.Created, CreatedAt = createdOrder.CreatedAt };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<Order>(createDto))
                .Returns(order);

            orderRepositoryMock
                .Setup(o => o.InsertAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdOrder);

            mapperMock
                .Setup(m => m.Map<OrderDto>(createdOrder))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto result = await service.CreateOrderAsync(createDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            mapperMock.Verify(m => m.Map<Order>(createDto), Times.Once);
            orderRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderDto>(createdOrder), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateOrderNoteAsync_ReturnsOrderDto_WhenExists()
        {
            Guid orderId = Guid.NewGuid();
            UpdateOrderNoteDto updateDto = new() { Note = "Note" };

            Order order = new() { Id = orderId, Note = "", UpdatedAt = DateTime.UtcNow };
            OrderDto expectedDto = new() { Id = orderId, Note = updateDto.Note, UpdatedAt = order.UpdatedAt };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<OrderDto>(order))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.UpdateOrderNoteAsync(orderId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderDto>(order), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateOrderNoteAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();
            UpdateOrderNoteDto updateDto = new() { Note = "Note" };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.UpdateOrderNoteAsync(orderId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrderAsync_ReturnsOrderDto_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            Order order = new() { Id = orderId, Items = [] };
            OrderDto expectedDto = new() { Id = orderId };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            mapperMock
                .Setup(m => m.Map<OrderDto>(order))
                .Returns(expectedDto);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.DeleteOrderAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderDto>(order), Times.Once);
            orderRepositoryMock.Verify(o => o.Remove(order), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrderAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(r => r.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.DeleteOrderAsync(orderId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(r => r.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
            orderRepositoryMock.Verify(r => r.Remove(It.IsAny<Order>()), Times.Never);
            orderRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_CreatedToPaid()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Paid };

            Order order = new() { Id = orderId, Status = OrderStatus.Created, UserId = Guid.NewGuid() };
            OrderDto expectedDto = new() { Id = orderId, Status = OrderStatus.Paid };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<OrderDto>(order))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderDto>(order), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_PaidToAccepted()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Accepted };

            Order order = new() { Id = orderId, Status = OrderStatus.Paid, UserId = Guid.NewGuid() };
            OrderDto expectedDto = new() { Id = orderId, Status = OrderStatus.Accepted };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<OrderDto>(order))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_AcceptedToShipped()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Shipped };

            Order order = new() { Id = orderId, Status = OrderStatus.Accepted, UserId = Guid.NewGuid() };
            OrderDto expectedDto = new() { Id = orderId, Status = OrderStatus.Shipped };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<OrderDto>(order))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_ShippedToCompleted_WithDeliveredDelivery()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Completed };

            Order order = new() { Id = orderId, Status = OrderStatus.Shipped, UserId = Guid.NewGuid() };
            OrderDto expectedDto = new() { Id = orderId, Status = OrderStatus.Completed };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            deliveryReadClientMock
                .Setup(d => d.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(DeliveryStatusContract.Delivered);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<OrderDto>(order))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderDto>(order), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_CreatedToCancelled()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Cancelled };

            Order order = new() { Id = orderId, Status = OrderStatus.Created, UserId = Guid.NewGuid() };
            OrderDto expectedDto = new() { Id = orderId, Status = OrderStatus.Cancelled };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<OrderDto>(order))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsUpdated_WhenValidTransition_PaidToRejected()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Rejected };

            Order order = new() { Id = orderId, Status = OrderStatus.Paid, UserId = Guid.NewGuid() };
            OrderDto expectedDto = new() { Id = orderId, Status = OrderStatus.Rejected };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<OrderDto>(order))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

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
            Mock<IMapper> mapperMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsNull_WhenInvalidTransition()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Shipped };

            Order order = new() { Id = orderId, Status = OrderStatus.Created, UserId = Guid.NewGuid() };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            deliveryReadClientMock.Verify(d => d.GetDeliveryStatusByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsNull_WhenCompletingAndDeliveryNotFound()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Completed };

            Order order = new() { Id = orderId, Status = OrderStatus.Shipped, UserId = Guid.NewGuid() };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            deliveryReadClientMock
                .Setup(d => d.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeliveryStatusContract?)null);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatusAsync_ReturnsNull_WhenCompletingAndDeliveryNotDelivered()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderStatus.Completed };

            Order order = new() { Id = orderId, Status = OrderStatus.Shipped, UserId = Guid.NewGuid() };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();

            orderRepositoryMock
                .Setup(o => o.FindOrderByIdIncludeOrderItemAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            deliveryReadClientMock
                .Setup(d => d.GetDeliveryStatusByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(DeliveryStatusContract.InProgress);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
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
                Items = [new() { ProductId = Guid.NewGuid(), Quantity = 2 }]
            };

            Order createdOrder = new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TotalPrice = createDto.TotalPrice,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow,
                Items = []
            };

            List<OrderItem> mappedItems = [new() { Id = Guid.NewGuid(), ProductId = createDto.Items[0].ProductId, Quantity = createDto.Items[0].Quantity, Order = createdOrder }];
            createdOrder.Items = mappedItems;

            Guid createdDeliveryId = Guid.NewGuid();

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();

            mapperMock
                .Setup(m => m.Map<List<OrderItem>>(createDto.Items))
                .Returns(mappedItems);

            orderRepositoryMock
                .Setup(o => o.InsertAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdOrder);

            deliveryReadClientMock
                .Setup(d => d.CreateDeliveryAsync(It.IsAny<CreateDeliveryDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDeliveryId);

            paymentReadClientMock
                .Setup(p => p.CreateCheckoutSessionAsync(It.IsAny<CreateCheckoutSessionRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<CreateCheckoutSessionResponseDto?>());


            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                paymentReadClientMock.Object
            );


            CreateOrderFromCartResponseDto result = await service.CreateOrderAndDeliveryFromCartAsync(createDto, It.IsAny<CancellationToken>());

            result.OrderId.Should().Be(createdOrder.Id);
            result.DeliveryId.Should().Be(createdDeliveryId);

            mapperMock.Verify(m => m.Map<List<OrderItem>>(createDto.Items), Times.Once);
            orderRepositoryMock.Verify(o => o.InsertAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.CreateDeliveryAsync(It.IsAny<CreateDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderItemsReservedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            paymentReadClientMock.Verify(p => p.CreateCheckoutSessionAsync(It.IsAny<CreateCheckoutSessionRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
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
                Items = [new() { ProductId = Guid.NewGuid(), Quantity = 2 }]
            };

            Order createdOrder = new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TotalPrice = createDto.TotalPrice,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow,
                Items = []
            };

            List<OrderItem> mappedItems = [new() { Id = Guid.NewGuid(), ProductId = createDto.Items[0].ProductId, Quantity = createDto.Items[0].Quantity, Order = createdOrder }];
            createdOrder.Items = mappedItems;

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IPublishEndpoint> publishEndpointMock = new();
            Mock<IDeliveryReadClient> deliveryReadClientMock = new();
            Mock<IPaymentReadClient> paymentReadClientMock = new();


            mapperMock
                .Setup(m => m.Map<List<OrderItem>>(createDto.Items))
                .Returns(mappedItems);

            orderRepositoryMock
                .Setup(o => o.InsertAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdOrder);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            deliveryReadClientMock
                .Setup(d => d.CreateDeliveryAsync(It.IsAny<CreateDeliveryDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RpcException(new Status(StatusCode.Internal, "error")));

            paymentReadClientMock
                .Setup(p => p.CreateCheckoutSessionAsync(It.IsAny<CreateCheckoutSessionRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<CreateCheckoutSessionResponseDto?>());

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                publishEndpointMock.Object,
                deliveryReadClientMock.Object,
                paymentReadClientMock.Object
            );


            CreateOrderFromCartResponseDto result = await service.CreateOrderAndDeliveryFromCartAsync(createDto, It.IsAny<CancellationToken>());

            result.OrderId.Should().Be(createdOrder.Id);
            result.DeliveryId.Should().BeNull();

            orderRepositoryMock.Verify(o => o.InsertAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderItemsReservedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            deliveryReadClientMock.Verify(d => d.CreateDeliveryAsync(It.IsAny<CreateDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Once);
            paymentReadClientMock.Verify(p => p.CreateCheckoutSessionAsync(It.IsAny<CreateCheckoutSessionRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeInternalOrderStatusAsync_ReturnsOrderDto_WhenExists()
        {
            Guid orderId = Guid.NewGuid();
            ChangeInternalOrderStatusDto changeDto = new() { InternalStatus = InternalOrderStatus.DeliveryFaild };

            Order order = new() { Id = orderId, InternalStatus = InternalOrderStatus.None, UpdatedAt = DateTime.UtcNow };
            OrderDto expectedDto = new() { Id = orderId, UpdatedAt = order.UpdatedAt };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            orderRepositoryMock
                .Setup(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<OrderDto>(order))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeInternalOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderDto>(order), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeInternalOrderStatusAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();
            ChangeInternalOrderStatusDto changeDto = new() { InternalStatus = InternalOrderStatus.DeliveryFaild };

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            OrderDto? result = await service.ChangeInternalOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderRepositoryMock.Verify(o => o.FindByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            orderRepositoryMock.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersWithDeliveryFaildInternalStatusAsync_ReturnsOrderDtoList_WhenExists()
        {
            List<Order> orders =
            [
                new() { Id = Guid.NewGuid(), InternalStatus = InternalOrderStatus.DeliveryFaild },
                new() { Id = Guid.NewGuid(), InternalStatus = InternalOrderStatus.DeliveryFaild }
            ];

            List<OrderDto> expectedDto =
            [
                new() { Id = orders[0].Id },
                new() { Id = orders[1].Id }
            ];

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllOrderStatusWithDeliveryFaildInternalStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            mapperMock
                .Setup(m => m.Map<List<OrderDto>>(orders))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            IReadOnlyList<OrderDto> result = await service.GetAllOrdersWithDeliveryFaildInternalStatusAsync(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.GetAllOrderStatusWithDeliveryFaildInternalStatus(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<OrderDto>>(orders), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersWithDeliveryFaildInternalStatusAsync_ReturnsEmptyList_WhenNotExists()
        {
            List<Order> orders = [];
            List<OrderDto> expectedDto = [];

            Mock<IOrderRepository> orderRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderRepositoryMock
                .Setup(o => o.GetAllOrderStatusWithDeliveryFaildInternalStatus(It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            mapperMock
                .Setup(m => m.Map<List<OrderDto>>(orders))
                .Returns(expectedDto);

            OrderServiceService service = new(
                orderRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderServiceService>>().Object,
                new Mock<IPublishEndpoint>().Object,
                new Mock<IDeliveryReadClient>().Object,
                new Mock<IPaymentReadClient>().Object
            );


            IReadOnlyList<OrderDto> result = await service.GetAllOrdersWithDeliveryFaildInternalStatusAsync(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderRepositoryMock.Verify(o => o.GetAllOrderStatusWithDeliveryFaildInternalStatus(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<OrderDto>>(orders), Times.Once);
        }



    }
}
