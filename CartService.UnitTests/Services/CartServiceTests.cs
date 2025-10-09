using AutoMapper;
using CartService.Application.Common;
using CartService.Application.DTOs.Cart;
using CartService.Application.DTOs.CartItem;
using CartService.Application.DTOs.External;
using CartService.Application.Interfaces.External;
using CartService.Application.Interfaces.Repositories;
using CartService.Domain.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using CartServiceService = CartService.Application.Services.CartService;

namespace CartService.UnitTests.Services
{
    public class CartServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrCreateCartByUserIdAsync_ReturnsExistingCart_WhenExists()
        {
            Guid userId = Guid.NewGuid();

            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };
            CartExtendedDto expectedDto = new() { Id = cart.Id, UserId = userId, Items = [] };

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            mapperMock
                .Setup(m => m.Map<CartExtendedDto>(cart))
                .Returns(expectedDto);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.GetOrCreateCartByUserIdAsync(userId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            cartRepositoryMock.Verify(c => c.InsertAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CartExtendedDto>(cart), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetOrCreateCartByUserIdAsync_CreatesAndReturnsCart_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            Cart createdCart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };
            CartExtendedDto expectedDto = new() { Id = createdCart.Id, UserId = userId, Items = [] };

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null);

            cartRepositoryMock
                .Setup(c => c.InsertAsync(It.Is<Cart>(x =>
                        x.Id == Guid.Empty &&
                        x.UserId == userId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdCart);

            mapperMock
                .Setup(m => m.Map<CartExtendedDto>(createdCart))
                .Returns(expectedDto);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.GetOrCreateCartByUserIdAsync(userId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            cartRepositoryMock.Verify(c => c.InsertAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartExtendedDto>(createdCart), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCartByUserIdAsync_ReturnsCartDto_WhenExists()
        {
            Guid userId = Guid.NewGuid();

            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };
            CartDto expectedDto = new() { Id = cart.Id, UserId = userId };

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            cartRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.DeleteCartByUserIdAsync(userId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            cartRepositoryMock.Verify(c => c.Remove(cart), Times.Once);
            cartRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCartByUserIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.DeleteCartByUserIdAsync(userId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            cartRepositoryMock.Verify(c => c.Remove(It.IsAny<Cart>()), Times.Never);
            cartRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckoutCartByUserIdAsync_ReturnsFail_CartNotFound_WhenMissing()
        {
            Guid userId = Guid.NewGuid();

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.CheckoutCartByUserIdAsync(userId, new CartCheckoutRequestDto(), It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartError.CartNotFound);

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()), Times.Never);
            orderClientMock.Verify(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Never);
            cartRepositoryMock.Verify(c => c.Remove(It.IsAny<Cart>()), Times.Never);
            cartRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckoutCartByUserIdAsync_ReturnsFail_CartNotFound_WhenEmpty()
        {
            Guid userId = Guid.NewGuid();

            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.CheckoutCartByUserIdAsync(userId, new CartCheckoutRequestDto(), It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartError.CartNotFound);

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()), Times.Never);
            orderClientMock.Verify(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Never);
            cartRepositoryMock.Verify(c => c.Remove(It.IsAny<Cart>()), Times.Never);
            cartRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckoutCartByUserIdAsync_ReturnsOkFalse_WhenSomeProductsUnavailable()
        {
            Guid userId = Guid.NewGuid();

            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };

            List<CartItem> items =
            [
                new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 10m, Cart = cart },
                new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 20m, Cart = cart }
            ];

            cart.Items = items;

            List<ProductQuantityCheckRequestDto> request =
            [
                new() {Id = items[0].ProductId, Quantity = items[0].Quantity },
                new() {Id = items[1].ProductId, Quantity = items[1].Quantity }
            ];

            List<ProductQuantityCheckResponseDto> bad =
            [
                new() { Id = items[0].ProductId }
            ];

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            productClientMock
                .Setup(p => p.CheckProductAvailabilityAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bad);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.CheckoutCartByUserIdAsync(userId, new CartCheckoutRequestDto(), It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeTrue();
            result.Value.Success.Should().BeFalse();
            result.Value.BadProducts.Should().BeEquivalentTo(bad);

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()), Times.Once);
            orderClientMock.Verify(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Never);
            cartRepositoryMock.Verify(c => c.Remove(It.IsAny<Cart>()), Times.Never);
            cartRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckoutCartByUserIdAsync_ReturnsOkTrue_WhenEverythingOk()
        {
            Guid userId = Guid.NewGuid();
            Guid courierId = Guid.NewGuid();

            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };

            List<CartItem> items =
            [
                new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 10m, Cart = cart },
                new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 25m, Cart = cart }
            ];

            cart.Items = items;

            List<ProductQuantityCheckResponseDto> ok = [];

            List<CartItemForCheckoutDto> mappedItems =
            [
                new() { ProductId = items[0].ProductId, Quantity = items[0].Quantity },
                new() { ProductId = items[1].ProductId, Quantity = items[1].Quantity }
            ];

            decimal expectedTotal = items.Sum(i => i.UnitPrice * i.Quantity);

            CartCheckoutRequestDto checkoutReq = new()
            {
                CourierId = courierId,
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+420123456789",
                Street = "Main Street 42",
                City = "Prague",
                PostalCode = "11000",
                State = "CZ",
                Note = "Please deliver between 9–11 AM"
            };

            CreateOrderAndDeliveryDto expectedRequest = new()
            {
                UserId = userId,
                CourierId = courierId,
                TotalPrice = expectedTotal,
                Note = checkoutReq.Note,
                Email = checkoutReq.Email,
                FirstName = checkoutReq.FirstName,
                LastName = checkoutReq.LastName,
                PhoneNumber = checkoutReq.PhoneNumber,
                Street = checkoutReq.Street,
                City = checkoutReq.City,
                PostalCode = checkoutReq.PostalCode,
                State = checkoutReq.State,
                Items = mappedItems
            };

            CreateOrderFromCartResponseDto orderResponse = new()
            {
                OrderId = Guid.NewGuid(),
                DeliveryId = Guid.NewGuid(),
                CheckoutUrl = "www.url.com"
            };

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            productClientMock
                .Setup(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ok);

            orderClientMock
                .Setup(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderResponse);

            cartRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.CheckoutCartByUserIdAsync(userId, checkoutReq, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeTrue();
            result.Value.Success.Should().BeTrue();
            result.Value.BadProducts.Should().BeEmpty();

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Exactly(2));
            productClientMock.Verify(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()), Times.Once);
            orderClientMock.Verify(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Once);
            cartRepositoryMock.Verify(c => c.Remove(cart), Times.Once);
            cartRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckoutCartByUserIdAsync_ReturnsFail_DeliveryNotCreated_WhenOrderDeliveryIdNull()
        {
            Guid userId = Guid.NewGuid();

            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };

            List<CartItem> items =
            [
                new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 3, UnitPrice = 15m, Cart = cart }
            ];

            cart.Items = items;

            IReadOnlyList<ProductQuantityCheckResponseDto> ok = [];

            List<CartItemForCheckoutDto> mappedItems =
            [
                new() { ProductId = items[0].ProductId, Quantity = items[0].Quantity }
            ];

            decimal expectedTotal = items.Sum(i => i.UnitPrice * i.Quantity);

            CartCheckoutRequestDto checkoutReq = new()
            {
                CourierId = Guid.Empty,
                Email = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                PhoneNumber = string.Empty,
                Street = string.Empty,
                City = string.Empty,
                PostalCode = string.Empty,
                State = string.Empty,
                Note = string.Empty
            };

            CreateOrderAndDeliveryDto expectedRequest = new()
            {
                UserId = userId,
                CourierId = checkoutReq.CourierId,
                TotalPrice = expectedTotal,
                Note = checkoutReq.Note,
                Email = checkoutReq.Email,
                FirstName = checkoutReq.FirstName,
                LastName = checkoutReq.LastName,
                PhoneNumber = checkoutReq.PhoneNumber,
                Street = checkoutReq.Street,
                City = checkoutReq.City,
                PostalCode = checkoutReq.PostalCode,
                State = checkoutReq.State,
                Items = mappedItems
            };

            CreateOrderFromCartResponseDto orderResponse = new()
            {
                OrderId = Guid.NewGuid(),
                DeliveryId = null,
                CheckoutUrl = "www.url.com"
            };

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            productClientMock
                .Setup(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ok);

            orderClientMock
                .Setup(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderResponse);

            cartRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.CheckoutCartByUserIdAsync(userId, checkoutReq, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartError.DeliveryNotCreated);

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Exactly(2));
            productClientMock.Verify(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()), Times.Once);
            orderClientMock.Verify(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Once);
            cartRepositoryMock.Verify(c => c.Remove(cart), Times.Once);
            cartRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckoutCartByUserIdAsync_ReturnsFail_DeliveryAndPaymentCheckoutNotCreated_WhenBothNull()
        {
            Guid userId = Guid.NewGuid();

            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };

            List<CartItem> items =
            [
                new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 1200, Cart = cart }
            ];

            cart.Items = items;

            CreateOrderFromCartResponseDto orderResponse = new()
            {
                OrderId = Guid.NewGuid(),
                DeliveryId = null,
                CheckoutUrl = null
            };

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            productClientMock
                .Setup(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            orderClientMock
                .Setup(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderResponse);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.CheckoutCartByUserIdAsync(userId, new CartCheckoutRequestDto(), It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartError.DeliveryAndPaymentCheckoutNotCreated);

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Exactly(2));
            productClientMock.Verify(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()), Times.Once);
            orderClientMock.Verify(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Once);
            cartRepositoryMock.Verify(c => c.Remove(cart), Times.Once);
            cartRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckoutCartByUserIdAsync_ReturnsFail_PaymentCheckoutUrlNotCreated_WhenCheckoutUrlMissing()
        {
            Guid userId = Guid.NewGuid();

            Cart cart = new() { Id = Guid.NewGuid(), UserId = userId, Items = [] };
            List<CartItem> items =
            [
                new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 1299, Cart = cart }
            ];

            cart.Items = items;

            CreateOrderFromCartResponseDto orderResponse = new()
            {
                OrderId = Guid.NewGuid(),
                DeliveryId = Guid.NewGuid(),
                CheckoutUrl = null
            };

            Mock<ICartRepository> cartRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IOrderReadClient> orderClientMock = new();

            cartRepositoryMock
                .Setup(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            productClientMock
                .Setup(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            orderClientMock
                .Setup(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderResponse);

            CartServiceService service = new(
                cartRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartServiceService>>().Object,
                productClientMock.Object,
                orderClientMock.Object
            );


            var result = await service.CheckoutCartByUserIdAsync(userId, new CartCheckoutRequestDto(), It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartError.PaymentCheckoutUrlNotCreated);

            cartRepositoryMock.Verify(c => c.FindCartByUserIdIncludeItemsAsync(userId, It.IsAny<CancellationToken>()), Times.Exactly(2));
            productClientMock.Verify(p => p.CheckProductAvailabilityAsync(It.IsAny<List<ProductQuantityCheckRequestDto>>(), It.IsAny<CancellationToken>()), Times.Once);
            orderClientMock.Verify(o => o.CreateOrderAndDeliveryAsync(It.IsAny<CreateOrderAndDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Once);
            cartRepositoryMock.Verify(c => c.Remove(cart), Times.Once);
            cartRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
