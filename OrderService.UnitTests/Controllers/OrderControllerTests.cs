using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.Api.Controllers;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Enums;

namespace OrderService.UnitTests.Controllers
{
    public class OrderControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrders_ReturnsOrderDtoList_WhenExists()
        {
            IReadOnlyList<OrderDto> expectedDto =
            [
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            ];

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.GetAllOrdersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderController controller = new(orderServiceMock.Object);


            IReadOnlyList<OrderDto> result = await controller.GetAllOrders(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderServiceMock.Verify(o => o.GetAllOrdersAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersByUserId_ReturnsOk_WhenValidUserId()
        {
            Guid userId = Guid.NewGuid();

            IReadOnlyList<OrderDto> expectedDto =
            [
                new() { Id = Guid.NewGuid(), UserId = userId },
                new() { Id = Guid.NewGuid(), UserId = userId }
            ];

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.GetAllOrdersByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderController controller = new(orderServiceMock.Object)
            {
                ControllerContext = new()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                            [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
                            "mock"))
                    }
                }
            };


            var result = await controller.GetAllOrdersByUserId(It.IsAny<CancellationToken>());
            var okResult = result as OkObjectResult;

            okResult!.Value.Should().BeEquivalentTo(expectedDto);

            orderServiceMock.Verify(o => o.GetAllOrdersByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersByUserId_ReturnsUnauthorized_WhenInvalidUserId()
        {
            Mock<IOrderService> orderServiceMock = new();

            OrderController controller = new(orderServiceMock.Object)
            {
                ControllerContext = new()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                            [new Claim(ClaimTypes.NameIdentifier, "invalid-guid")],
                            "mock"))
                    }
                }
            };


            var result = await controller.GetAllOrdersByUserId(It.IsAny<CancellationToken>());

            result.Should().BeOfType<UnauthorizedResult>();

            orderServiceMock.Verify(o => o.GetAllOrdersByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrder_ReturnsOk_WhenExists()
        {
            Guid orderId = Guid.NewGuid();
            OrderExtendedDto expectedDto = new() { Id = orderId };

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.GetOrder(orderId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            orderServiceMock.Verify(o => o.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrder_ReturnsNotFound_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderExtendedDto?)null);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.GetOrder(orderId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            orderServiceMock.Verify(o => o.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateOrder_ReturnsCreatedAtAction_WithOrderDto()
        {
            CreateOrderDto createDto = new() { UserId = Guid.NewGuid() };

            OrderDto expectedDto = new() { Id = Guid.NewGuid(), UserId = createDto.UserId };

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.CreateOrderAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.CreateOrder(createDto, It.IsAny<CancellationToken>());
            var createdResult = result as CreatedAtActionResult;

            createdResult!.ActionName.Should().Be(nameof(OrderController.CreateOrder));
            createdResult.RouteValues!["orderId"].Should().Be(expectedDto.Id);
            createdResult.Value.Should().Be(expectedDto);

            orderServiceMock.Verify(o => o.CreateOrderAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateOrder_ReturnsOk_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            UpdateOrderNoteDto updateDto = new() { Note = "new note" };
            OrderDto expectedDto = new() { Id = orderId, Note = updateDto.Note };

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.UpdateOrderNoteAsync(orderId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.UpdateOrder(orderId, updateDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            orderServiceMock.Verify(o => o.UpdateOrderNoteAsync(orderId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateOrder_ReturnsNotFound_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();
            UpdateOrderNoteDto updateDto = new() { Note = "new note" };

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.UpdateOrderNoteAsync(orderId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderDto?)null);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.UpdateOrder(orderId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            orderServiceMock.Verify(o => o.UpdateOrderNoteAsync(orderId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrder_ReturnsOk_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            OrderDto expectedDto = new() { Id = orderId };

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.DeleteOrderAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.DeleteOrder(orderId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            orderServiceMock.Verify(o => o.DeleteOrderAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOrder_ReturnsNotFound_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.DeleteOrderAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderDto?)null);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.DeleteOrder(orderId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            orderServiceMock.Verify(o => o.DeleteOrderAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatus_ReturnsOk_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            ChangeOrderStatusDto changeDto = new() { Status = OrderService.Domain.Enums.OrderStatus.Accepted };
            OrderDto expectedDto = new() { Id = orderId, Status = changeDto.Status };

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.ChangeOrderStatus(orderId, changeDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            orderServiceMock.Verify(o => o.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeOrderStatus_ReturnsNotFound_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();
            ChangeOrderStatusDto changeDto = new() { Status = OrderService.Domain.Enums.OrderStatus.Accepted };

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderDto?)null);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.ChangeOrderStatus(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            orderServiceMock.Verify(o => o.ChangeOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeInternalOrderStatus_ReturnsOk_WhenExists()
        {
            Guid orderId = Guid.NewGuid();

            ChangeInternalOrderStatusDto changeDto = new() { InternalStatus = InternalOrderStatus.DeliveryFaild };
            OrderDto expectedDto = new() { Id = orderId };

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.ChangeInternalOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.ChangeInternalOrderStatus(orderId, changeDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            orderServiceMock.Verify(o => o.ChangeInternalOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeInternalOrderStatus_ReturnsNotFound_WhenNotExists()
        {
            Guid orderId = Guid.NewGuid();
            ChangeInternalOrderStatusDto changeDto = new() { InternalStatus = OrderService.Domain.Enums.InternalOrderStatus.DeliveryFaild };

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.ChangeInternalOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderDto?)null);

            OrderController controller = new(orderServiceMock.Object);


            var result = await controller.ChangeInternalOrderStatus(orderId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            orderServiceMock.Verify(o => o.ChangeInternalOrderStatusAsync(orderId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllOrdersWithDeliveryFaildInternalStatus_ReturnsOrderDtoList()
        {
            IReadOnlyList<OrderDto> expectedDto =
            [
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            ];

            Mock<IOrderService> orderServiceMock = new();

            orderServiceMock
                .Setup(o => o.GetAllOrdersWithDeliveryFaildInternalStatusAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            OrderController controller = new(orderServiceMock.Object);


            IReadOnlyList<OrderDto> result = await controller.GetAllOrdersWithDeliveryFaildInternalStatus(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            orderServiceMock.Verify(o => o.GetAllOrdersWithDeliveryFaildInternalStatusAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
