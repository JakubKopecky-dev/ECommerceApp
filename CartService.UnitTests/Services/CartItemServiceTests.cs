using CartService.Application.Common;
using CartService.Application.DTOs.CartItem;
using CartService.Application.DTOs.External;
using CartService.Application.Interfaces.External;
using CartService.Application.Interfaces.Repositories;
using CartService.Application.Services;
using CartService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CartService.UnitTests.Services
{
    public class CartItemServiceTests
    {
        private static CartItemService CreateService(
            Mock<ICartItemRepository> cartItemRepositoryMock,
            Mock<IProductReadClient> productClientMock)
        {
            return new CartItemService(
                cartItemRepositoryMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCartItemAsync_ReturnsCartItemDto_WhenExists()
        {
            CartItem cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), "iPhone 16", 1299, 2);
            Guid cartItemId = cartItem.Id;

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.GetCartItemByIdAsync(cartItemId, It.IsAny<CancellationToken>());

            result.Should().NotBeNull();
            result!.Id.Should().Be(cartItemId);
            result.CartId.Should().Be(cartItem.CartId);
            result.ProductId.Should().Be(cartItem.ProductId);
            result.Quantity.Should().Be(cartItem.Quantity);

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCartItemAsync_ReturnsNull_WhenNotExists()
        {
            Guid cartItemId = Guid.NewGuid();

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.GetCartItemByIdAsync(cartItemId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_UpdatesQuantity_WhenAlreadyExists_AndStockSufficient()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 3 };

            CartItem existingCartItem = CartItem.Create(cartId, productId, "iPhone 16", 1299, 2);
            ProductDto productDto = new() { Title = "iPhone 16", Price = 1299, QuantityInStock = 10 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            cartItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Quantity.Should().Be(5); // 2 + 3

            cartItemRepositoryMock.Verify(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_ReturnsFail_ProductNotFound_WhenAlreadyExists()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 1 };
            CartItem existingCartItem = CartItem.Create(cartId, productId, "iPhone 16", 1299, 2);

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.ProductNotFound);

            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            cartItemRepositoryMock.Verify(i => i.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_ReturnsFail_OutOfStock_WhenAlreadyExists()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 5 };
            CartItem existing = CartItem.Create(cartId, productId, "iPhone 16", 1200, 8);
            ProductDto productDto = new() { Title = "iPhone 16", Price = 1200, QuantityInStock = 12 }; // 8+5=13 > 12

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.OutOfStock);

            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            cartItemRepositoryMock.Verify(i => i.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_CreatesNew_WhenNotExists_AndStockSufficient()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 2 };
            ProductDto productDto = new() { Title = "iPhone", Price = 1200, QuantityInStock = 10 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            cartItemRepositoryMock
                .Setup(i => i.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            cartItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.CartId.Should().Be(cartId);
            result.Value.ProductId.Should().Be(productId);
            result.Value.Quantity.Should().Be(createDto.Quantity);
            result.Value.ProductName.Should().Be(productDto.Title);
            result.Value.UnitPrice.Should().Be(productDto.Price);

            cartItemRepositoryMock.Verify(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_ReturnsFail_ProductNotFound_WhenNotExists()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 2 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.ProductNotFound);

            cartItemRepositoryMock.Verify(i => i.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_ReturnsFail_OutOfStock_WhenNotExists()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 7 };
            ProductDto productDto = new() { Title = "iPhone 15", Price = 999, QuantityInStock = 5 }; // 7 > 5

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.OutOfStock);

            cartItemRepositoryMock.Verify(i => i.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantityAsync_ReturnsFail_WhenCartItemNotFound()
        {
            Guid cartItemId = Guid.NewGuid();
            ChangeQuantityCartItemDto changeDto = new() { Quantity = 3 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.ProductNotFound);

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            productClientMock.Verify(c => c.GetProductAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantityAsync_ReturnsNoContent_WhenQuantityZero()
        {
            ChangeQuantityCartItemDto changeDto = new() { Quantity = 0 };

            CartItem cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), "iPhone 16", 1299, 4);
            Guid cartItemId = cartItem.Id;

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            cartItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeNull(); // NoContent – žádná hodnota

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.Remove(cartItem), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantityAsync_ReturnsFail_ProductNotFound_WhenProductMissing()
        {
            ChangeQuantityCartItemDto changeDto = new() { Quantity = 7 };

            CartItem cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), "iPhone 16", 1299, 2);
            Guid cartItemId = cartItem.Id;

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(cartItem.ProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.ProductNotFound);

            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantityAsync_ReturnsFail_OutOfStock_WhenInsufficient()
        {
            ChangeQuantityCartItemDto changeDto = new() { Quantity = 10 };

            Guid productId = Guid.NewGuid();
            CartItem cartItem = CartItem.Create(Guid.NewGuid(), productId, "iPhone 16", 1200, 2);
            Guid cartItemId = cartItem.Id;
            ProductDto productDto = new() { Title = "iPhone 16", Price = 1200, QuantityInStock = 5 }; // 10 > 5

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.OutOfStock);

            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantityAsync_ReturnsOk_WhenStockSufficient()
        {
            ChangeQuantityCartItemDto changeDto = new() { Quantity = 6 };

            Guid productId = Guid.NewGuid();
            CartItem cartItem = CartItem.Create(Guid.NewGuid(), productId, "iPhone 16", 1200, 2);
            Guid cartItemId = cartItem.Id;
            ProductDto productDto = new() { Title = "iPhone 16", Price = 1200, QuantityInStock = 20 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            cartItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Quantity.Should().Be(changeDto.Quantity);

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCartItemAsync_ReturnsTrue_WhenExists()
        {
            CartItem cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), "iPhone 16", 1299, 3);
            Guid cartItemId = cartItem.Id;

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            cartItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.DeleteCartItemAsync(cartItemId, It.IsAny<CancellationToken>());

            result.Should().BeTrue();

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.Remove(cartItem), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCartItemAsync_ReturnsFalse_WhenNotExists()
        {
            Guid cartItemId = Guid.NewGuid();

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            var service = CreateService(cartItemRepositoryMock, productClientMock);

            var result = await service.DeleteCartItemAsync(cartItemId, It.IsAny<CancellationToken>());

            result.Should().BeFalse();

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.Remove(It.IsAny<CartItem>()), Times.Never);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}