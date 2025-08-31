using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CartService.Application.Common;
using CartService.Application.DTOs.CartItem;
using CartService.Application.DTOs.External;
using CartService.Application.Interfaces.External;
using CartService.Application.Interfaces.Repositories;
using CartService.Application.Services;
using CartService.Domain.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CartService.UnitTests.Services
{
    public class CartItemServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCartItemAsync_ReturnsCartItemDto_WhenExists()
        {
            Guid cartItemId = Guid.NewGuid();

            CartItem cartItem = new() { Id = cartItemId, CartId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2 };
            CartItemDto expectedDto = new() { Id = cartItemId, CartId = cartItem.CartId, ProductId = cartItem.ProductId, Quantity = cartItem.Quantity };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            mapperMock
                .Setup(m => m.Map<CartItemDto>(cartItem))
                .Returns(expectedDto);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                new Mock<IProductReadClient>().Object
            );


            var result = await service.GetCartItemByIdAsync(cartItemId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartItemDto>(cartItem), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCartItemAsync_ReturnsNull_WhenNotExists()
        {
            Guid cartItemId = Guid.NewGuid();

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                new Mock<IProductReadClient>().Object
            );


            var result = await service.GetCartItemByIdAsync(cartItemId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItem>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_UpdatesQuantity_WhenAlreadyExists_AndStockSufficient()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 3 };

            CartItem existingCartItem = new() { Id = Guid.NewGuid(), CartId = cartId, ProductId = productId, Quantity = 2 };
            ProductDto productDto = new() { Title = "iPhone 16", Price = 1299, QuantityInStock = 10 };
            CartItemDto expectedDto = new() { Id = existingCartItem.Id, CartId = cartId, ProductId = productId, Quantity = 5 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            cartItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<CartItemDto>(existingCartItem))
                .Returns(expectedDto);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedDto);

            cartItemRepositoryMock.Verify(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartItemDto>(existingCartItem), Times.Once);
            cartItemRepositoryMock.Verify(i => i.InsertAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_ReturnsFail_ProductNotFound_WhenAlreadyExists()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 1 };

            CartItem existingCartItem = new() { Id = Guid.NewGuid(), CartId = cartId, ProductId = productId, Quantity = 2 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.ProductNotFound);

            cartItemRepositoryMock.Verify(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItem>()), Times.Never);
            cartItemRepositoryMock.Verify(i => i.InsertAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_ReturnsFail_OutOfStock_WhenAlreadyExists()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 5 };

            CartItem existing = new() { Id = Guid.NewGuid(), CartId = cartId, ProductId = productId, Quantity = 8 };
            ProductDto productDto = new() { Title = "iPhone 16", Price = 1200, QuantityInStock = 12 }; // newQuantity = 8+5 = 13 > 12

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.OutOfStock);

            cartItemRepositoryMock.Verify(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItem>()), Times.Never);
            cartItemRepositoryMock.Verify(i => i.InsertAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_CreatesNew_WhenNotExists_AndStockSufficient()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 2 };

            CartItem cartItem = new() { Id = Guid.Empty, CartId = cartId, ProductId = productId, Quantity = createDto.Quantity, CreatedAt = DateTime.UtcNow };
            ProductDto productDto = new() { Title = "iPhone", Price = 1200, QuantityInStock = 10 };

            CartItem createdCartItem = new() { Id = Guid.NewGuid(), CartId = cartId, ProductId = productId, Quantity = 2, ProductName = productDto.Title, UnitPrice = productDto.Price };
            CartItemDto expectedDto = new() { Id = createdCartItem.Id, CartId = cartId, ProductId = productId, Quantity = 2, ProductName = productDto.Title, UnitPrice = productDto.Price };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            mapperMock
                .Setup(m => m.Map<CartItem>(createDto))
                .Returns(cartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            cartItemRepositoryMock
                .Setup(i => i.InsertAsync(cartItem, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdCartItem);

            mapperMock
                .Setup(m => m.Map<CartItemDto>(createdCartItem))
                .Returns(expectedDto);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedDto);

            cartItemRepositoryMock.Verify(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartItem>(createDto), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.InsertAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CartItemDto>(createdCartItem), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_ReturnsFail_ProductNotFound_WhenNotExists()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 2 };

            CartItem cartItem = new() { Id = Guid.Empty, CartId = cartId, ProductId = productId, Quantity = 2 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            mapperMock
                .Setup(m => m.Map<CartItem>(createDto))
                .Returns(cartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.ProductNotFound);

            cartItemRepositoryMock.Verify(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartItem>(createDto), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.InsertAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItem>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCartItemOrChangeQuantityAsync_ReturnsFail_OutOfStock_WhenNotExists()
        {
            Guid cartId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();

            CreateCartItemDto createDto = new() { CartId = cartId, ProductId = productId, Quantity = 7 };

            CartItem mapped = new() { CartId = cartId, ProductId = productId, Quantity = 7 };
            ProductDto productDto = new() { Title = "iPhone 15", Price = 999, QuantityInStock = 5 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            mapperMock
                .Setup(m => m.Map<CartItem>(createDto))
                .Returns(mapped);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.CreateCartItemOrChangeQuantityAsync(createDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.OutOfStock);

            cartItemRepositoryMock.Verify(i => i.FindCartItemByCartIdAndProductIdAsync(cartId, productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartItem>(createDto), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.InsertAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItem>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantityAsync_ReturnsFail_WhenCartItemNotFound()
        {
            Guid cartItemId = Guid.NewGuid();
            ChangeQuantityCartItemDto changeDto = new() { Quantity = 3 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.ProductNotFound);

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            productClientMock.Verify(c => c.GetProductAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItem>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantityAsync_Deletes_WhenQuantityZero()
        {
            Guid cartItemId = Guid.NewGuid();
            ChangeQuantityCartItemDto changeDto = new() { Quantity = 0 };

            CartItem cartItem = new() { Id = cartItemId, CartId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 4 };
            CartItemDto expectedDeletedDto = new() { Id = cartItemId, CartId = cartItem.CartId, ProductId = cartItem.ProductId, Quantity = cartItem.Quantity };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            mapperMock
                .Setup(m => m.Map<CartItemDto>(cartItem))
                .Returns(expectedDeletedDto);

            cartItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedDeletedDto);

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartItemDto>(cartItem), Times.Once);
            cartItemRepositoryMock.Verify(i => i.Remove(cartItem), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantityAsync_ReturnsFail_ProductNotFound_WhenProductMissing()
        {
            Guid cartItemId = Guid.NewGuid();
            ChangeQuantityCartItemDto changeDto = new() { Quantity = 7 };

            CartItem cartItem = new() { Id = cartItemId, ProductId = Guid.NewGuid(), Quantity = 2 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(cartItem.ProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.ProductNotFound);

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(cartItem.ProductId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItem>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantityAsync_ReturnsFail_OutOfStock_WhenInsufficient()
        {
            Guid cartItemId = Guid.NewGuid();
            ChangeQuantityCartItemDto changeDto = new() { Quantity = 10 };

            Guid productId = Guid.NewGuid();
            CartItem cartItem = new() { Id = cartItemId, ProductId = productId, Quantity = 2 };

            ProductDto productDto = new() { Title = "iPhone 16", Price = 1200, QuantityInStock = 5 }; // 10 > 5

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartItemError.OutOfStock);

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItem>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeCartItemQuantityAsync_ReturnsOk_WhenStockSufficient()
        {
            Guid cartItemId = Guid.NewGuid();
            ChangeQuantityCartItemDto changeDto = new() { Quantity = 6 };

            Guid productId = Guid.NewGuid();
            CartItem cartItem = new() { Id = cartItemId, ProductId = productId, Quantity = 2 };

            ProductDto productDto = new() { Title = "iPhone 16", Price = 1200, QuantityInStock = 20 };
            CartItemDto expectedDto = new() { Id = cartItemId, ProductId = productId, Quantity = 6 };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IProductReadClient> productClientMock = new();
            Mock<IMapper> mapperMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            productClientMock
                .Setup(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            cartItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<CartItemDto>(cartItem))
                .Returns(expectedDto);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.ChangeCartItemQuantityAsync(cartItemId, changeDto, It.IsAny<CancellationToken>());

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedDto);

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            productClientMock.Verify(c => c.GetProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartItemDto>(cartItem), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCartItemAsync_ReturnsCartItemDto_WhenExists()
        {
            Guid cartItemId = Guid.NewGuid();

            CartItem cartItem = new() { Id = cartItemId, CartId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 3 };
            CartItemDto expectedDto = new() { Id = cartItemId, CartId = cartItem.CartId, ProductId = cartItem.ProductId, Quantity = cartItem.Quantity };

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartItem);

            mapperMock
                .Setup(m => m.Map<CartItemDto>(cartItem))
                .Returns(expectedDto);

            cartItemRepositoryMock
                .Setup(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.DeleteCartItemAsync(cartItemId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartItemDto>(cartItem), Times.Once);
            cartItemRepositoryMock.Verify(i => i.Remove(cartItem), Times.Once);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCartItemAsync_ReturnsNull_WhenNotExists()
        {
            Guid cartItemId = Guid.NewGuid();

            Mock<ICartItemRepository> cartItemRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReadClient> productClientMock = new();

            cartItemRepositoryMock
                .Setup(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CartItem?)null);

            CartItemService service = new(
                cartItemRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CartItemService>>().Object,
                productClientMock.Object
            );


            var result = await service.DeleteCartItemAsync(cartItemId, It.IsAny<CancellationToken>());

            result.Should().BeNull();

            cartItemRepositoryMock.Verify(i => i.FindByIdAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CartItemDto>(It.IsAny<CartItem>()), Times.Never);
            cartItemRepositoryMock.Verify(i => i.Remove(It.IsAny<CartItem>()), Times.Never);
            cartItemRepositoryMock.Verify(i => i.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



    }
}
