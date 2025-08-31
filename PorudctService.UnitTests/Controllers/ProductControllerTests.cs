using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductService.Api.Controllers;
using ProductService.Application.DTOs.Product;
using ProductService.Application.Interfaces.Services;

namespace ProductService.UnitTests.Controllers
{
    public class ProductControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProducts_ReturnsProductDtoList_WhenExists()
        {
            IReadOnlyList<ProductDto> expectedDto =
            [
                new() {Id = Guid.NewGuid(), Title = "iPhone 16"},
                new() {Id = Guid.NewGuid(), Title = "MacBook Pro"}
            ];

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            IReadOnlyList<ProductDto> result = await controller.GetAllProducts(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetAllProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProducts_ReturnsEmptyList_WhenNotExists()
        {
            IReadOnlyList<ProductDto> expectedDto = [];

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            IReadOnlyList<ProductDto> result = await controller.GetAllProducts(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetAllProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProduct_ReturnsOk_WhenExists()
        {
            Guid productId = Guid.NewGuid();

            ProductExtendedDto expectedDto = new() { Id = productId, Title = "iPhone 16" };

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            var result = await controller.GetProduct(productId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProduct_ReturnsNotFound_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductExtendedDto?)null);

            ProductController controller = new(productServiceMock.Object);


            var result = await controller.GetProduct(productId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            productServiceMock.Verify(p => p.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateProduct_ReturnsCreatedAtAction_WithProductExtendedDto()
        {
            CreateProductDto createDto = new() { Title = "iPhone 16", BrandId = Guid.NewGuid() };

            ProductExtendedDto expectedDto = new() { Id = Guid.NewGuid(), Title = createDto.Title, BrandId = createDto.BrandId };

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.CreateProductAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            var result = await controller.CreateProduct(createDto, It.IsAny<CancellationToken>());
            var createdResult = result as CreatedAtActionResult;

            createdResult!.ActionName.Should().Be(nameof(ProductController.GetProduct));
            createdResult.RouteValues!["productId"].Should().Be(expectedDto.Id);
            createdResult.Value.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.CreateProductAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProduct_ReturnsOk_WhenExists()
        {
            Guid productId = Guid.NewGuid();
            UpdateProductDto updateDto = new() { Title = "iPhone 16" };

            ProductExtendedDto expectedDto = new() { Id = productId, Title = updateDto.Title };

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.UpdateProductAsync(productId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            var result = await controller.UpdateProduct(productId, updateDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.UpdateProductAsync(productId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProduct_ReturnsNotFound_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();
            UpdateProductDto updateDto = new() { Title = "iPhone 16" };

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.UpdateProductAsync(productId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductExtendedDto?)null);

            ProductController controller = new(productServiceMock.Object);


            var result = await controller.UpdateProduct(productId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            productServiceMock.Verify(p => p.UpdateProductAsync(productId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProduct_ReturnsOk_WhenExists()
        {
            Guid productId = Guid.NewGuid();
            ProductDto expectedDto = new() { Id = productId, Title = "MacBook Pro" };

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.DeleteProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);

            var result = await controller.DeleteProduct(productId, It.IsAny<CancellationToken>());


            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.DeleteProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProduct_ReturnsNotFound_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.DeleteProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            ProductController controller = new(productServiceMock.Object);

            var result = await controller.DeleteProduct(productId, It.IsAny<CancellationToken>());


            result.Should().BeOfType<NotFoundResult>();

            productServiceMock.Verify(p => p.DeleteProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task InactivateProduct_ReturnsOk_WhenExists()
        {
            Guid productId = Guid.NewGuid();
            ProductDto expectedDto = new() { Id = productId, Title = "iPhone 16" };

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.InactivateProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            var result = await controller.InactivateProduct(productId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.InactivateProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task InactivateProduct_ReturnsNotFound_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.InactivateProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            ProductController controller = new(productServiceMock.Object);


            var result = await controller.InactivateProduct(productId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            productServiceMock.Verify(p => p.InactivateProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ActivateProduct_ReturnsOk_WhenExists()
        {
            Guid productId = Guid.NewGuid();
            ProductDto expectedDto = new() { Id = productId, Title = "MacBook Pro" };

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.ActivateProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            var result = await controller.ActivateProduct(productId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.ActivateProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task ActivateProduct_ReturnsNotFound_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.ActivateProductAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            ProductController controller = new(productServiceMock.Object);


            var result = await controller.ActivateProduct(productId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            productServiceMock.Verify(p => p.ActivateProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByBrandId_ReturnsProductDtoList_WhenExists()
        {
            Guid brandId = Guid.NewGuid();

            IReadOnlyList<ProductDto> expectedDto =
            [
                new() { Id = Guid.NewGuid(), Title = "iPhone 16", BrandId = brandId },
                new() { Id = Guid.NewGuid(), Title = "iPhone 15", BrandId = brandId },
            ];

            Mock<IProductService> productServiceMock = new();
            productServiceMock
                .Setup(p => p.GetAllProductsByBrandIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            IReadOnlyList<ProductDto> result = await controller.GetAllProductsByBrandId(brandId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetAllProductsByBrandIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByBrandId_ReturnsEmptyList_WhenNotExists()
        {
            Guid brandId = Guid.NewGuid();

            IReadOnlyList<ProductDto> expectedDto = [];

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetAllProductsByBrandIdAsync(brandId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            IReadOnlyList<ProductDto> result = await controller.GetAllProductsByBrandId(brandId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetAllProductsByBrandIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByCategories_ReturnsProductDtoList_WhenExists()
        {
            List<string> categories = ["Phones", "Laptops"];

            IReadOnlyList<ProductDto> expectedDto =
            [
                new() { Id = Guid.NewGuid(), Title = "iPhone 16" },
                new() { Id = Guid.NewGuid(), Title = "MacBook Pro" }
            ];

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            IReadOnlyList<ProductDto> result = await controller.GetAllProductsByCategories(categories, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsByCategories_ReturnsProductDtoList_WhenNotExists()
        {
            List<string> categories = ["Phones", "Laptops"];

            IReadOnlyList<ProductDto> expectedDto = [];

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            IReadOnlyList<ProductDto> result = await controller.GetAllProductsByCategories(categories, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetAllProductsByCategoriesAsync(categories, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllActiveProducts_ReturnsProductDtoList_WhenExists()
        {
            IReadOnlyList<ProductDto> expectedDto =
            [
                new() { Id = Guid.NewGuid(), Title = "iPhone 16", IsActive = true },
                new() {Id = Guid.NewGuid(), Title = "MacBook Pro", IsActive = true }
            ];

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            IReadOnlyList<ProductDto> result = await controller.GetAllActiveProducts(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllActiveProducts_ReturnsEmptList_WhenNotExists()
        {
            IReadOnlyList<ProductDto> expectedDto = [];

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            IReadOnlyList<ProductDto> result = await controller.GetAllActiveProducts(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetAllActiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllInactiveProducts_ReturnsProductDtoList_WhenExists()
        {
            IReadOnlyList<ProductDto> expectedDto =
            [
                new() { Id = Guid.NewGuid(), Title = "iPhone 16", IsActive = false },
                new() {Id = Guid.NewGuid(), Title = "MacBook Pro", IsActive = false }            
            ];

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            IReadOnlyList<ProductDto> result = await controller.GetAllInactiveProducts(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllInactiveProducts_ReturnsEmptyList_WhenNotExists()
        {
            IReadOnlyList<ProductDto> expectedDto = [];

            Mock<IProductService> productServiceMock = new();

            productServiceMock
                .Setup(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductController controller = new(productServiceMock.Object);


            IReadOnlyList<ProductDto> result = await controller.GetAllInactiveProducts(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            productServiceMock.Verify(p => p.GetAllInactiveProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
