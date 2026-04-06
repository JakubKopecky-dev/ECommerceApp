using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Application.DTOs.Product;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;
using Shared.Contracts.DTOs;
using System.Reflection;
using ProductServiceService = ProductService.Application.Services.ProductService;

namespace ProductService.UnitTests.Services
{
    public class ProductServiceTests
    {
        private static ProductServiceService CreateService(
            Mock<IProductRepository> productRepositoryMock,
            Mock<ICategoryRepository>? categoryRepositoryMock = null,
            Mock<IProductReviewRepository>? productReviewRepositoryMock = null)
        {
            return new ProductServiceService(
                productRepositoryMock.Object,
                (categoryRepositoryMock ?? new Mock<ICategoryRepository>()).Object,
                (productReviewRepositoryMock ?? new Mock<IProductReviewRepository>()).Object,
                new Mock<ILogger<ProductServiceService>>().Object
            );
        }

        private static Product CreateProduct(string title = "iPhone 16", string description = "Great phone", decimal price = 1299m, Guid? brandId = null)
            => Product.Create(title, description, price, "https://example.com/image.jpg", brandId ?? Guid.NewGuid());

        private static void AddCategories(Product product, IEnumerable<Category> categories)
        {
            var field = typeof(Product).GetField("_categories", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (List<Category>)field!.GetValue(product)!;
            list.AddRange(categories);
        }

        private static void AddReviews(Product product, IEnumerable<ProductReview> reviews)
        {
            var field = typeof(Product).GetField("_reviews", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (List<ProductReview>)field!.GetValue(product)!;
            list.AddRange(reviews);
        }

        private static void AddProducts(Brand brand, IEnumerable<Product> products)
        {
            var field = typeof(Brand).GetField("_products", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (List<Product>)field!.GetValue(brand)!;
            list.AddRange(products);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsAsync_ReturnsProductDtoList_WhenExists()
        {
            Brand brand = Brand.Create("Apple", "Tech company");
            List<Product> products =
            [
                CreateProduct("iPhone 16", brandId: brand.Id),
                CreateProduct("MacBook Air", brandId: brand.Id)
            ];

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductDto> result = await service.GetAllProductsAsync();

            result.Should().HaveCount(2);
            productRepositoryMock.Verify(p => p.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsAsync_ReturnsEmptyList_WhenNotExists()
        {
            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductDto> result = await service.GetAllProductsAsync();

            result.Should().BeEmpty();
            productRepositoryMock.Verify(p => p.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProductByIdAsync_ReturnsProductExtendedDto_WhenExists()
        {
            Product product = CreateProduct();
            Guid productId = product.Id;
            Category category = Category.Create("Phone");
            AddCategories(product, [category]);

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var service = CreateService(productRepositoryMock);

            ProductExtendedDto? result = await service.GetProductByIdAsync(productId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(productId);
            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProductByIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var service = CreateService(productRepositoryMock);

            ProductExtendedDto? result = await service.GetProductByIdAsync(productId);

            result.Should().BeNull();
            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateProductAsync_ReturnsProductExtendedDto()
        {
            Brand brand = Brand.Create("Apple", "Tech company");
            CreateProductDto createDto = new()
            {
                Title = "iPhone 16",
                Description = "Great phone",
                Price = 1299m,
                ImageUrl = "https://example.com/image.jpg",
                BrandId = brand.Id,
                QuantityInStock = 10,
                Categories = ["Phone"]
            };

            List<Category> categories = [Category.Create("Phone")];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<ICategoryRepository> categoryRepositoryMock = new();

            categoryRepositoryMock
                .Setup(c => c.GetCategoriesByName(createDto.Categories, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            productRepositoryMock
                .Setup(p => p.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productRepositoryMock, categoryRepositoryMock);

            ProductExtendedDto result = await service.CreateProductAsync(createDto);

            result.Should().NotBeNull();
            result.Title.Should().Be(createDto.Title);
            result.Price.Should().Be(createDto.Price);
            result.QuantityInStock.Should().Be(createDto.QuantityInStock);

            categoryRepositoryMock.Verify(c => c.GetCategoriesByName(createDto.Categories, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProductAsync_ReturnsProductExtendedDto_WhenExists()
        {
            Product product = CreateProduct();
            Guid productId = product.Id;
            Category category = Category.Create("Phone");
            AddCategories(product, [category]);

            UpdateProductDto updateDto = new()
            {
                Title = "iPhone 16 Plus",
                Description = "Better phone",
                Price = 1499m,
                ImageUrl = "https://example.com/image2.jpg",
                BrandId = product.BrandId,
                Categories = ["Phone"]
            };

            List<Category> categories = [category];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<ICategoryRepository> categoryRepositoryMock = new();

            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            categoryRepositoryMock
                .Setup(c => c.GetCategoriesByTitle(updateDto.Categories, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productRepositoryMock, categoryRepositoryMock);

            ProductExtendedDto? result = await service.UpdateProductAsync(productId, updateDto);

            result.Should().NotBeNull();
            result!.Title.Should().Be(updateDto.Title);
            result.Price.Should().Be(updateDto.Price);

            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            categoryRepositoryMock.Verify(c => c.GetCategoriesByTitle(updateDto.Categories, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProductAsync_ReturnsNull_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();
            UpdateProductDto updateDto = new() { Title = "iPhone 16 Plus", Categories = ["Phone"] };

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<ICategoryRepository> categoryRepositoryMock = new();

            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var service = CreateService(productRepositoryMock, categoryRepositoryMock);

            ProductExtendedDto? result = await service.UpdateProductAsync(productId, updateDto);

            result.Should().BeNull();

            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            categoryRepositoryMock.Verify(c => c.GetCategoriesByTitle(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()), Times.Never);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task InactivateProductAsync_ReturnsProductDto_WhenExists()
        {
            Product product = CreateProduct();
            product.Activate();
            Guid productId = product.Id;

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productRepositoryMock);

            ProductDto? result = await service.InactivateProductAsync(productId);

            result.Should().NotBeNull();
            result!.IsActive.Should().BeFalse();

            productRepositoryMock.Verify(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task InactivateProductAsync_ReturnsNull_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var service = CreateService(productRepositoryMock);

            ProductDto? result = await service.InactivateProductAsync(productId);

            result.Should().BeNull();

            productRepositoryMock.Verify(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ActivateProductAsync_ReturnsProductDto_WhenExists()
        {
            Product product = CreateProduct();
            Guid productId = product.Id;

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productRepositoryMock);

            ProductDto? result = await service.ActivateProductAsync(productId);

            result.Should().NotBeNull();
            result!.IsActive.Should().BeTrue();

            productRepositoryMock.Verify(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ActivateProductAsync_ReturnsNull_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var service = CreateService(productRepositoryMock);

            ProductDto? result = await service.ActivateProductAsync(productId);

            result.Should().BeNull();

            productRepositoryMock.Verify(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProductAsync_ReturnsTrue_WhenExists()
        {
            Product product = CreateProduct();
            Guid productId = product.Id;

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAndReviewsAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productRepositoryMock);

            bool result = await service.DeleteProductAsync(productId);

            result.Should().BeTrue();

            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAndReviewsAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.Remove(product), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProductAsync_ReturnsFalse_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAndReviewsAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var service = CreateService(productRepositoryMock);

            bool result = await service.DeleteProductAsync(productId);

            result.Should().BeFalse();

            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAndReviewsAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.Remove(It.IsAny<Product>()), Times.Never);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByBrandIdAsync_ReturnsProductDtoList_WhenExists()
        {
            Brand brand = Brand.Create("Apple", "Tech company");
            List<Product> products =
            [
                CreateProduct("iPhone 16", brandId: brand.Id),
                CreateProduct("MacBook Air", brandId: brand.Id)
            ];

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllProductsByBrandIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductDto> result = await service.GetAllProductsByBrandIdAsync(brand.Id);

            result.Should().HaveCount(2);
            productRepositoryMock.Verify(p => p.GetAllProductsByBrandIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByBrandIdAsync_ReturnsEmptyList_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllProductsByBrandIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductDto> result = await service.GetAllProductsByBrandIdAsync(brandId);

            result.Should().BeEmpty();
            productRepositoryMock.Verify(p => p.GetAllProductsByBrandIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByCategoriesAsync_ReturnsProductDtoList_WhenExists()
        {
            List<string> categories = ["Phone", "Laptop"];
            List<Product> products =
            [
                CreateProduct("iPhone 16"),
                CreateProduct("MacBook Air")
            ];

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductDto> result = await service.GetAllProductsByCategoriesAsync(categories);

            result.Should().HaveCount(2);
            productRepositoryMock.Verify(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByCategoriesAsync_ReturnsEmptyList_WhenNotExists()
        {
            List<string> categories = ["Phone", "Laptop"];

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductDto> result = await service.GetAllProductsByCategoriesAsync(categories);

            result.Should().BeEmpty();
            productRepositoryMock.Verify(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllActiveProductsAsync_ReturnsProductDtoList_WhenExists()
        {
            List<Product> products =
            [
                CreateProduct("iPhone 16"),
                CreateProduct("MacBook Air")
            ];
            products.ForEach(p => p.Activate());

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductDto> result = await service.GetAllActiveProductsAsync();

            result.Should().HaveCount(2);
            result.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
            productRepositoryMock.Verify(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllActiveProductsAsync_ReturnsEmptyList_WhenNotExists()
        {
            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductDto> result = await service.GetAllActiveProductsAsync();

            result.Should().BeEmpty();
            productRepositoryMock.Verify(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllInactiveProductsAsync_ReturnsProductDtoList_WhenExists()
        {
            List<Product> products =
            [
                CreateProduct("iPhone 16"),
                CreateProduct("MacBook Air")
            ];

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductDto> result = await service.GetAllInactiveProductsAsync();

            result.Should().HaveCount(2);
            result.Should().AllSatisfy(p => p.IsActive.Should().BeFalse());
            productRepositoryMock.Verify(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllInactiveProductsAsync_ReturnsEmptyList_WhenNotExists()
        {
            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductDto> result = await service.GetAllInactiveProductsAsync();

            result.Should().BeEmpty();
            productRepositoryMock.Verify(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ProductsQuantityCheckFromCartAsync_ReturnsOutOfStockList_WhenLackOfStock()
        {
            Guid productId1 = Guid.NewGuid();
            Guid productId2 = Guid.NewGuid();

            List<ProductQuantityCheckRequestDto> productsFromCart =
            [
                new() { Id = productId1, Quantity = 10 },
                new() { Id = productId2, Quantity = 10 }
            ];

            List<Guid> productIds = [.. productsFromCart.Select(p => p.Id)];

            IReadOnlyList<ProductQuantityCheckResponseDto> productsStock =
            [
                new() { Id = productId1, QuantityInStock = 2, Title = "iPhone 16" },
                new() { Id = productId2, QuantityInStock = 3, Title = "MacBook Pro" }
            ];

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productsStock);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductQuantityCheckResponseDto> result = await service.ProductsQuantityCheckFromCartAsync(productsFromCart);

            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(productsStock);
            productRepositoryMock.Verify(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ProductsQuantityCheckFromCartAsync_ReturnsEmptyList_WhenEnoughProducts()
        {
            Guid productId1 = Guid.NewGuid();
            Guid productId2 = Guid.NewGuid();

            List<ProductQuantityCheckRequestDto> productsFromCart =
            [
                new() { Id = productId1, Quantity = 2 },
                new() { Id = productId2, Quantity = 2 }
            ];

            List<Guid> productIds = [.. productsFromCart.Select(p => p.Id)];

            IReadOnlyList<ProductQuantityCheckResponseDto> productsStock =
            [
                new() { Id = productId1, QuantityInStock = 100 },
                new() { Id = productId2, QuantityInStock = 100 }
            ];

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productsStock);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductQuantityCheckResponseDto> result = await service.ProductsQuantityCheckFromCartAsync(productsFromCart);

            result.Should().BeEmpty();
            productRepositoryMock.Verify(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ProductsQuantityCheckFromCartAsync_ReturnsNotFoundEntry_WhenProductNotFound()
        {
            Guid productId1 = Guid.NewGuid();
            Guid productId2 = Guid.NewGuid();

            List<ProductQuantityCheckRequestDto> productsFromCart =
            [
                new() { Id = productId1, Quantity = 2 },
                new() { Id = productId2, Quantity = 2 }
            ];

            List<Guid> productIds = [.. productsFromCart.Select(p => p.Id)];

            IReadOnlyList<ProductQuantityCheckResponseDto> productsStock =
            [
                new() { Id = productId1, QuantityInStock = 100 }
            ];

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productsStock);

            var service = CreateService(productRepositoryMock);

            IReadOnlyList<ProductQuantityCheckResponseDto> result = await service.ProductsQuantityCheckFromCartAsync(productsFromCart);

            result.Should().HaveCount(1);
            result[0].Id.Should().Be(productId2);
            result[0].Title.Should().Be("(Nout found)");
            result[0].QuantityInStock.Should().Be(0);
            productRepositoryMock.Verify(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ProductQuantityReserved_DecreasesStock_WhenProductsExist()
        {
            Product product1 = CreateProduct("iPhone 16");
            Product product2 = CreateProduct("MacBook Pro");
            product1.AddStock(10);
            product2.AddStock(10);

            List<OrderItemCreatedDto> orderItemsDto =
            [
                new() { ProductId = product1.Id, Quantity = 5 },
                new() { ProductId = product2.Id, Quantity = 2 }
            ];

            IReadOnlyList<Product> products = [product1, product2];
            List<Guid> productIds = [.. orderItemsDto.Select(p => p.ProductId)];

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllProductsByIdsAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productRepositoryMock);

            await service.ProductQuantityReserved(orderItemsDto);

            product1.QuantityInStock.Should().Be(5);
            product2.QuantityInStock.Should().Be(8);

            productRepositoryMock.Verify(p => p.GetAllProductsByIdsAsync(productIds, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task ProductQuantityReserved_ShouldDoNothing_WhenProductNotFound()
        {
            Product product1 = CreateProduct("iPhone 16");
            product1.AddStock(10);

            List<OrderItemCreatedDto> orderItemsDto =
            [
                new() { ProductId = product1.Id, Quantity = 5 },
                new() { ProductId = Guid.NewGuid(), Quantity = 2 }
            ];

            IReadOnlyList<Product> products = [product1];
            List<Guid> productIds = [.. orderItemsDto.Select(p => p.ProductId)];

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetAllProductsByIdsAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productRepositoryMock);

            await service.ProductQuantityReserved(orderItemsDto);

            product1.QuantityInStock.Should().Be(5);
            productRepositoryMock.Verify(p => p.GetAllProductsByIdsAsync(productIds, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}