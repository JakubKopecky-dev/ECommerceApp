using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using ProductService.Application.DTOs.ProductReview;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Services;
using ProductService.Domain.Entity;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace ProductService.UnitTests.Services
{
    public class ProductReviewServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsReviewsAsync_ReturnsProductReviewDtoList_WhenExists()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product = new() { Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16" };

            List<ProductReview> reviews =
            [
                new() {Id = Guid.NewGuid(), Product = product},
                new() {Id = Guid.NewGuid(), Product = product}
            ];

            List<ProductReviewDto> expectedDto =
            [
                new() {Id = reviews[0].Id, ProductId = product.Id},
                new() {Id = reviews[1].Id, ProductId = product.Id}
            ];

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(reviews);

            mapperMock
                .Setup(m => m.Map<List<ProductReviewDto>>(reviews))
                .Returns(expectedDto);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            IReadOnlyList<ProductReviewDto> result = await service.GetAllProductReviewsAsync();

            result.Should().BeEquivalentTo(expectedDto);

            productReviewRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductReviewDto>>(reviews), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductsReviewsAsync_ReturnsEmptyList_WhenNotExists()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product = new() { Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16" };

            List<ProductReview> reviews = [];

            List<ProductReviewDto> expectedDto = [];

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(reviews);

            mapperMock
                .Setup(m => m.Map<List<ProductReviewDto>>(reviews))
                .Returns(expectedDto);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            IReadOnlyList<ProductReviewDto> result = await service.GetAllProductReviewsAsync();

            result.Should().BeEquivalentTo(expectedDto);

            productReviewRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductReviewDto>>(reviews), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProductReviewAsync_RetrunsProductReviewDto_WhenExists()
        {
            Guid reviewId = Guid.NewGuid();

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product = new() { Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16" };

            ProductReview review = new() { Id = reviewId, Product = product };
            ProductReviewDto expectedDto = new() { Id = reviewId, ProductId = product.Id };

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            mapperMock
                .Setup(m => m.Map<ProductReviewDto>(review))
                .Returns(expectedDto);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            ProductReviewDto? result = await service.GetProductReviewAsync(reviewId);

            result.Should().BeEquivalentTo(expectedDto);

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductReviewDto>(review), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProductReviewAsync_RetrunsNull_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReview?)null);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            ProductReviewDto? result = await service.GetProductReviewAsync(reviewId);

            result.Should().BeNull();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductReviewDto>(It.IsAny<ProductReview>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateProductReviewAsync_ReturnsProductReviewDto()
        {
            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product = new() { Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16" };

            Guid userId = Guid.NewGuid();
            string userName = "John";
            CreateProductReviewDto createDto = new() { ProductId = product.Id };

            ProductReview review = new() { Id = Guid.Empty, Product = product, CreatedAt = DateTime.UtcNow, UserId = userId, UserName = userName };
            ProductReview createdReview = new() { Id = Guid.NewGuid(), Product = product, CreatedAt = review.CreatedAt };
            ProductReviewDto expectedDto = new() { Id = createdReview.Id, ProductId = product.Id, CreatedAt = review.CreatedAt };

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<ProductReview>(createDto))
                .Returns(review);

            productReviewRepositoryMock
                .Setup(r => r.InsertAsync(review, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdReview);

            mapperMock
                .Setup(m => m.Map<ProductReviewDto>(createdReview))
                .Returns(expectedDto);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            ProductReviewDto result = await service.CreateProductReviewAsync(createDto,userId,userName);

            result.Should().BeEquivalentTo(expectedDto);

            mapperMock.Verify(m => m.Map<ProductReview>(createDto), Times.Once);
            productReviewRepositoryMock.Verify(r => r.InsertAsync(review, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductReviewDto>(createdReview), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProductReview_ReturnsProductReviewDto_WhenExists()
        {
            Guid reviewId = Guid.NewGuid();
            UpdateProductReviewDto updateDto = new() { Comment = "I hate it." };

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product = new() { Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16" };

            ProductReview reviewDb = new() { Id = reviewId, Product = product, Comment = "I love it.", UpdatedAt = DateTime.UtcNow };
            ProductReviewDto expectedDto = new() { Id = reviewId, ProductId = product.Id, Comment = updateDto.Comment, UpdatedAt = reviewDb.UpdatedAt };

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reviewDb);

            mapperMock
                .Setup(m => m.Map<UpdateProductReviewDto, ProductReview>(updateDto, reviewDb))
                .Returns(reviewDb);

            productReviewRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<ProductReviewDto>(reviewDb))
                .Returns(expectedDto);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            ProductReviewDto? result = await service.UpdateProductReviewAsync(reviewId, updateDto);

            result.Should().BeEquivalentTo(expectedDto);

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<UpdateProductReviewDto, ProductReview>(updateDto, reviewDb), Times.Once);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductReviewDto>(reviewDb), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProductReview_ReturnsNull_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();
            UpdateProductReviewDto updateDto = new() { Comment = "I hate it." };

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReview?)null);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            ProductReviewDto? result = await service.UpdateProductReviewAsync(reviewId, updateDto);

            result.Should().BeNull();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<UpdateProductReviewDto, ProductReview>(updateDto, It.IsAny<ProductReview>()), Times.Never);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            mapperMock.Verify(m => m.Map<ProductReviewDto>(It.IsAny<ProductReview>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProducReviewAsync_ReturnsProductReviewDto_WhenExists()
        {
            Guid reviewId = Guid.NewGuid();

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product = new() { Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16" };

            ProductReview review = new() { Id = reviewId, Product = product };
            ProductReviewDto expectedDto = new() { Id = reviewId, ProductId = product.Id };

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            mapperMock
                .Setup(m => m.Map<ProductReviewDto>(review))
                .Returns(expectedDto);

            productReviewRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            ProductReviewDto? result = await service.DeleteProductReviewAsync(reviewId);

            result.Should().BeEquivalentTo(expectedDto);

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductReviewDto>(review), Times.Once);
            productReviewRepositoryMock.Verify(r => r.Remove(review), Times.Once);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProducReviewAsync_ReturnsNull_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReview?)null);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            ProductReviewDto? result = await service.DeleteProductReviewAsync(reviewId);

            result.Should().BeNull();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductReviewDto>(It.IsAny<ProductReview>()), Times.Never);
            productReviewRepositoryMock.Verify(r => r.Remove(It.IsAny<ProductReview>()), Times.Never);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOwnProductReviewAsync_ReturnsProductReviewDto_WhenExistsAndIsOwner()
        {
            Guid reviewId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product = new() { Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16" };

            ProductReview review = new() { Id = reviewId, Product = product, UserId = userId };
            ProductReviewDto expectedDto = new() { Id = reviewId, ProductId = product.Id, UserId = userId };

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            mapperMock
                .Setup(m => m.Map<ProductReviewDto>(review))
                .Returns(expectedDto);

            productReviewRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            ProductReviewDto? result = await service.DeleteOwnProductReviewAsync(reviewId, userId);

            result.Should().BeEquivalentTo(expectedDto);

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductReviewDto>(review), Times.Once);
            productReviewRepositoryMock.Verify(r => r.Remove(review), Times.Once);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOwnProductReviewAsync_ReturnsNull_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReview?)null);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            ProductReviewDto? result = await service.DeleteOwnProductReviewAsync(reviewId, userId);

            result.Should().BeNull();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductReviewDto>(It.IsAny<ProductReview>()), Times.Never);
            productReviewRepositoryMock.Verify(r => r.Remove(It.IsAny<ProductReview>()), Times.Never);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOwnProductReviewAsync_ReturnsNull_WhenIsNotOwner()
        {
            Guid reviewId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product = new() { Id = Guid.NewGuid(), Brand = brand, Title = "iPhone 16" };

            ProductReview review = new() { Id = reviewId, Product = product, UserId = Guid.NewGuid() };
            ProductReviewDto expectedDto = new() { Id = reviewId, ProductId = product.Id, UserId = review.UserId };

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            ProductReviewDto? result = await service.DeleteOwnProductReviewAsync(reviewId, userId);

            result.Should().BeNull();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<ProductReviewDto>(review), Times.Never);
            productReviewRepositoryMock.Verify(r => r.Remove(review), Times.Never);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductReviewsByProductIdAsync_ReturnsProductReviewDtoList_WhenExists()
        {
            Guid productId = Guid.NewGuid();

            Brand brand = new() { Id = Guid.NewGuid(), Title = "Apple" };
            Product product = new() { Id = productId, Brand = brand, Title = "iPhone 16" };

            List<ProductReview> reviews =
            [
                new() {Id = Guid.NewGuid(), Product = product},
                new() {Id = Guid.NewGuid(), Product = product}
            ];

            List<ProductReviewDto> expectedDto =
            [
                new() {Id = reviews[0].Id, ProductId = product.Id},
                new() {Id = reviews[1].Id, ProductId = product.Id}
            ];

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.GetAllProductReviewsByProductIdAsync(productId,It.IsAny<CancellationToken>()))
                .ReturnsAsync(reviews);

            mapperMock
                .Setup(m => m.Map<List<ProductReviewDto>>(reviews))
                .Returns(expectedDto);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            IReadOnlyList<ProductReviewDto> result = await service.GetAllProductReviewsByProductIdAsync(productId);

            result.Should().BeEquivalentTo(expectedDto);

            productReviewRepositoryMock.Verify(r => r.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductReviewDto>>(reviews), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductReviewsByProductIdAsync_ReturnsEmptyList_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            List<ProductReview> reviews = [];

            List<ProductReviewDto> expectedDto = [];


            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            productReviewRepositoryMock
                .Setup(r => r.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reviews);

            mapperMock
                .Setup(m => m.Map<List<ProductReviewDto>>(reviews))
                .Returns(expectedDto);

            ProductReviewService service = new(
                productReviewRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
                );


            IReadOnlyList<ProductReviewDto> result = await service.GetAllProductReviewsByProductIdAsync(productId);

            result.Should().BeEquivalentTo(expectedDto);

            productReviewRepositoryMock.Verify(r => r.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductReviewDto>>(reviews), Times.Once);
        }



    }
}
