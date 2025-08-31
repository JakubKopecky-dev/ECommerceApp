using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductService.Api.Controllers;
using ProductService.Application.DTOs.Category;
using ProductService.Application.Interfaces.Services;

namespace ProductService.UnitTests.Controllers
{
    public class CategoryControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCategories_ReturnsCategoryDtoList_WhenExists()
        {
            IReadOnlyList<CategoryDto> expectedDto =
            [
                new() {Id = Guid.NewGuid(), Title = "Phone"},
                new() {Id = Guid.NewGuid(), Title = "Laptop"}
            ];

            Mock<ICategoryService> categoryServiceMock = new();

            categoryServiceMock
                .Setup(c => c.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CategoryController controller = new(categoryServiceMock.Object);


            IReadOnlyList<CategoryDto> result = await controller.GetAllCategories(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            categoryServiceMock.Verify(c => c.GetAllCategoriesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCategories_EmptyList_WhenNotExists()
        {
            IReadOnlyList<CategoryDto> expectedDto = [];

            Mock<ICategoryService> categoryServiceMock = new();

            categoryServiceMock
                .Setup(c => c.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CategoryController controller = new(categoryServiceMock.Object);


            IReadOnlyList<CategoryDto> result = await controller.GetAllCategories(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            categoryServiceMock.Verify(c => c.GetAllCategoriesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCategory_ReturnsOk_WhenExists()
        {
            Guid categoryId = Guid.NewGuid();

            CategoryDto expectedDto = new() { Id = categoryId, Title = "Laptop" };

            Mock<ICategoryService> categoryServiceMock = new();

            categoryServiceMock
                .Setup(c => c.GetCategoryByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CategoryController controller = new(categoryServiceMock.Object);


            var result = await controller.GetCategory(categoryId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            categoryServiceMock.Verify(c => c.GetCategoryByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCategory_ReturnsNotFound_WhenNotExists()
        {
            Guid categoryId = Guid.NewGuid();

            Mock<ICategoryService> categoryServiceMock = new();

            categoryServiceMock
                .Setup(c => c.GetCategoryByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryDto?)null);

            CategoryController controller = new(categoryServiceMock.Object);


            var result = await controller.GetCategory(categoryId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            categoryServiceMock.Verify(c => c.GetCategoryByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCategory_ReturnsCreatedAtAction_WithCategoryDto()
        {
            CreateUpdateCategoryDto createDto = new() { Title = "Laptop" };

            CategoryDto expectedDto = new() { Id = Guid.NewGuid(), Title = "Laptop" };

            Mock<ICategoryService> categoryServiceMock = new();

            categoryServiceMock
                .Setup(c => c.CreateCategoryAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CategoryController controller = new(categoryServiceMock.Object);


            var result = await controller.CreateCategory(createDto, It.IsAny<CancellationToken>());
            var createdResult = result as CreatedAtActionResult;

            createdResult!.ActionName.Should().Be(nameof(CategoryController.GetCategory));
            createdResult.RouteValues!["categoryId"].Should().Be(expectedDto.Id);
            createdResult.Value.Should().BeEquivalentTo(expectedDto);

            categoryServiceMock.Verify(c => c.CreateCategoryAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCategory_ReturnsOk_WhenExists()
        {
            Guid categoryId = Guid.NewGuid();
            CreateUpdateCategoryDto updateDto = new() { Title = "Laptop" };

            CategoryDto expectedDto = new() { Id = categoryId, Title = "Laptop" };

            Mock<ICategoryService> categoryServiceMock = new();

            categoryServiceMock
                .Setup(c => c.UpdateCategoryAsync(categoryId,updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CategoryController controller = new(categoryServiceMock.Object);


            var result = await controller.UpadteCategory(categoryId,updateDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            categoryServiceMock.Verify(c => c.UpdateCategoryAsync(categoryId, updateDto,It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCategory_ReturnsNotFound_WhenNotExists()
        {
            Guid categoryId = Guid.NewGuid();
            CreateUpdateCategoryDto updateDto = new() { Title = "Laptop" };

            Mock<ICategoryService> categoryServiceMock = new();

            categoryServiceMock
                .Setup(c => c.UpdateCategoryAsync(categoryId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryDto?)null);

            CategoryController controller = new(categoryServiceMock.Object);


            var result = await controller.UpadteCategory(categoryId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            categoryServiceMock.Verify(c => c.UpdateCategoryAsync(categoryId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCategory_ReturnsOk_WhenExists()
        {
            Guid categoryId = Guid.NewGuid();

            CategoryDto expectedDto = new() { Id = categoryId, Title = "Laptop" };

            Mock<ICategoryService> categoryServiceMock = new();

            categoryServiceMock
                .Setup(c => c.DeleteCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            CategoryController controller = new(categoryServiceMock.Object);


            var result = await controller.DeleteCategory(categoryId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            categoryServiceMock.Verify(c => c.DeleteCategoryAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCategory_ReturnsNotFound_WhenNotExists()
        {
            Guid categoryId = Guid.NewGuid();

            CategoryDto expectedDto = new() { Id = categoryId, Title = "Laptop" };

            Mock<ICategoryService> categoryServiceMock = new();

            categoryServiceMock
                .Setup(c => c.DeleteCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryDto?)null);

            CategoryController controller = new(categoryServiceMock.Object);


            var result = await controller.DeleteCategory(categoryId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            categoryServiceMock.Verify(c => c.DeleteCategoryAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
