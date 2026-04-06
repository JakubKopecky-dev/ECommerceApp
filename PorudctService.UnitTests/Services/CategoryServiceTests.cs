using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Application.DTOs.Category;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Services;
using ProductService.Domain.Entities;

namespace ProductService.UnitTests.Services
{
    public class CategoryServiceTests
    {
        private static CategoryService CreateService(Mock<ICategoryRepository> categoryRepositoryMock)
        {
            return new CategoryService(
                categoryRepositoryMock.Object,
                new Mock<ILogger<CategoryService>>().Object
            );
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCategoriesAsync_ReturnsCategoryDtoList_WhenExists()
        {
            List<Category> categories =
            [
                Category.Create("Computer"),
                Category.Create("Phone")
            ];

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            categoryRepositoryMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            var service = CreateService(categoryRepositoryMock);

            IReadOnlyList<CategoryDto> result = await service.GetAllCategoriesAsync();

            result.Should().HaveCount(2);
            categoryRepositoryMock.Verify(c => c.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCategoriesAsync_ReturnsEmptyList_WhenNotExists()
        {
            Mock<ICategoryRepository> categoryRepositoryMock = new();
            categoryRepositoryMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(categoryRepositoryMock);

            IReadOnlyList<CategoryDto> result = await service.GetAllCategoriesAsync();

            result.Should().BeEmpty();
            categoryRepositoryMock.Verify(c => c.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCategoryByIdAsync_ReturnsCategoryDto_WhenExists()
        {
            Category category = Category.Create("Phone");
            Guid categoryId = category.Id;

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            categoryRepositoryMock
                .Setup(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var service = CreateService(categoryRepositoryMock);

            CategoryDto? result = await service.GetCategoryByIdAsync(categoryId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(categoryId);
            result.Title.Should().Be("Phone");
            categoryRepositoryMock.Verify(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCategoryByIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid categoryId = Guid.NewGuid();

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            categoryRepositoryMock
                .Setup(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var service = CreateService(categoryRepositoryMock);

            CategoryDto? result = await service.GetCategoryByIdAsync(categoryId);

            result.Should().BeNull();
            categoryRepositoryMock.Verify(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCategoryAsync_ReturnsCategoryDto()
        {
            CreateUpdateCategoryDto createDto = new() { Title = "Phone" };

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            categoryRepositoryMock
                .Setup(c => c.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            categoryRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(categoryRepositoryMock);

            CategoryDto result = await service.CreateCategoryAsync(createDto);

            result.Should().NotBeNull();
            result.Title.Should().Be(createDto.Title);

            categoryRepositoryMock.Verify(c => c.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
            categoryRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCategoryAsync_ReturnsCategoryDto_WhenExists()
        {
            Category category = Category.Create("Phone");
            Guid categoryId = category.Id;
            CreateUpdateCategoryDto updateDto = new() { Title = "Computer" };

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            categoryRepositoryMock
                .Setup(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            categoryRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(categoryRepositoryMock);

            CategoryDto? result = await service.UpdateCategoryAsync(categoryId, updateDto);

            result.Should().NotBeNull();
            result!.Title.Should().Be(updateDto.Title);

            categoryRepositoryMock.Verify(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            categoryRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCategoryAsync_ReturnsNull_WhenNotExists()
        {
            Guid categoryId = Guid.NewGuid();
            CreateUpdateCategoryDto updateDto = new() { Title = "Computer" };

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            categoryRepositoryMock
                .Setup(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var service = CreateService(categoryRepositoryMock);

            CategoryDto? result = await service.UpdateCategoryAsync(categoryId, updateDto);

            result.Should().BeNull();

            categoryRepositoryMock.Verify(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            categoryRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCategoryAsync_ReturnsTrue_WhenExists()
        {
            Category category = Category.Create("Phone");
            Guid categoryId = category.Id;

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            categoryRepositoryMock
                .Setup(c => c.FindCategoryByIdWithIncludeProductsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            categoryRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(categoryRepositoryMock);

            bool result = await service.DeleteCategoryAsync(categoryId);

            result.Should().BeTrue();

            categoryRepositoryMock.Verify(c => c.FindCategoryByIdWithIncludeProductsAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            categoryRepositoryMock.Verify(c => c.Remove(category), Times.Once);
            categoryRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCategoryAsync_ReturnsFalse_WhenNotExists()
        {
            Guid categoryId = Guid.NewGuid();

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            categoryRepositoryMock
                .Setup(c => c.FindCategoryByIdWithIncludeProductsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var service = CreateService(categoryRepositoryMock);

            bool result = await service.DeleteCategoryAsync(categoryId);

            result.Should().BeFalse();

            categoryRepositoryMock.Verify(c => c.FindCategoryByIdWithIncludeProductsAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            categoryRepositoryMock.Verify(c => c.Remove(It.IsAny<Category>()), Times.Never);
            categoryRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}