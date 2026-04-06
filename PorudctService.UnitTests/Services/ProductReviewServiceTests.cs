using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Application.DTOs.ProductReview;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Services;
using ProductService.Domain.Entities;

namespace ProductService.UnitTests.Services
{
    public class ProductReviewServiceTests
    {
        private static ProductReviewService CreateService(Mock<IProductReviewRepository> productReviewRepositoryMock)
        {
            return new ProductReviewService(
                productReviewRepositoryMock.Object,
                new Mock<ILogger<ProductReviewService>>().Object
            );
        }

        private static ProductReview CreateReview(Guid? productId = null, Guid? userId = null)
            => ProductReview.Create("Great product", productId ?? Guid.NewGuid(), 5, "Really good!", userId ?? Guid.NewGuid(), "John");


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductReviewsAsync_ReturnsProductReviewDtoList_WhenExists()
        {
            List<ProductReview> reviews =
            [
                CreateReview(),
                CreateReview()
            ];

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(reviews);

            var service = CreateService(productReviewRepositoryMock);

            IReadOnlyList<ProductReviewDto> result = await service.GetAllProductReviewsAsync();

            result.Should().HaveCount(2);
            productReviewRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductReviewsAsync_ReturnsEmptyList_WhenNotExists()
        {
            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(productReviewRepositoryMock);

            IReadOnlyList<ProductReviewDto> result = await service.GetAllProductReviewsAsync();

            result.Should().BeEmpty();
            productReviewRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProductReviewAsync_ReturnsProductReviewDto_WhenExists()
        {
            ProductReview review = CreateReview();
            Guid reviewId = review.Id;

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var service = CreateService(productReviewRepositoryMock);

            ProductReviewDto? result = await service.GetProductReviewAsync(reviewId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(reviewId);
            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProductReviewAsync_ReturnsNull_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReview?)null);

            var service = CreateService(productReviewRepositoryMock);

            ProductReviewDto? result = await service.GetProductReviewAsync(reviewId);

            result.Should().BeNull();
            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateProductReviewAsync_ReturnsProductReviewDto()
        {
            Guid userId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();
            string userName = "John";

            CreateProductReviewDto createDto = new()
            {
                Title = "Great product",
                ProductId = productId,
                Rating = 5,
                Comment = "Really good!"
            };

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<ProductReview>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            productReviewRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productReviewRepositoryMock);

            ProductReviewDto result = await service.CreateProductReviewAsync(createDto, userId, userName);

            result.Should().NotBeNull();
            result.ProductId.Should().Be(productId);
            result.UserId.Should().Be(userId);
            result.UserName.Should().Be(userName);
            result.Rating.Should().Be(createDto.Rating);

            productReviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ProductReview>(), It.IsAny<CancellationToken>()), Times.Once);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProductReviewAsync_ReturnsProductReviewDto_WhenExists()
        {
            ProductReview review = CreateReview();
            Guid reviewId = review.Id;

            UpdateProductReviewDto updateDto = new()
            {
                Title = "Updated title",
                Rating = 3,
                Comment = "Changed my mind"
            };

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            productReviewRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productReviewRepositoryMock);

            ProductReviewDto? result = await service.UpdateProductReviewAsync(reviewId, updateDto);

            result.Should().NotBeNull();
            result!.Title.Should().Be(updateDto.Title);
            result.Rating.Should().Be(updateDto.Rating);
            result.Comment.Should().Be(updateDto.Comment);

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProductReviewAsync_ReturnsNull_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();
            UpdateProductReviewDto updateDto = new() { Comment = "Changed" };

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReview?)null);

            var service = CreateService(productReviewRepositoryMock);

            ProductReviewDto? result = await service.UpdateProductReviewAsync(reviewId, updateDto);

            result.Should().BeNull();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProductReviewAsync_ReturnsTrue_WhenExists()
        {
            ProductReview review = CreateReview();
            Guid reviewId = review.Id;

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            productReviewRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productReviewRepositoryMock);

            bool result = await service.DeleteProductReviewAsync(reviewId);

            result.Should().BeTrue();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            productReviewRepositoryMock.Verify(r => r.Remove(review), Times.Once);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProductReviewAsync_ReturnsFalse_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReview?)null);

            var service = CreateService(productReviewRepositoryMock);

            bool result = await service.DeleteProductReviewAsync(reviewId);

            result.Should().BeFalse();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            productReviewRepositoryMock.Verify(r => r.Remove(It.IsAny<ProductReview>()), Times.Never);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOwnProductReviewAsync_ReturnsTrue_WhenExistsAndIsOwner()
        {
            Guid userId = Guid.NewGuid();
            ProductReview review = CreateReview(userId: userId);
            Guid reviewId = review.Id;

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            productReviewRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(productReviewRepositoryMock);

            bool result = await service.DeleteOwnProductReviewAsync(reviewId, userId);

            result.Should().BeTrue();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            productReviewRepositoryMock.Verify(r => r.Remove(review), Times.Once);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOwnProductReviewAsync_ReturnsFalse_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReview?)null);

            var service = CreateService(productReviewRepositoryMock);

            bool result = await service.DeleteOwnProductReviewAsync(reviewId, userId);

            result.Should().BeFalse();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            productReviewRepositoryMock.Verify(r => r.Remove(It.IsAny<ProductReview>()), Times.Never);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOwnProductReviewAsync_ReturnsFalse_WhenIsNotOwner()
        {
            Guid userId = Guid.NewGuid();
            ProductReview review = CreateReview(userId: Guid.NewGuid()); // jiný userId
            Guid reviewId = review.Id;

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var service = CreateService(productReviewRepositoryMock);

            bool result = await service.DeleteOwnProductReviewAsync(reviewId, userId);

            result.Should().BeFalse();

            productReviewRepositoryMock.Verify(r => r.FindByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
            productReviewRepositoryMock.Verify(r => r.Remove(It.IsAny<ProductReview>()), Times.Never);
            productReviewRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductReviewsByProductIdAsync_ReturnsProductReviewDtoList_WhenExists()
        {
            Guid productId = Guid.NewGuid();
            List<ProductReview> reviews =
            [
                CreateReview(productId: productId),
                CreateReview(productId: productId)
            ];

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reviews);

            var service = CreateService(productReviewRepositoryMock);

            IReadOnlyList<ProductReviewDto> result = await service.GetAllProductReviewsByProductIdAsync(productId);

            result.Should().HaveCount(2);
            result.Should().AllSatisfy(r => r.ProductId.Should().Be(productId));
            productReviewRepositoryMock.Verify(r => r.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductReviewsByProductIdAsync_ReturnsEmptyList_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            Mock<IProductReviewRepository> productReviewRepositoryMock = new();
            productReviewRepositoryMock
                .Setup(r => r.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var service = CreateService(productReviewRepositoryMock);

            IReadOnlyList<ProductReviewDto> result = await service.GetAllProductReviewsByProductIdAsync(productId);

            result.Should().BeEmpty();
            productReviewRepositoryMock.Verify(r => r.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}