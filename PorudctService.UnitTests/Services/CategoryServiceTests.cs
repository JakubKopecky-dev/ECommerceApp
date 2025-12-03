using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Application.DTOs.Category;
using ProductService.Application.DTOs.Product;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Services;
using ProductService.Domain.Entities;

namespace ProductService.UnitTests.Services
{
    public class CategoryServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCategoriesAsync_ReturnsCategoryDtoList_WhenExists()
        {
            List<Category> categories =
            [
                new() {Id = Guid.NewGuid(), Title = "Computer"},
                new() {Id = Guid.NewGuid(),Title = "Phone" }
            ];

            List<CategoryDto> expectedDto =
            [
                new() {Id = categories[0].Id, Title = "Computer"},
                new() {Id = categories[1].Id, Title = "Phone"}
            ];

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            categoryRepositoryMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            mapperMock
                .Setup(m => m.Map<List<CategoryDto>>(categories))
                .Returns(expectedDto);

            CategoryService service = new(
                categoryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CategoryService>>().Object
                );


            IReadOnlyList<CategoryDto> result = await service.GetAllCategoriesAsync();

            result.Should().BeEquivalentTo(expectedDto);

            categoryRepositoryMock.Verify(c => c.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<CategoryDto>>(categories), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllCategoriesAsync_ReturnsEmptyList_WhenNotExists()
        {
            List<Category> categories = [];
            List<CategoryDto> expectedDto = [];

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            categoryRepositoryMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            mapperMock
                .Setup(m => m.Map<List<CategoryDto>>(categories))
                .Returns(expectedDto);

            CategoryService service = new(
                categoryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CategoryService>>().Object
                );


            IReadOnlyList<CategoryDto> result = await service.GetAllCategoriesAsync();

            result.Should().BeEquivalentTo(expectedDto);

            categoryRepositoryMock.Verify(c => c.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<CategoryDto>>(categories), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCategoryByIdAsync_ReturnsCategoryDto_WhenExists()
        {
            Guid categoryId = Guid.NewGuid();

            Category category = new() { Id = categoryId, Title = "Phone" };
            CategoryDto expectedDto = new() { Id = category.Id, Title = "Phone" };

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            categoryRepositoryMock
                .Setup(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            mapperMock
                .Setup(m => m.Map<CategoryDto>(category))
                .Returns(expectedDto);

            CategoryService service = new(
                categoryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CategoryService>>().Object);


            CategoryDto? result = await service.GetCategoryByIdAsync(categoryId);

            result.Should().BeEquivalentTo(expectedDto);

            categoryRepositoryMock.Verify(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CategoryDto>(category), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetCategoryByIdAsync_ReturnsNull_WhenNotExists()
        {
            Guid categoryId = Guid.NewGuid();

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            categoryRepositoryMock
                .Setup(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            CategoryService service = new(
                categoryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CategoryService>>().Object);


            CategoryDto? result = await service.GetCategoryByIdAsync(categoryId);

            result.Should().BeNull();

            categoryRepositoryMock.Verify(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CategoryDto>(It.IsAny<Category>()), Times.Never);

        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateCategoryAsync_ReturnsCategoryDto()
        {
            CreateUpdateCategoryDto createDto = new() { Title = "Phone" };

            Category category = new() { Id = Guid.Empty, Title = createDto.Title, CreatedAt = DateTime.UtcNow };
            Category createdCategory = new() { Id = Guid.NewGuid(), Title = category.Title, CreatedAt = category.CreatedAt };
            CategoryDto expectedDto = new() { Id = createdCategory.Id, Title = createdCategory.Title, CreatedAt = createdCategory.CreatedAt };

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<Category>(createDto))
                .Returns(category);

            categoryRepositoryMock
                .Setup(c => c.InsertAsync(category, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdCategory);

            mapperMock
                .Setup(m => m.Map<CategoryDto>(createdCategory))
                .Returns(expectedDto);

            CategoryService service = new(
                categoryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CategoryService>>().Object
                );


            CategoryDto result = await service.CreateCategoryAsync(createDto);

            result.Should().BeEquivalentTo(expectedDto);

            mapperMock.Verify(m => m.Map<Category>(createDto), Times.Once);
            categoryRepositoryMock.Verify(c => c.InsertAsync(category, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CategoryDto>(createdCategory), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCategoryAsync_ReturnsCategoryDto_WhenExists()
        {
            Guid categoryId = Guid.NewGuid();
            CreateUpdateCategoryDto updateDto = new() { Title = "Computer" };

            Category categoryDb = new() { Id = categoryId, Title = "Phone", UpdatedAt = DateTime.UtcNow };
            CategoryDto expectedDto = new() { Id = categoryId, Title = updateDto.Title, UpdatedAt = categoryDb.UpdatedAt };

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            categoryRepositoryMock
                .Setup(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoryDb);

            mapperMock
                .Setup(m => m.Map<CreateUpdateCategoryDto, Category>(updateDto,categoryDb))
                .Returns(categoryDb);

            categoryRepositoryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<CategoryDto>(categoryDb))
                .Returns(expectedDto);

            CategoryService service = new(
                categoryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CategoryService>>().Object
                );


            CategoryDto? result = await service.UpdateCategoryAsync(categoryId, updateDto);

            result.Should().BeEquivalentTo(expectedDto);

            categoryRepositoryMock.Verify(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CreateUpdateCategoryDto, Category>(updateDto, categoryDb), Times.Once);
            categoryRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CategoryDto>(categoryDb), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateCategoryAsync_ReturnsNull_WhenNotExists()
        {
            Guid categoryId = Guid.NewGuid();
            CreateUpdateCategoryDto updateDto = new() { Title = "Computer" };

            Mock<ICategoryRepository> categoryRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            categoryRepositoryMock
                .Setup(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            CategoryService service = new(
                categoryRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CategoryService>>().Object
                );


            CategoryDto? result = await service.UpdateCategoryAsync(categoryId, updateDto);

            result.Should().BeNull();

            categoryRepositoryMock.Verify(c => c.FindByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CreateUpdateCategoryDto, Category>(updateDto, It.IsAny<Category>()), Times.Never);
            categoryRepositoryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<CategoryDto>(It.IsAny<Category>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCategoryAsync_ReturnsCategoryDto_WhenExists()
        {
            Guid categoryId = Guid.NewGuid();

            Category category = new() { Id = categoryId, Title = "Phone", Products = [] };
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product = new() { Id = Guid.NewGuid(), Title = "iPhone 16", Brand = brand };
            brand.Products.Add(product);
            category.Products.Add(product);
            CategoryDto expectedDto = new() { Id = category.Id, Title = category.Title };

            Mock<ICategoryRepository> categoryRepisotryMock = new();
            Mock<IMapper> mapperMock = new();

            categoryRepisotryMock
                .Setup(c => c.FindCategoryByIdWithIncludeProductsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            mapperMock
                .Setup(m => m.Map<CategoryDto>(category))
                .Returns(expectedDto);

            categoryRepisotryMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            CategoryService service = new(
                categoryRepisotryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CategoryService>>().Object
                );


            CategoryDto? result = await service.DeleteCategoryAsync(categoryId);

            result.Should().BeEquivalentTo(expectedDto);

            categoryRepisotryMock.Verify(c => c. FindCategoryByIdWithIncludeProductsAsync(categoryId, It.IsAny<CancellationToken>()),Times.Once);
            mapperMock.Verify(m => m.Map<CategoryDto>(category), Times.Once);
            categoryRepisotryMock.Verify(c => c.Remove(category), Times.Once);
            categoryRepisotryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteCategoryAsync_ReturnsNull_WhenNotExists()
        {
            Guid categoryId = Guid.NewGuid();

            Mock<ICategoryRepository> categoryRepisotryMock = new();
            Mock<IMapper> mapperMock = new();

            categoryRepisotryMock
                .Setup(c => c.FindCategoryByIdWithIncludeProductsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            CategoryService service = new(
                categoryRepisotryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<CategoryService>>().Object
                );


            CategoryDto? result = await service.DeleteCategoryAsync(categoryId);

            result.Should().BeNull();

            categoryRepisotryMock.Verify(c => c.FindCategoryByIdWithIncludeProductsAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<CategoryDto>(It.IsAny<Category>()), Times.Never);
            categoryRepisotryMock.Verify(c => c.Remove(It.IsAny<Category>()), Times.Never);
            categoryRepisotryMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



    }
}
