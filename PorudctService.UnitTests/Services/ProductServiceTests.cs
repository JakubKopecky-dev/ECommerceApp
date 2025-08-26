using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Application.DTOs.Product;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entity;
using Shared.Contracts.DTOs;
using ProductServiceService = ProductService.Application.Services.ProductService;



namespace ProductService.UnitTests.Services
{
    public class ProductServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsAsync_ReturnsProductDtoList_WhenExists()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };

            List<Product> products =
            [
                new() {Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16" },
                new() {Id = Guid.NewGuid(), Brand = brand, Title = "MacBook Air"}
            ];

            brand.Products = products;

            List<ProductDto> expectedDto =
            [
                new() {Id = products[0].Id, Title = products[0].Title},
                new() {Id = products[1].Id, Title = products[1].Title}

            ];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            IReadOnlyList<ProductDto> result = await service.GetAllProductsAsync();

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsAsync_ReturnsEmptyList_WhenNotExists()
        {
            List<Product> products = [];

            List<ProductDto> expectedDto = [];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            IReadOnlyList<ProductDto> result = await service.GetAllProductsAsync();

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProductByIdAsync_ReturnsProductExtendedDto_WhenExists()
        {
            Guid productId = Guid.NewGuid();

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };

            Product product = new() { Id = productId, Brand = brand, Title = "iPhone 16", Categories = [new() { Id = Guid.NewGuid(), Title = "Phone" }] };
            ProductExtendedDto expectedDto = new() { Id = productId, Title = product.Title, Categories = ["Phone"] };
            brand.Products.Add(product);

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            mapperMock
                .Setup(m => m.Map<ProductExtendedDto>(product))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductExtendedDto? result = await service.GetProductByIdAsync(productId);

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductExtendedDto>(product), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProductByIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);


            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductExtendedDto? result = await service.GetProductByIdAsync(productId);

            result.Should().BeNull();

            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductExtendedDto>(It.IsAny<Product>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateProductAsync_ReturnsProductExtendedDto()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            CreateProductDto createDto = new() { Title = "iPhone 16", Categories = ["Phone"], BrandId = brand.Id };

            List<Category> expectedCategory = [new() { Id = Guid.NewGuid(), Title = "Phone" }];
            Product product = new() { Id = Guid.Empty, Brand = brand, Title = createDto.Title, CreatedAt = DateTime.UtcNow, Categories = [] };
            Product createdProduct = new() { Id = Guid.NewGuid(), Brand = brand, Title = product.Title, CreatedAt = product.CreatedAt, Categories = expectedCategory};
            ProductExtendedDto expectedDto = new() { Title = createDto.Title, Categories = createDto.Categories, BrandId = createDto.BrandId, CreatedAt = createdProduct.CreatedAt };

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<ICategoryRepository> categoryRepositoryMock = new();

            mapperMock
                .Setup(m => m.Map<Product>(createDto))
                .Returns(product);

            categoryRepositoryMock
                .Setup(c => c.GetCategoriesByName(createDto.Categories, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCategory);

            productRepositoryMock
                .Setup(p => p.InsertAsync(product, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdProduct);

            mapperMock
                .Setup(m => m.Map<ProductExtendedDto>(createdProduct))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                categoryRepositoryMock.Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductExtendedDto result = await service.CreateProductAsync(createDto);

            result.Should().BeEquivalentTo(expectedDto);

            mapperMock.Verify(m => m.Map<Product>(createDto), Times.Once);
            categoryRepositoryMock.Verify(c => c.GetCategoriesByName(createDto.Categories, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.InsertAsync(product, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify((m => m.Map<ProductExtendedDto>(createdProduct)), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProductAsync_ReturnsProductExtendedDto_WhenExists()
        {
            Guid productId = Guid.NewGuid();
            UpdateProductDto updateDto = new() { Title = "iPhone 16 Plus", Categories = ["Phone"] };

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            List<Category> expectedCategory = [new() { Id = Guid.NewGuid(), Title = "Phone" }];

            Product productDb = new() { Id = productId, Brand = brand, Title = "iPhone 16", Categories = expectedCategory, UpdatedAt = DateTime.UtcNow };
            ProductExtendedDto expectedDto = new() { Id = productId, Title = updateDto.Title, Categories = updateDto.Categories, UpdatedAt = productDb.UpdatedAt };
            brand.Products.Add(productDb);


            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<ICategoryRepository> categoryRepositoryMock = new();

            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDb);

            mapperMock
                .Setup(m => m.Map<UpdateProductDto, Product>(updateDto, productDb))
                .Returns(productDb);

            categoryRepositoryMock
                .Setup(c => c.GetCategoriesByTitle(updateDto.Categories, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCategory);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<ProductExtendedDto>(productDb))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                categoryRepositoryMock.Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductExtendedDto? result = await service.UpdateProductAsync(productId, updateDto);

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<UpdateProductDto, Product>(updateDto, productDb), Times.Once);
            categoryRepositoryMock.Verify(c => c.GetCategoriesByTitle(updateDto.Categories, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductExtendedDto>(productDb), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProductAsync_ReturnsNull_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();
            UpdateProductDto updateDto = new() { Title = "iPhone 16 Plus", Categories = ["Phone"] };

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<ICategoryRepository> categoryRepositoryMock = new();

            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                categoryRepositoryMock.Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductExtendedDto? result = await service.UpdateProductAsync(productId, updateDto);

            result.Should().BeNull();

            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<UpdateProductDto, Product>(updateDto, It.IsAny<Product>()), Times.Never);
            categoryRepositoryMock.Verify(c => c.GetCategoriesByTitle(updateDto.Categories, It.IsAny<CancellationToken>()), Times.Never);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<ProductExtendedDto>(It.IsAny<Product>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task InactivateProductAsync_RetursProductDto_WhenExists()
        {
            Guid productId = Guid.NewGuid();

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };

            Product product = new() { Id = productId, Brand = brand, Title = "iPhone 16", IsActive = false };
            ProductDto expectedDto = new() { Id = productId, Title = product.Title, IsActive = false };
            brand.Products.Add(product);

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<ProductDto>(product))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductDto? result = await service.InactivateProductAsync(productId);

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductDto>(product), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task InactivateProductAsync_RetursNull_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductDto? result = await service.InactivateProductAsync(productId);

            result.Should().BeNull();

            productRepositoryMock.Verify(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<Product>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ActivateProductAsync_RetursProductDto_WhenExists()
        {
            Guid productId = Guid.NewGuid();

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };

            Product product = new() { Id = productId, Brand = brand, Title = "iPhone 16", IsActive = true };
            ProductDto expectedDto = new() { Id = productId, Title = product.Title, IsActive = true };
            brand.Products.Add(product);

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<ProductDto>(product))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductDto? result = await service.ActivateProductAsync(productId);

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductDto>(product), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ActivateProductAsync_RetursNull_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductDto? result = await service.ActivateProductAsync(productId);

            result.Should().BeNull();

            productRepositoryMock.Verify(p => p.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<Product>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProductAsync_RetursProductDto_WhenExists()
        {
            Guid productId = Guid.NewGuid();

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };

            Product product = new() { Id = productId, Brand = brand, Title = "iPhone 16", Categories = [new() { Id = Guid.NewGuid(), Title = "Phone" }] };
            ProductDto expectedDto = new() { Id = productId, Title = product.Title };
            ProductReview review1 = new() { Id = Guid.NewGuid(), Product = product, UserId = Guid.NewGuid() };
            ProductReview review2 = new() { Id = Guid.NewGuid(), Product = product, UserId = Guid.NewGuid() };
            product.Reviews.Add(review1);
            product.Reviews.Add(review2);
            brand.Products.Add(product);

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReviewRepository> productReviewRepositoryMock = new();

            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAndReviewsAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            mapperMock
                .Setup(m => m.Map<ProductDto>(product))
                .Returns(expectedDto);

            productReviewRepositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductDto? result = await service.DeleteProductAsync(productId);

            result.Should().BeEquivalentTo(expectedDto);
            product.Categories.Should().BeEmpty();

            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAndReviewsAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductDto>(product), Times.Once);
            productReviewRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            productRepositoryMock.Verify(p => p.Remove(product), Times.Once);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProductAsync_RetursNull_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductReviewRepository> productReviewRepositoryMock = new();

            productRepositoryMock
                .Setup(p => p.FindProductByIdIncludeCategoriesAndReviewsAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            ProductDto? result = await service.DeleteProductAsync(productId);

            result.Should().BeNull();

            productRepositoryMock.Verify(p => p.FindProductByIdIncludeCategoriesAndReviewsAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<Product>()), Times.Never);
            productReviewRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            productRepositoryMock.Verify(p => p.Remove(It.IsAny<Product>()), Times.Never);
            productRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByBrandIdAsync_RetursProductDtoList_WhenExists()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };

            List<Product> products =
            [
                new() {Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16" },
                new() {Id = Guid.NewGuid(), Brand = brand, Title = "MacBook Air"}
            ];

            brand.Products = products;

            List<ProductDto> expectedDto =
            [
                new() {Id = products[0].Id, Title = products[0].Title},
                new() {Id = products[1].Id, Title = products[1].Title}

            ];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllProductsByBrandIdAsync(brand.Id,It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            IReadOnlyList<ProductDto> result = await service.GetAllProductsByBrandIdAsync(brand.Id);

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetAllProductsByBrandIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByBrandIdAsync_RetursEmptyList_WhenNotExists()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };

            List<Product> products = [];
            List<ProductDto> expectedDto = [];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllProductsByBrandIdAsync(brand.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            IReadOnlyList<ProductDto> result = await service.GetAllProductsByBrandIdAsync(brand.Id);

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetAllProductsByBrandIdAsync(brand.Id, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByCategoriesAsync_ReturnsProductDtoList_WhenExists()
        {
            List<string> categories = ["Phone", "Laptop"];
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };

            List<Product> products =
            [
                new() {Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16",  Categories = [new() { Id = Guid.NewGuid(), Title = "Phone"}] },
                new() {Id = Guid.NewGuid(), Brand = brand, Title = "MacBook Air", Categories = [new() { Id = Guid.NewGuid(), Title = "Laptop"}] }
            ];

            brand.Products = products;

            List<ProductDto> expectedDto =
            [
                new() {Id = products[0].Id, Title = products[0].Title},
                new() {Id = products[1].Id, Title = products[1].Title}

            ];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            IReadOnlyList<ProductDto> result = await service.GetAllProductsByCategoriesAsync(categories);

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByCategoriesAsync_ReturnsEmptyList_WhenNotExists()
        {
            List<string> categories = ["Phone", "Laptop"];

            List<Product> products = [];
            List<ProductDto> expectedDto = [];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            IReadOnlyList<ProductDto> result = await service.GetAllProductsByCategoriesAsync(categories);

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllActiveProductsAsync_RetursProductDtoList_WhenExists()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };

            List<Product> products =
            [
                new() {Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16", IsActive = true },
                new() {Id = Guid.NewGuid(), Brand = brand, Title = "MacBook Air", IsActive = true}
            ];

            brand.Products = products;

            List<ProductDto> expectedDto =
            [
                new() {Id = products[0].Id, Title = products[0].Title, IsActive = true},
                new() {Id = products[1].Id, Title = products[1].Title, IsActive = true}

            ];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            IReadOnlyList<ProductDto> result = await service.GetAllActiveProductsAsync();

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllActiveProductsAsync_RetursEmptyList_WhenNotExists()
        {
            List<Product> products = [];
            List<ProductDto> expectedDto = [];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            IReadOnlyList<ProductDto> result = await service.GetAllActiveProductsAsync();

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllInactiveProductsAsync_RetursProductDtoList_WhenExists()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };

            List<Product> products =
            [
                new() {Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16", IsActive = false },
                new() {Id = Guid.NewGuid(), Brand = brand, Title = "MacBook Air", IsActive = false}
            ];

            brand.Products = products;

            List<ProductDto> expectedDto =
            [
                new() {Id = products[0].Id, Title = products[0].Title, IsActive = false},
                new() {Id = products[1].Id, Title = products[1].Title, IsActive = false}

            ];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            IReadOnlyList<ProductDto> result = await service.GetAllInactiveProductsAsync();

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllInactiveProductsAsync_RetursEmptyList_WhenNotExists()
        {
            List<Product> products = [];
            List<ProductDto> expectedDto = [];

            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(expectedDto);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );


            IReadOnlyList<ProductDto> result = await service.GetAllInactiveProductsAsync();

            result.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ProductsQuantityCheckFromCartAsync_RetursProductQuantityCheckResponseDtoList_WhenLackOfStock()
        {
            Guid productId1 = Guid.NewGuid();
            Guid productId2 = Guid.NewGuid();

            List<ProductQuantityCheckRequestDto> productsFromCart =
            [
                new() {Id = productId1, Quantity = 10 },
                new() {Id = productId2, Quantity = 10 }

            ];

            List<Guid> productIds = [.. productsFromCart.Select(p => p.Id)];

            IReadOnlyList<ProductQuantityCheckResponseDto> productsStock =
            [
                new() {Id = productId1, QuantityInStock = 2, Title = "iPhone 16"},
                new() {Id = productId2, QuantityInStock = 3, Title = "MacBook Pro"}
            ];

            IReadOnlyList<ProductQuantityCheckResponseDto> expectedDto = productsStock;
 

            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productsStock);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                new Mock<IMapper>().Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );

            IReadOnlyList<ProductQuantityCheckResponseDto> resutl = await service.ProductsQuantityCheckFromCartAsync(productsFromCart);

            resutl.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ProductsQuantityCheckFromCartAsync_RetursEmptyList_WhenEnoughProducts()
        {
            Guid productId1 = Guid.NewGuid();
            Guid productId2 = Guid.NewGuid();

            List<ProductQuantityCheckRequestDto> productsFromCart =
            [
                new() {Id = productId1, Quantity = 2 },
                new() {Id = productId2, Quantity = 2 }

            ];

            List<Guid> productIds = [.. productsFromCart.Select(p => p.Id)];

            IReadOnlyList<ProductQuantityCheckResponseDto> productsStock =
            [
                new() {Id = productId1, QuantityInStock = 100},
                new() {Id = productId2, QuantityInStock = 100}
            ];


            Mock<IProductRepository> productRepositoryMock = new();
            productRepositoryMock
                .Setup(p => p.GetProductsAsQuantityCheckDtoAsync(productIds,It.IsAny<CancellationToken>()))
                .ReturnsAsync(productsStock);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                new Mock<IMapper>().Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );

            IReadOnlyList<ProductQuantityCheckResponseDto> resutl = await service.ProductsQuantityCheckFromCartAsync(productsFromCart);

            resutl.Should().BeEmpty();

            productRepositoryMock.Verify(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ProductsQuantityCheckFromCartAsync_RetursProductQuantityCheckResponseDtoList_WhenProductNotFound()
        {
            Guid productId1 = Guid.NewGuid();
            Guid productId2 = Guid.NewGuid();

            List<ProductQuantityCheckRequestDto> productsFromCart =
            [
                new() {Id = productId1, Quantity = 2 },
                new() {Id = productId2, Quantity = 2 }

            ];

            List<Guid> productIds = [.. productsFromCart.Select(p => p.Id)];

            IReadOnlyList<ProductQuantityCheckResponseDto> productsStock =
            [
                new() {Id = productId1, QuantityInStock = 100},

            ];

            IReadOnlyList<ProductQuantityCheckResponseDto> expectedDto =
            [
                new() {Id = productId2, Title = "(Nout found)", QuantityInStock = 0}
            ];


            Mock<IProductRepository> productRepositoryMock = new();

            productRepositoryMock
                .Setup(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productsStock);

            ProductServiceService service = new(
                productRepositoryMock.Object,
                new Mock<ICategoryRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                new Mock<IMapper>().Object,
                new Mock<ILogger<ProductServiceService>>().Object
                );

            IReadOnlyList<ProductQuantityCheckResponseDto> resutl = await service.ProductsQuantityCheckFromCartAsync(productsFromCart);

            resutl.Should().BeEquivalentTo(expectedDto);

            productRepositoryMock.Verify(p => p.GetProductsAsQuantityCheckDtoAsync(productIds, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ProductQuantityReservedAsync_DecreasesStock_WhenProductsExist()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product1 = new() { Id = Guid.NewGuid(), Title = "iPhone 16", Brand = brand, QuantityInStock = 10 };
            Product product2 = new() { Id = Guid.NewGuid(), Title = "MacBook Pro", Brand = brand, QuantityInStock = 10 };

            List<OrderItemCreatedDto> orderItemsDto =
            [
                new() {ProductId = product1.Id, Quantity = 5},
                new() {ProductId = product2.Id, Quantity = 2}
            ];

            IReadOnlyList<Product> products = [ product1, product2 ];
            List<Guid> productIds = [.. orderItemsDto.Select(p => p.ProductId)];

            Mock<IProductRepository> productRepositoryMock = new();

            productRepositoryMock
                .Setup(p => p.GetAllProductsByIdsAsync(productIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            productRepositoryMock
                .Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ProductServiceService service = new(
               productRepositoryMock.Object,
               new Mock<ICategoryRepository>().Object,
               new Mock<IProductReviewRepository>().Object,
               new Mock<IMapper>().Object,
               new Mock<ILogger<ProductServiceService>>().Object
               );
            

            await service.ProductQuantityReserved(orderItemsDto);

            product1.QuantityInStock.Should().Be(5);
            product2.QuantityInStock.Should().Be(8);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ProductQuantityReservedAsync_ShouldDoNothing_WhenProductNotFound()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product1 = new() { Id = Guid.NewGuid(), Title = "iPhone 16", Brand = brand, QuantityInStock = 10 };

            List<OrderItemCreatedDto> orderItemsDto =
            [
                new() {ProductId = product1.Id, Quantity = 5},
                new() {ProductId = Guid.NewGuid(), Quantity = 2}
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

            ProductServiceService service = new(
               productRepositoryMock.Object,
               new Mock<ICategoryRepository>().Object,
               new Mock<IProductReviewRepository>().Object,
               new Mock<IMapper>().Object,
               new Mock<ILogger<ProductServiceService>>().Object
               );


            await service.ProductQuantityReserved(orderItemsDto);

            product1.QuantityInStock.Should().Be(5);
        }



    }
}
