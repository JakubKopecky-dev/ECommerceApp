using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Application.DTOs.Brand;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Services;
using ProductService.Domain.Entities;
using System.Reflection;

namespace ProductService.UnitTests.Services
{
    public class BrandServiceTests
    {
        private static BrandService CreateService(
            Mock<IBrandRepository> brandRepositoryMock,
            Mock<IProductRepository>? productRepositoryMock = null,
            Mock<IProductReviewRepository>? productReviewRepositoryMock = null)
        {
            return new BrandService(
                brandRepositoryMock.Object,
                (productRepositoryMock ?? new Mock<IProductRepository>()).Object,
                (productReviewRepositoryMock ?? new Mock<IProductReviewRepository>()).Object,
                new Mock<ILogger<BrandService>>().Object
            );
        }

        private static void AddProducts(Brand brand, IEnumerable<Product> products)
        {
            var field = typeof(Brand).GetField("_products", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (List<Product>)field!.GetValue(brand)!;
            list.AddRange(products);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllBrandsAsync_ReturnsBrandDtoList_WhenExists()
        {
            List<Brand> brands =
            [
                Brand.Create("Apple", "Tech company"),
                Brand.Create("ASUS", "Tech company")
            ];

            Mock<IBrandRepository> brandRepositoryMock = new();
            brandRepositoryMock
                .Setup(b => b.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(brands);

            var service = CreateService(brandRepositoryMock);

            IReadOnlyList<BrandDto> result = await service.GetAllBrandsAsync();

            result.Should().HaveCount(2);
            brandRepositoryMock.Verify(b => b.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllBrandsAsync_ReturnsEmptyList_WhenNotExists()
        {
            Mock<IBrandRepository> brandRepositoryMock = new();
            brandRepositoryMock
                .Setup(b => b.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(brandRepositoryMock);

            IReadOnlyList<BrandDto> result = await service.GetAllBrandsAsync();

            result.Should().BeEmpty();
            brandRepositoryMock.Verify(b => b.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetBrandByIdAsync_ReturnsBrandDto_WhenExists()
        {
            Brand brand = Brand.Create("Apple", "Tech company");
            Guid brandId = brand.Id;

            Mock<IBrandRepository> brandRepositoryMock = new();
            brandRepositoryMock
                .Setup(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            var service = CreateService(brandRepositoryMock);

            BrandDto? result = await service.GetBrandByIdAsync(brandId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(brandId);
            result.Title.Should().Be("Apple");
            brandRepositoryMock.Verify(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetBrandByIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();

            Mock<IBrandRepository> brandRepositoryMock = new();
            brandRepositoryMock
                .Setup(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Brand?)null);

            var service = CreateService(brandRepositoryMock);

            BrandDto? result = await service.GetBrandByIdAsync(brandId);

            result.Should().BeNull();
            brandRepositoryMock.Verify(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateBrandAsync_ReturnsBrandDto()
        {
            CreateUpdateBrandDto createDto = new() { Title = "Apple", Description = "Tech company" };

            Mock<IBrandRepository> brandRepositoryMock = new();
            brandRepositoryMock
                .Setup(b => b.AddAsync(It.IsAny<Brand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            brandRepositoryMock
                .Setup(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(brandRepositoryMock);

            BrandDto result = await service.CreateBrandAsync(createDto);

            result.Should().NotBeNull();
            result.Title.Should().Be(createDto.Title);
            result.Description.Should().Be(createDto.Description);

            brandRepositoryMock.Verify(b => b.AddAsync(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Once);
            brandRepositoryMock.Verify(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateBrandAsync_ReturnsBrandDto_WhenExists()
        {
            Brand brand = Brand.Create("Apple", "Tech company");
            Guid brandId = brand.Id;
            CreateUpdateBrandDto updateDto = new() { Title = "ASUS", Description = "Another tech company" };

            Mock<IBrandRepository> brandRepositoryMock = new();
            brandRepositoryMock
                .Setup(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            brandRepositoryMock
                .Setup(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(brandRepositoryMock);

            BrandDto? result = await service.UpdateBrandAsync(brandId, updateDto);

            result.Should().NotBeNull();
            result!.Title.Should().Be(updateDto.Title);
            result.Description.Should().Be(updateDto.Description);

            brandRepositoryMock.Verify(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
            brandRepositoryMock.Verify(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateBrandAsync_ReturnsNull_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();
            CreateUpdateBrandDto updateDto = new() { Title = "ASUS", Description = "Company" };

            Mock<IBrandRepository> brandRepositoryMock = new();
            brandRepositoryMock
                .Setup(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Brand?)null);

            var service = CreateService(brandRepositoryMock);

            BrandDto? result = await service.UpdateBrandAsync(brandId, updateDto);

            result.Should().BeNull();

            brandRepositoryMock.Verify(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
            brandRepositoryMock.Verify(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteBrandAsync_ReturnsTrue_WhenExists()
        {
            Brand brand = Brand.Create("Apple", "Tech company");
            Guid brandId = brand.Id;

            Mock<IBrandRepository> brandRepositoryMock = new();
            brandRepositoryMock
                .Setup(b => b.FindBrandByIdWithIncludes(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            brandRepositoryMock
                .Setup(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(brandRepositoryMock);

            bool result = await service.DeleteBrandAsync(brandId);

            result.Should().BeTrue();

            brandRepositoryMock.Verify(b => b.FindBrandByIdWithIncludes(brandId, It.IsAny<CancellationToken>()), Times.Once);
            brandRepositoryMock.Verify(b => b.Remove(brand), Times.Once);
            brandRepositoryMock.Verify(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteBrandAsync_ReturnsFalse_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();

            Mock<IBrandRepository> brandRepositoryMock = new();
            brandRepositoryMock
                .Setup(b => b.FindBrandByIdWithIncludes(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Brand?)null);

            var service = CreateService(brandRepositoryMock);

            bool result = await service.DeleteBrandAsync(brandId);

            result.Should().BeFalse();

            brandRepositoryMock.Verify(b => b.FindBrandByIdWithIncludes(brandId, It.IsAny<CancellationToken>()), Times.Once);
            brandRepositoryMock.Verify(b => b.Remove(It.IsAny<Brand>()), Times.Never);
            brandRepositoryMock.Verify(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}