using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.Api.Controllers;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Application.Interfaces.Services;

namespace OrderService.UnitTests.Controllers
{
    public class OrderItemControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrderItems_ReturnsOrderItemDtoList_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            IReadOnlyList<OrderItemDto> expectedDto =
            [
                new() { Id = Guid.NewGuid(), OrderId = orderId, Quantity = 1 },
                new() { Id = Guid.NewGuid(), OrderId = orderId, Quantity = 2 }
            ];

            Mock<IOrderItemService> orderItemServiceMock = new();

            orderItemServiceMock
                .Setup(i => i.GetAllOrderItemsByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderItemController controller = new(orderItemServiceMock.Object);


            IReadOnlyList<OrderItemDto> result = await controller.GetAllOrderItems(orderId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderItemServiceMock.Verify(i => i.GetAllOrderItemsByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrderItems_ReturnsEmptyList_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();

            IReadOnlyList<OrderItemDto> expectedDto = [];

            Mock<IOrderItemService> orderItemServiceMock = new();

            orderItemServiceMock
                .Setup(i => i.GetAllOrderItemsByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderItemController controller = new(orderItemServiceMock.Object);


            IReadOnlyList<OrderItemDto> result = await controller.GetAllOrderItems(orderId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderItemServiceMock.Verify(i => i.GetAllOrderItemsByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrderItem_ReturnsOk_WhenExists()
        {
            Guid orderItemId = Guid.NewGuid();

            OrderItemDto expectedDto = new() { Id = orderItemId, OrderId = Guid.NewGuid(), Quantity = 3 };

            Mock<IOrderItemService> orderItemServiceMock = new();

            orderItemServiceMock
                .Setup(i => i.GetOrderItemAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderItemController controller = new(orderItemServiceMock.Object);


            var result = await controller.GetOrderItem(orderItemId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            orderItemServiceMock.Verify(i => i.GetOrderItemAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrderItem_ReturnsNotFound_WhenNotExists()
        {
            Guid orderItemId = Guid.NewGuid();

            Mock<IOrderItemService> orderItemServiceMock = new();

            orderItemServiceMock
                .Setup(i => i.GetOrderItemAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderItemDto?)null);

            OrderItemController controller = new(orderItemServiceMock.Object);


            var result = await controller.GetOrderItem(orderItemId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            orderItemServiceMock.Verify(i => i.GetOrderItemAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateOrderItem_ReturnsCreatedAtAction_WithOrderItemDto()
        {
            CreateOrderItemDto createDto = new() { OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2 };

            OrderItemDto expectedDto = new() { Id = Guid.NewGuid(), OrderId = createDto.OrderId, Quantity = createDto.Quantity };

            Mock<IOrderItemService> orderItemServiceMock = new();

            orderItemServiceMock
                .Setup(i => i.CreateOrderItemAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderItemController controller = new(orderItemServiceMock.Object);


            var result = await controller.CreateOrderItem(createDto, It.IsAny<CancellationToken>());
            var createdResult = result as CreatedAtActionResult;

            createdResult!.ActionName.Should().Be(nameof(OrderItemController.GetOrderItem));
            createdResult.RouteValues!["orderItemId"].Should().Be(expectedDto.Id);
            createdResult.Value.Should().BeEquivalentTo(expectedDto);

            orderItemServiceMock.Verify(i => i.CreateOrderItemAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderItemQuantity_ReturnsOk_WhenExists()
        {
            Guid orderItemId = Guid.NewGuid();

            ChangeOrderItemQuantityDto changeDto = new() { Quantity = 10 };
            OrderItemDto expectedDto = new() { Id = orderItemId, OrderId = Guid.NewGuid(), Quantity = changeDto.Quantity };

            Mock<IOrderItemService> orderItemServiceMock = new();

            orderItemServiceMock
                .Setup(i => i.ChangeOrderItemQuantityAsync(orderItemId, changeDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderItemController controller = new(orderItemServiceMock.Object);


            var result = await controller.ChangeOrderItemQuantity(orderItemId, changeDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            orderItemServiceMock.Verify(i => i.ChangeOrderItemQuantityAsync(orderItemId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderItemQuantity_ReturnsNotFound_WhenNotExists()
        {
            Guid orderItemId = Guid.NewGuid();

            ChangeOrderItemQuantityDto changeDto = new() { Quantity = 10 };

            Mock<IOrderItemService> orderItemServiceMock = new();

            orderItemServiceMock
                .Setup(i => i.ChangeOrderItemQuantityAsync(orderItemId, changeDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderItemDto?)null);

            OrderItemController controller = new(orderItemServiceMock.Object);


            var result = await controller.ChangeOrderItemQuantity(orderItemId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            orderItemServiceMock.Verify(i => i.ChangeOrderItemQuantityAsync(orderItemId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrderItem_ReturnsOk_WhenExists()
        {
            Guid orderItemId = Guid.NewGuid();

            OrderItemDto expectedDto = new() { Id = orderItemId, OrderId = Guid.NewGuid(), Quantity = 1 };

            Mock<IOrderItemService> orderItemServiceMock = new();

            orderItemServiceMock
                .Setup(i => i.DeleteOrderItemAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderItemController controller = new(orderItemServiceMock.Object);


            var result = await controller.DeleteOrderItem(orderItemId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            orderItemServiceMock.Verify(i => i.DeleteOrderItemAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrderItem_ReturnsNotFound_WhenNotExists()
        {
            Guid orderItemId = Guid.NewGuid();

            Mock<IOrderItemService> orderItemServiceMock = new();

            orderItemServiceMock
                .Setup(i => i .DeleteOrderItemAsync(orderItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderItemDto?)null);

            OrderItemController controller = new(orderItemServiceMock.Object);


            var result = await controller.DeleteOrderItem(orderItemId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            orderItemServiceMock.Verify(i => i.DeleteOrderItemAsync(orderItemId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
