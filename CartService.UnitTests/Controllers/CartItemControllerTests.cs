using System;
using System.Threading;
using System.Threading.Tasks;
using CartService.Api.Controllers;
using CartService.Application.Common;
using CartService.Application.DTOs.CartItem;
using CartService.Application.Interfaces.Services;
using CartService.Domain.Entity;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartService.UnitTests.Controllers
{
    public class CartItemControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCartItem_ReturnsOk_WhenExists()
        {
            Guid cartItemId = Guid.NewGuid();

            CartItemDto expectedDto = new() { Id = cartItemId, ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 1000 };

            Mock<ICartItemService> serviceMock = new();

            serviceMock
                .Setup(i => i.GetCartItemByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CartItemController controller = new(serviceMock.Object);


            var result = await controller.GetCartItem(cartItemId);

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            serviceMock.Verify(i => i.GetCartItemByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCartItem_ReturnsNotFound_WhenNotExists()
        {
            Guid cartItemId = Guid.NewGuid();

            Mock<ICartItemService> serviceMock = new();

            serviceMock
                .Setup(i => i.GetCartItemByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItemDto?)null);

            CartItemController controller = new(serviceMock.Object);


            var result = await controller.GetCartItem(cartItemId);

            result.Should().BeOfType<NotFoundResult>();

            serviceMock.Verify(i => i.GetCartItemByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemAsync_ReturnsCreatedAtAction_WhenSuccess()
        {
            CreateCartItemDto createDto = new() { CartId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 3 };

            CartItemDto createdDto = new()
            {
                Id = Guid.NewGuid(),
                CartId = createDto.CartId,
                ProductId = createDto.ProductId,
                Quantity = createDto.Quantity,
                UnitPrice = 1200
            };

            Result<CartItemDto, CartItemError> serviceResult = Result<CartItemDto, CartItemError>.Ok(createdDto);

            Mock<ICartItemService> serviceMock = new();

            serviceMock
                .Setup(i => i.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResult);

            CartItemController controller = new(serviceMock.Object);


            var result = await controller.CreateCartItem(createDto, It.IsAny<CancellationToken>());
            var created = result as CreatedAtActionResult;

            created!.ActionName.Should().Be(nameof(CartItemController.GetCartItem));
            created.RouteValues!["cartItemId"].Should().Be(createdDto.Id);
            created.Value.Should().Be(createdDto);

            serviceMock.Verify(i => i.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemAsync_ReturnsBadRequest_WhenFail()
        {
            CreateCartItemDto createDto = new() { CartId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 1 };

            Result<CartItemDto, CartItemError> serviceResult = Result<CartItemDto, CartItemError>.Fail(CartItemError.OutOfStock);

            Mock<ICartItemService> serviceMock = new();

            serviceMock
                .Setup(i => i.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResult);

            CartItemController controller = new(serviceMock.Object);


            var result = await controller.CreateCartItem(createDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<BadRequestObjectResult>();

            serviceMock.Verify(i => i.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantity_ReturnsOk_WhenSuccess()
        {
            Guid cartItemId = Guid.NewGuid();

            ChangeQuantityCartItemDto changeDto = new() { Quantity = 5 };

            CartItemDto updatedDto = new()
            {
                Id = cartItemId,
                ProductId = Guid.NewGuid(),
                Quantity = changeDto.Quantity,
                UnitPrice = 900
            };

            Result<CartItemDto, CartItemError> serviceResult = Result<CartItemDto, CartItemError>.Ok(updatedDto);

            Mock<ICartItemService> serviceMock = new();

            serviceMock
                .Setup(i => i.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResult);

            CartItemController controller = new(serviceMock.Object);


            var result = await controller.ChangeCartItemQuantity(cartItemId, changeDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(updatedDto);

            serviceMock.Verify(i => i.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantity_ReturnsBadRequest_WhenFail()
        {
            Guid cartItemId = Guid.NewGuid();

            ChangeQuantityCartItemDto changeDto = new() { Quantity = 1 };

            Result<CartItemDto, CartItemError> serviceResult = Result<CartItemDto, CartItemError>.Fail(CartItemError.ProductNotFound);

            Mock<ICartItemService> serviceMock = new();

            serviceMock
                .Setup(i => i.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResult);

            CartItemController controller = new(serviceMock.Object);


            var result = await controller.ChangeCartItemQuantity(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundObjectResult>();

            serviceMock.Verify(i => i.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCartItem_ReturnsOk_WhenExists()
        {
            Guid cartItemId = Guid.NewGuid();

            CartItemDto expectedDto = new() { Id = cartItemId, ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 11m };

            Mock<ICartItemService> serviceMock = new();

            serviceMock
                .Setup(i => i.DeleteCartItemAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CartItemController controller = new(serviceMock.Object);


            var result = await controller.DeleteCartItem(cartItemId);

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            serviceMock.Verify(i => i.DeleteCartItemAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCartItem_ReturnsNotFound_WhenNotExists()
        {
            Guid cartItemId = Guid.NewGuid();

            Mock<ICartItemService> serviceMock = new();

            serviceMock
                .Setup(i => i.DeleteCartItemAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItemDto?)null);

            CartItemController controller = new(serviceMock.Object);


            var result = await controller.DeleteCartItem(cartItemId);

            result.Should().BeOfType<NotFoundResult>();

            serviceMock.Verify(i => i.DeleteCartItemAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
