using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductService.Api.Controllers;
using ProductService.Application.DTOs.Brand;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;

namespace ProductService.UnitTests.Controllers
{
    public class BrandControllerTests
    {

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllBrands_ReturnsBrandDtoList_WhenExists()
        {
            IReadOnlyList<BrandDto> expectedDto =
            [
                new() {Id = Guid.NewGuid(), Title = "Apple"},
                new() {Id = Guid.NewGuid(), Title = "Asus" }
            ];

            Mock<IBrandService> brandServiceMock = new();

            brandServiceMock
                .Setup(b => b.GetAllBrandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            BrandController controller = new(brandServiceMock.Object);


            IReadOnlyList<BrandDto> result = await controller.GetAllBrands(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            brandServiceMock.Verify(b => b.GetAllBrandsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllBrands_ReturnsEmptyList_WhenNotExists()
        {
            IReadOnlyList<BrandDto> expectedDto = [];

            Mock<IBrandService> brandServiceMock = new();

            brandServiceMock
                .Setup(b => b.GetAllBrandsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            BrandController controller = new(brandServiceMock.Object);


            IReadOnlyList<BrandDto> result = await controller.GetAllBrands(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            brandServiceMock.Verify(b => b.GetAllBrandsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetBrand_ReturnsOk_WhenExists()
        {
            Guid brandId = Guid.NewGuid();

            BrandDto expectedDto = new() { Id = brandId, Title = "Apple" };

            Mock<IBrandService> brandServiceMock = new();

            brandServiceMock
                .Setup(b => b.GetBrandByIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            BrandController controller = new(brandServiceMock.Object);

            var result = await controller.GetBrand(brandId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            brandServiceMock.Verify(b => b.GetBrandByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetBrand_ReturnsNotFound_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();

            Mock<IBrandService> brandServiceMock = new();

            brandServiceMock
                .Setup(b => b.GetBrandByIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((BrandDto?)null);

            BrandController controller = new(brandServiceMock.Object);

            var result = await controller.GetBrand(brandId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            brandServiceMock.Verify(b => b.GetBrandByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateBrand_ReturnsCreatedAtAction_WithBrandDto()
        {
            CreateUpdateBrandDto createDto = new() { Title = "Apple", Description = "Company" };

            BrandDto expectedDto = new() { Id = Guid.NewGuid(), Title = "Apple", Description = "Company" };

            Mock<IBrandService> brandServiceMock = new();

            brandServiceMock
                .Setup(b => b.CreateBrandAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            BrandController controller = new(brandServiceMock.Object);


            var result = await controller.CreateBrand(createDto, It.IsAny<CancellationToken>());
            var createdResult = result as CreatedAtActionResult;

            createdResult!.ActionName.Should().Be(nameof(BrandController.GetBrand));
            createdResult.RouteValues!["brandId"].Should().Be(expectedDto.Id);
            createdResult.Value.Should().BeEquivalentTo(expectedDto);

            brandServiceMock.Verify(b => b.CreateBrandAsync(createDto,It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateBrand_ReturnsOk_WhenExists()
        {
            Guid brandId = Guid.NewGuid();
            CreateUpdateBrandDto updateDto = new() { Title = "Apple" };
            BrandDto expectedDto = new() { Id = brandId, Title = "Apple" };

            Mock<IBrandService> brandServiceMock = new();

            brandServiceMock
                .Setup(b => b.UpdateBrandAsync(brandId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            BrandController controller = new(brandServiceMock.Object);

            var result = await controller.UpdateBrand(brandId, updateDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            brandServiceMock.Verify(b => b.UpdateBrandAsync(brandId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateBrand_ReturnsNotFoud_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();
            CreateUpdateBrandDto updateDto = new() { Title = "Apple" };
            BrandDto expectedDto = new() { Id = brandId, Title = "Apple" };

            Mock<IBrandService> brandServiceMock = new();

            brandServiceMock
                .Setup(b => b.UpdateBrandAsync(brandId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((BrandDto?)null);

            BrandController controller = new(brandServiceMock.Object);

            var result = await controller.UpdateBrand(brandId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            brandServiceMock.Verify(b => b.UpdateBrandAsync(brandId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeletetBrand_ReturnsOk_WhenExists()
        {
            Guid brandId = Guid.NewGuid();

            BrandDto expectedDto = new() { Id = brandId, Title = "Apple" };

            Mock<IBrandService> brandServiceMock = new();

            brandServiceMock
                .Setup(b => b.DeleteBrandAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            BrandController controller = new(brandServiceMock.Object);

            var result = await controller.DeleteBrand(brandId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            brandServiceMock.Verify(b => b.DeleteBrandAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeletetBrand_ReturnNull_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();

            BrandDto expectedDto = new() { Id = brandId, Title = "Apple" };

            Mock<IBrandService> brandServiceMock = new();

            brandServiceMock
                .Setup(b => b.DeleteBrandAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((BrandDto?)null);

            BrandController controller = new(brandServiceMock.Object);

            var result = await controller.DeleteBrand(brandId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            brandServiceMock.Verify(b => b.DeleteBrandAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
