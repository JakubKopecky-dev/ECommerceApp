using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Services;
using OrderService.Domain.Entities;

namespace OrderService.UnitTests.Services
{
    public class OrderItemServiceTests
    {
        private static OrderItemService CreateService(Mock<IOrderItemRepository> orderItemRepositoryMock)
        {
            return new OrderItemService(
                orderItemRepositoryMock.Object,
                new Mock<ILogger<OrderItemService>>().Object
            );
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrderItemsByOrderIdAsync_ReturnOrderItemDtoList_WhenExists()
        {
            Order order = Order.Create(Guid.NewGuid(), null);

            List<OrderItem> orderItems =
            [
                OrderItem.Create(Guid.NewGuid(), "iPhone 16", 1299m, 1, order.Id),
                OrderItem.Create(Guid.NewGuid(), "MacBook Pro", 2499m, 2, order.Id)
            ];

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();

            orderItemRepositoryMock
                .Setup(i => i.GetAllOrderItemsByOrderId(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItems);

            var service = CreateService(orderItemRepositoryMock);

            IReadOnlyList<OrderItemDto> result = await service.GetAllOrderItemsByOrderIdAsync(order.Id);

            result.Should().HaveCount(2);
            result.Should().AllSatisfy(i => i.OrderId.Should().Be(order.Id));

            orderItemRepositoryMock.Verify(i => i.GetAllOrderItemsByOrderId(order.Id, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrderItemsByOrderIdAsync_ReturnsEmptyList_WhenNotExists()
        {
            Order order = Order.Create(Guid.NewGuid(), null);

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();

            orderItemRepositoryMock
                .Setup(i => i.GetAllOrderItemsByOrderId(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(orderItemRepositoryMock);

            IReadOnlyList<OrderItemDto> result = await service.GetAllOrderItemsByOrderIdAsync(order.Id);

            result.Should().BeEmpty();

            orderItemRepositoryMock.Verify(i => i.GetAllOrderItemsByOrderId(order.Id, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrderItemAsync_ReturnsOrderItemDto_WhenExists()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            OrderItem orderItem = OrderItem.Create(Guid.NewGuid(), "iPhone 16", 1299m, 1, order.Id);
            Guid orderItemId = orderItem.Id;

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItem);

            var service = CreateService(orderItemRepositoryMock);

            OrderItemDto? result = await service.GetOrderItemAsync(orderItemId, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Id.Should().Be(orderItemId);
            result.OrderId.Should().Be(order.Id);

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrderItemAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderItemId = Guid.NewGuid();

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderItem?)null);

            var service = CreateService(orderItemRepositoryMock);

            OrderItemDto? result = await service.GetOrderItemAsync(orderItemId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateOrderItemAsync_ReturnsOrderItemDto()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            CreateOrderItemDto createDto = new()
            {
                OrderId = order.Id,
                ProductId = Guid.NewGuid(),
                ProductName = "iPhone 16",
                UnitPrice = 1299m,
                Quantity = 2
            };

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();

            orderItemRepositoryMock
                .Setup(i => i.AddAsync(It.IsAny<OrderItem>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            orderItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderItemRepositoryMock);

            OrderItemDto result = await service.CreateOrderItemAsync(createDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result.OrderId.Should().Be(createDto.OrderId);
            result.ProductId.Should().Be(createDto.ProductId);
            result.Quantity.Should().Be(createDto.Quantity);

            orderItemRepositoryMock.Verify(i => i.AddAsync(It.IsAny<OrderItem>(), It.IsAny<CancellationToken>()), Times.Once);
            orderItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderItemQuantityAsync_ReturnsOrderItemDto_WhenExists()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            OrderItem orderItem = OrderItem.Create(Guid.NewGuid(), "iPhone 16", 1299m, 1, order.Id);
            Guid orderItemId = orderItem.Id;
            ChangeOrderItemQuantityDto changeDto = new() { Quantity = 5 };

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItem);

            orderItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderItemRepositoryMock);

            OrderItemDto? result = await service.ChangeOrderItemQuantityAsync(orderItemId, changeDto, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Quantity.Should().Be(changeDto.Quantity);

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
            orderItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderItemQuantityAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderItemId = Guid.NewGuid();
            ChangeOrderItemQuantityDto changeDto = new() { Quantity = 5 };

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderItem?)null);

            var service = CreateService(orderItemRepositoryMock);

            OrderItemDto? result = await service.ChangeOrderItemQuantityAsync(orderItemId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
            orderItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrderItemAsync_ReturnsTrue_WhenExists()
        {
            Order order = Order.Create(Guid.NewGuid(), null);
            OrderItem orderItem = OrderItem.Create(Guid.NewGuid(), "iPhone 16", 1299m, 1, order.Id);
            Guid orderItemId = orderItem.Id;

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItem);

            orderItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(orderItemRepositoryMock);

            bool result = await service.DeleteOrderItemAsync(orderItemId, It.IsAny<CancellationToken>());

            result.Should().BeTrue();

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
            orderItemRepositoryMock.Verify(i => i.Remove(orderItem), Times.Once);
            orderItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrderItemAsync_ReturnsFalse_WhenNotExists()
        {
            Guid orderItemId = Guid.NewGuid();

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderItem?)null);

            var service = CreateService(orderItemRepositoryMock);

            bool result = await service.DeleteOrderItemAsync(orderItemId, It.IsAny<CancellationToken>());

            result.Should().BeFalse();

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
            orderItemRepositoryMock.Verify(i => i.Remove(It.IsAny<OrderItem>()), Times.Never);
            orderItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}