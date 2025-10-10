using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CartService.Api.Controllers;
using CartService.Application.Common;
using CartService.Application.DTOs.Cart;
using CartService.Application.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartService.UnitTests.Controllers
{
    public class CartControllerTests
    {
        private static CartController CreateControllerWithUser(Mock<ICartService> cartServiceMock, Guid? userId)
        {
            CartController controller = new(cartServiceMock.Object);

            ClaimsIdentity identity = userId is not null
                ? new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())], "mock")
                : new ClaimsIdentity();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            return controller;
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrCreateCart_ReturnsUnauthorized_WhenUserIdMissingOrInvalid()
        {
            Mock<ICartService> cartServiceMock = new();

            CartController controller = CreateControllerWithUser(cartServiceMock, null);


            var result = await controller.GetOrCreateCart(It.IsAny<CancellationToken>());

            result.Should().BeOfType<UnauthorizedResult>();

            cartServiceMock.Verify(c => c.GetOrCreateCartByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrCreateCart_ReturnsOk_WithCartExtendedDto()
        {
            Guid userId = Guid.NewGuid();

            CartExtendedDto expectedDto = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };

            Mock<ICartService> cartServiceMock = new();

            cartServiceMock
                .Setup(c => c.GetOrCreateCartByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CartController controller = CreateControllerWithUser(cartServiceMock, userId);


            var result = await controller.GetOrCreateCart(It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            cartServiceMock.Verify(c => c.GetOrCreateCartByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCart_ReturnsUnauthorized_WhenUserIdMissingOrInvalid()
        {
            Mock<ICartService> cartServiceMock = new();

            CartController controller = CreateControllerWithUser(cartServiceMock, null);


            var result = await controller.DeleteCart(It.IsAny<CancellationToken>());

            result.Should().BeOfType<UnauthorizedResult>();

            cartServiceMock.Verify(c => c.DeleteCartByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCart_ReturnsOk_WhenCartExists()
        {
            Guid userId = Guid.NewGuid();

            CartDto expectedDto = new() { Id = Guid.NewGuid(), UserId = userId };

            Mock<ICartService> cartServiceMock = new();

            cartServiceMock
                .Setup(c => c.DeleteCartByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CartController controller = CreateControllerWithUser(cartServiceMock, userId);


            var result = await controller.DeleteCart(It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            cartServiceMock.Verify(c => c.DeleteCartByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCart_ReturnsNotFound_WhenCartNotExists()
        {
            Guid userId = Guid.NewGuid();

            Mock<ICartService> cartServiceMock = new();

            cartServiceMock
                .Setup(c => c.DeleteCartByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartDto?)null);

            CartController controller = CreateControllerWithUser(cartServiceMock, userId);


            var result = await controller.DeleteCart(It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            cartServiceMock.Verify(c => c.DeleteCartByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckoutCart_ReturnsUnauthorized_WhenUserIdMissingOrInvalid()
        {
            Mock<ICartService> cartServiceMock = new();

            CartController controller = CreateControllerWithUser(cartServiceMock, null);


            var result = await controller.CheckoutCart(new(), It.IsAny<CancellationToken>());

            result.Should().BeOfType<UnauthorizedResult>();

            cartServiceMock.Verify(c => c.CheckoutCartByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CartCheckoutRequestDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckoutCart_ReturnsOk_WhenServiceReturnsSuccess()
        {
            Guid userId = Guid.NewGuid();

            CheckoutResult okResult = new(true, [],"www.url.com");
            Result<CheckoutResult, CartError> serviceResult = Result<CheckoutResult, CartError>.Ok(okResult);

            Mock<ICartService> cartServiceMock = new();

            cartServiceMock
                .Setup(c => c.CheckoutCartByUserIdAsync(userId, It.IsAny<CartCheckoutRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResult);

            CartController controller = CreateControllerWithUser(cartServiceMock, userId);


            var result = await controller.CheckoutCart(new(), It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(okResult);

            cartServiceMock.Verify(c => c.CheckoutCartByUserIdAsync(userId, It.IsAny<CartCheckoutRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckoutCart_ReturnsBadRequest_WhenServiceReturnsFail()
        {
            Guid userId = Guid.NewGuid();

            Result<CheckoutResult, CartError> serviceResult = Result<CheckoutResult, CartError>.Fail(CartError.DeliveryNotCreated);

            Mock<ICartService> cartServiceMock = new();

            cartServiceMock
                .Setup(c => c.CheckoutCartByUserIdAsync(userId, It.IsAny<CartCheckoutRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResult);

            CartController controller = CreateControllerWithUser(cartServiceMock, userId);


            var result = await controller.CheckoutCart(new(), It.IsAny<CancellationToken>());

            result.Should().BeOfType<BadRequestObjectResult>();

            cartServiceMock.Verify(c => c.CheckoutCartByUserIdAsync(userId, It.IsAny<CartCheckoutRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
