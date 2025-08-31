using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Services;
using OrderService.Domain.Entity;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace OrderService.UnitTests.Services
{
    public class OrderItemServiceTests
    {

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrderItemsByOrderIdAsync_ReturnOrderItemDtoList_WhenExists()
        {
            Order order = new() { Id = Guid.NewGuid() };

            List<OrderItem> orderItems =
            [
                new() {Id = Guid.NewGuid(), Order = order },
                new() {Id = Guid.NewGuid(), Order = order }
            ];

            List<OrderItemDto> expectedDto =
            [
                new() {Id = orderItems[0].Id, OrderId = order.Id},
                new() {Id = orderItems[1].Id, OrderId = order.Id},
            ];

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderItemRepositoryMock
                .Setup(i => i.GetAllOrderItemsByOrderId(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItems);

            mapperMock
                .Setup(m => m.Map<List<OrderItemDto>>(orderItems))
                .Returns(expectedDto);

            OrderItemService service = new(
                orderItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderItemService>>().Object
            );


            IReadOnlyList<OrderItemDto> result = await service.GetAllOrderItemsByOrderIdAsync(order.Id);

            result.Should().BeEquivalentTo(expectedDto);

            orderItemRepositoryMock.Verify(i => i.GetAllOrderItemsByOrderId(order.Id, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<OrderItemDto>>(orderItems), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrderItemsByOrderIdAsync_ReturnsEmptyList_WhenNotExists()
        {
            Order order = new() { Id = Guid.NewGuid() };

            List<OrderItem> orderItems = [];
            List<OrderItemDto> expectedDto = [];

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderItemRepositoryMock
                .Setup(i => i.GetAllOrderItemsByOrderId(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItems);

            mapperMock
                .Setup(m => m.Map<List<OrderItemDto>>(orderItems))
                .Returns(expectedDto);

            OrderItemService service = new(
                orderItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderItemService>>().Object
            );


            IReadOnlyList<OrderItemDto> result = await service.GetAllOrderItemsByOrderIdAsync(order.Id);

            result.Should().BeEquivalentTo(expectedDto);

            orderItemRepositoryMock.Verify(i => i.GetAllOrderItemsByOrderId(order.Id, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<OrderItemDto>>(orderItems), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrderItemAsync_ReturnsOrderItemDto_WhenExists()
        {
            Guid orderItemId = Guid.NewGuid();
            Order order = new() { Id = Guid.NewGuid() };

            OrderItem orderItem = new() { Id = orderItemId, Order = order };
            OrderItemDto expectedDto = new() { Id = orderItemId, OrderId = order.Id };

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItem);

            mapperMock
                .Setup(m => m.Map<OrderItemDto>(orderItem))
                .Returns(expectedDto);

            OrderItemService service = new(
                orderItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderItemService>>().Object
            );


            OrderItemDto? result = await service.GetOrderItemAsync(orderItemId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderItemDto>(orderItem), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrderItemAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderItemId = Guid.NewGuid();

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderItem?)null);

            OrderItemService service = new(
                orderItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderItemService>>().Object
            );


            OrderItemDto? result = await service.GetOrderItemAsync(orderItemId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderItemDto>(It.IsAny<OrderItem>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateOrderItemAsync_ReturnsOrderItemDto()
        {
            CreateOrderItemDto createDto = new() { OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2 };

            Order order = new() { Id = createDto.OrderId };
            OrderItem orderItem = new() { Id = Guid.Empty, Order = order, Quantity = createDto.Quantity, CreatedAt = DateTime.UtcNow };
            OrderItem createdOrderItem = new() { Id = Guid.NewGuid(), Order = order, Quantity = createDto.Quantity, CreatedAt = orderItem.CreatedAt };
            OrderItemDto expectedDto = new() { Id = createdOrderItem.Id, OrderId = order.Id, Quantity = createdOrderItem.Quantity, CreatedAt = createdOrderItem.CreatedAt };

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<OrderItem>(createDto))
                .Returns(orderItem);

            orderItemRepositoryMock
                .Setup(i => i.InsertAsync(It.IsAny<OrderItem>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdOrderItem);

            mapperMock
                .Setup(m => m.Map<OrderItemDto>(createdOrderItem))
                .Returns(expectedDto);

            OrderItemService service = new(
                orderItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderItemService>>().Object
            );


            OrderItemDto result = await service.CreateOrderItemAsync(createDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            mapperMock.Verify(m => m.Map<OrderItem>(createDto), Times.Once);
            orderItemRepositoryMock.Verify(i => i.InsertAsync(It.IsAny<OrderItem>(), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderItemDto>(createdOrderItem), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderItemQuantityAsync_ReturnsOrderItemDto_WhenExists()
        {
            Guid orderItemId = Guid.NewGuid();
            ChangeOrderItemQuantityDto changeDto = new() { Quantity = 5 };

            Order order = new() { Id = Guid.NewGuid() };
            OrderItem orderItem = new() { Id = orderItemId, Order = order, Quantity = 1, UpdatedAt = DateTime.UtcNow };
            OrderItemDto expectedDto = new() { Id = orderItemId, OrderId = order.Id, Quantity = changeDto.Quantity, UpdatedAt = orderItem.UpdatedAt };

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItem);

            orderItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<OrderItemDto>(orderItem))
                .Returns(expectedDto);

            OrderItemService service = new(
                orderItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderItemService>>().Object
            );


            OrderItemDto? result = await service.ChangeOrderItemQuantityAsync(orderItemId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
            orderItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderItemDto>(orderItem), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderItemQuantityAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderItemId = Guid.NewGuid();
            ChangeOrderItemQuantityDto changeDto = new() { Quantity = 5 };

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderItem?)null);

            OrderItemService service = new(
                orderItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderItemService>>().Object
            );


            OrderItemDto? result = await service.ChangeOrderItemQuantityAsync(orderItemId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
            orderItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<OrderItemDto>(It.IsAny<OrderItem>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrderItemAsync_ReturnsOrderItemDto_WhenExists()
        {
            Guid orderItemId = Guid.NewGuid();

            Order order = new() { Id = Guid.NewGuid() };
            OrderItem orderItem = new() { Id = orderItemId, Order = order };
            OrderItemDto expectedDto = new() { Id = orderItemId, OrderId = order.Id };

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderItem);

            mapperMock
                .Setup(m => m.Map<OrderItemDto>(orderItem))
                .Returns(expectedDto);

            orderItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            OrderItemService service = new(
                orderItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderItemService>>().Object
            );


            OrderItemDto? result = await service.DeleteOrderItemAsync(orderItemId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderItemDto>(orderItem), Times.Once);
            orderItemRepositoryMock.Verify(i => i.Remove(orderItem), Times.Once);
            orderItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrderItemAsync_ReturnsNull_WhenNotExists()
        {
            Guid orderItemId = Guid.NewGuid();

            Mock<IOrderItemRepository> orderItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            orderItemRepositoryMock
                .Setup(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderItem?)null);

            OrderItemService service = new(
                orderItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<OrderItemService>>().Object
            );


            OrderItemDto? result = await service.DeleteOrderItemAsync(orderItemId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            orderItemRepositoryMock.Verify(i => i.FindByIdAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<OrderItemDto>(It.IsAny<OrderItem>()), Times.Never);
            orderItemRepositoryMock.Verify(i => i.Remove(It.IsAny<OrderItem>()), Times.Never);
            orderItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



    }
}
