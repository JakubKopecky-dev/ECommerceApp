using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Application.DTOs.Brand;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Services;
using ProductService.Domain.Entities;

namespace ProductService.UnitTests.Services
{
    public class BrandServiceTests
    {

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllBrandsAsync_ReturnsBrandDtoList_WhenExists()
        {
            List<Brand> brands =
            [
                new() {Id =Guid.NewGuid(),Title = "Apple"},
                new() {Id = Guid.NewGuid(), Title = "ASUS"}
            ];

            List<BrandDto> expectedDto =
            [
                new() {Id = brands[0].Id, Title = "Apple"},
                new() {Id = brands[1].Id,Title = "ASUS"}
            ];

            Mock<IBrandRepository> brandRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            brandRepositoryMock
                .Setup(b => b.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(brands);

            mapperMock
                .Setup(m => m.Map<List<BrandDto>>(brands))
                .Returns(expectedDto);

            BrandService service = new(
                brandRepositoryMock.Object,
                new Mock<IProductRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<BrandService>>().Object
                );


            IReadOnlyList<BrandDto> result = await service.GetAllBrandsAsync();

            result.Should().BeEquivalentTo(expectedDto);

            brandRepositoryMock.Verify(b => b.GetAllAsync(It.IsAny<CancellationToken>()));
            mapperMock.Verify(m => m.Map<List<BrandDto>>(brands), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllBrandsAsync_ReturnsEmptyList_WhenNotExists()
        {
            List<Brand> brands = [];
            List<BrandDto> expectedDto = [];

            Mock<IBrandRepository> brandRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            brandRepositoryMock
                .Setup(b => b.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(brands);

            mapperMock
                .Setup(m => m.Map<List<BrandDto>>(brands))
                .Returns(expectedDto);

            BrandService service = new(
                brandRepositoryMock.Object,
                new Mock<IProductRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<BrandService>>().Object
                );


            IReadOnlyList<BrandDto> result = await service.GetAllBrandsAsync();

            result.Should().BeEquivalentTo(expectedDto);

            brandRepositoryMock.Verify(b => b.GetAllAsync(It.IsAny<CancellationToken>()));
            mapperMock.Verify(m => m.Map<List<BrandDto>>(brands), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetBrandByIdAsync_ReturnsBrandDto_WhenExists()
        {
            Guid brandId = Guid.NewGuid();

            Brand brand = new() { Id = brandId, Title = "Apple" };
            BrandDto expectedDto = new() { Id = brandId, Title = "Apple" };

            Mock<IBrandRepository> brandRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            brandRepositoryMock
                .Setup(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            mapperMock
                .Setup(m => m.Map<BrandDto>(brand))
                .Returns(expectedDto);

            BrandService service = new(
                brandRepositoryMock.Object,
                new Mock<IProductRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<BrandService>>().Object
                );


            BrandDto? result = await service.GetBrandByIdAsync(brandId);

            result.Should().BeEquivalentTo(expectedDto);

            brandRepositoryMock.Verify(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<BrandDto>(brand), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetBrandByIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();

            Mock<IBrandRepository> brandRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            brandRepositoryMock
                .Setup(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Brand?)null);

            BrandService service = new(
                brandRepositoryMock.Object,
                new Mock<IProductRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<BrandService>>().Object
                );


            BrandDto? result = await service.GetBrandByIdAsync(brandId);

            result.Should().BeNull();

            brandRepositoryMock.Verify(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<BrandDto>(It.IsAny<Brand>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateBrandAsync_RetursBrandDto()
        {
            CreateUpdateBrandDto createDto = new() { Title = "Apple" };

            Brand brand = new() { Id = Guid.Empty, Title = "Apple", CreatedAt = DateTime.UtcNow };
            Brand createdBrand = new() { Id = Guid.NewGuid(), Title = "Apple", CreatedAt = brand.CreatedAt };
            BrandDto expectedDto = new() { Id = createdBrand.Id, Title = "Apple", CreatedAt = brand.CreatedAt };

            Mock<IMapper> mapperMock = new();
            Mock<IBrandRepository> brandRepositoryMock = new();

            mapperMock
                .Setup(m => m.Map<Brand>(createDto))
                .Returns(brand);

            brandRepositoryMock
                .Setup(b => b.InsertAsync(brand, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdBrand);

            mapperMock
                .Setup(m => m.Map<BrandDto>(createdBrand))
                .Returns(expectedDto);

            BrandService service = new(
                brandRepositoryMock.Object,
                new Mock<IProductRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<BrandService>>().Object
                );


            BrandDto result = await service.CreateBrandAsync(createDto);

            result.Should().BeEquivalentTo(expectedDto);

            mapperMock.Verify(m => m.Map<Brand>(createDto), Times.Once);
            brandRepositoryMock.Verify(b => b.InsertAsync(brand, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<BrandDto>(createdBrand), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateBrandAsync_ReturnsBrandDto_WhenExists()
        {
            Guid brandId = Guid.NewGuid();
            CreateUpdateBrandDto updateDto = new() { Title = "Asus", Description = "Company" };

            Brand brandDb = new() { Id = brandId, Title = "Apple", Description = "Comp", UpdatedAt = DateTime.UtcNow };
            BrandDto expectedDto = new() { Id = brandId, Title = updateDto.Title, Description = updateDto.Description, UpdatedAt = brandDb.UpdatedAt };

            Mock<IBrandRepository> brandRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            brandRepositoryMock
                .Setup(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brandDb);

            mapperMock
                .Setup(m => m.Map<CreateUpdateBrandDto, Brand>(updateDto, brandDb))
                .Returns(brandDb);

            brandRepositoryMock
                .Setup(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<BrandDto>(brandDb))
                .Returns(expectedDto);

            BrandService service = new(
                brandRepositoryMock.Object,
                new Mock<IProductRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<BrandService>>().Object
                );


            BrandDto? result = await service.UpdateBrandAsync(brandId, updateDto);

            result.Should().BeEquivalentTo(expectedDto);

            brandRepositoryMock.Verify(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CreateUpdateBrandDto, Brand>(updateDto, brandDb), Times.Once);
            brandRepositoryMock.Verify(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<BrandDto>(brandDb), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateBrandAsync_ReturnsNull_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();
            CreateUpdateBrandDto updateDto = new() { Title = "Asus", Description = "Company" };

            Mock<IBrandRepository> brandRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            brandRepositoryMock
                .Setup(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Brand?)null);

            BrandService service = new(
                brandRepositoryMock.Object,
                new Mock<IProductRepository>().Object,
                new Mock<IProductReviewRepository>().Object,
                mapperMock.Object,
                new Mock<ILogger<BrandService>>().Object
                );


            BrandDto? result = await service.UpdateBrandAsync(brandId, updateDto);

            result.Should().BeNull();

            brandRepositoryMock.Verify(b => b.FindByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CreateUpdateBrandDto, Brand>(updateDto, It.IsAny<Brand>()), Times.Never);
            brandRepositoryMock.Verify(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<BrandDto>(It.IsAny<Brand>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteBrandAsync_ReturnsBrandDto_WhenExists()
        {
            Guid brandId = Guid.NewGuid();

            Brand brand = new() { Id = brandId, Title = "Apple" };
            Product product1 = new() { Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 17", Categories = [new() { Id = Guid.NewGuid(), Title = "Phone" }] };
            Product product2 = new() { Id = Guid.NewGuid(), Brand = brand, Title = "MacBook Air", Categories = [new() { Id = Guid.NewGuid(), Title = "Laptop" }] };
            ProductReview review1 = new() { Id = Guid.NewGuid(), Product = product1, UserId = Guid.NewGuid() };
            ProductReview review2 = new() { Id = Guid.NewGuid(), Product = product1, UserId = Guid.NewGuid() };
            product1.Reviews.Add(review1);
            product1.Reviews.Add(review2);
            brand.Products.Add(product1);
            brand.Products.Add(product2);

            BrandDto expectedDto = new() { Id = brandId, Title = "Apple" };

            Mock<IBrandRepository> brandRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IProductReviewRepository> productReviewRepositoryMock = new();

            brandRepositoryMock
                .Setup(b => b.FindBrandByIdWithIncludes(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

            mapperMock
                .Setup(m => m.Map<BrandDto>(brand))
                .Returns(expectedDto);

            productReviewRepositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            productRepositoryMock
                .Setup(p => p.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) => p);

            productRepositoryMock
                .Setup(p => p.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            brandRepositoryMock
                .Setup(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            BrandService service = new(
                brandRepositoryMock.Object,
                productRepositoryMock.Object,
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<BrandService>>().Object
                );


            BrandDto? result = await service.DeleteBrandAsync(brandId);

            result.Should().BeEquivalentTo(expectedDto);
            product1.Categories.Should().BeEmpty();
            product2.Categories.Should().BeEmpty();

            brandRepositoryMock.Verify(b => b.FindBrandByIdWithIncludes(brandId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<BrandDto>(brand), Times.Once);
            productReviewRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            productRepositoryMock.Verify(p => p.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            productRepositoryMock.Verify(p => p.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            brandRepositoryMock.Verify(b => b.Remove(brand), Times.Once);
            brandRepositoryMock.Verify(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteBrandAsync_ReturnsNull_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();

            Mock<IBrandRepository> brandRepositoryMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<IProductRepository> productRepositoryMock = new();
            Mock<IProductReviewRepository> productReviewRepositoryMock = new();

            brandRepositoryMock
                .Setup(b => b.FindBrandByIdWithIncludes(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Brand?)null);

            BrandService service = new(
                brandRepositoryMock.Object,
                productRepositoryMock.Object,
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<BrandService>>().Object
                );


            BrandDto? result = await service.DeleteBrandAsync(brandId);

            result.Should().BeNull();

            brandRepositoryMock.Verify(b => b.FindBrandByIdWithIncludes(brandId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<BrandDto>(It.IsAny<Brand>()), Times.Never);
            productReviewRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            productRepositoryMock.Verify(p => p.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
            productRepositoryMock.Verify(p => p.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            brandRepositoryMock.Verify(b => b.Remove(It.IsAny<Brand>()), Times.Never);
            brandRepositoryMock.Verify(b => b.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



    }
}
