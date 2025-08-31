using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductService.Api.Controllers;
using ProductService.Application.DTOs.ProductReview;
using ProductService.Application.Interfaces.Services;

namespace ProductService.UnitTests.Controllers
{
    public class ProductReviewControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductReviews_ReturnsProductReviewDtoList_WhenExists()
        {
            IReadOnlyList<ProductReviewDto> expectedDto =
            [
                new() { Id = Guid.NewGuid(), Comment = "Great product" },
                new() { Id = Guid.NewGuid(), Comment = "Not bad" }
            ];

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.GetAllProductReviewsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductReviewController controller = new(reviewServiceMock.Object);


            IReadOnlyList<ProductReviewDto> result = await controller.GetAllProductReviews(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            reviewServiceMock.Verify(p => p.GetAllProductReviewsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductReviews_ReturnsEmptyList_WhenNotExists()
        {
            IReadOnlyList<ProductReviewDto> expectedDto = [];

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.GetAllProductReviewsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductReviewController controller = new(reviewServiceMock.Object);


            IReadOnlyList<ProductReviewDto> result = await controller.GetAllProductReviews(It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            reviewServiceMock.Verify(p => p.GetAllProductReviewsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProductReview_ReturnsOk_WhenExists()
        {
            Guid reviewId = Guid.NewGuid();

            ProductReviewDto expectedDto = new() { Id = reviewId, Comment = "Nice phone" };

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.GetProductReviewAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductReviewController controller = new(reviewServiceMock.Object);


            var result = await controller.GetProductReview(reviewId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            reviewServiceMock.Verify(p => p.GetProductReviewAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetProductReview_ReturnsNotFound_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.GetProductReviewAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReviewDto?)null);

            ProductReviewController controller = new(reviewServiceMock.Object);


            var result = await controller.GetProductReview(reviewId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            reviewServiceMock.Verify(p => p.GetProductReviewAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateProductReview_ReturnsCreatedAtAction_WithProductReviewDto()
        {
            CreateProductReviewDto createDto = new() { Comment = "Excellent", ProductId = Guid.NewGuid(), UserId = Guid.NewGuid() };

            ProductReviewDto expectedDto = new() { Id = Guid.NewGuid(), Comment = createDto.Comment, ProductId = createDto.ProductId, UserId = createDto.UserId };

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(r => r.CreateProductReviewAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductReviewController controller = new(reviewServiceMock.Object);


            var result = await controller.CreateProductReview(createDto, It.IsAny<CancellationToken>());
            var createdResult = result as CreatedAtActionResult;

            createdResult!.ActionName.Should().Be(nameof(ProductReviewController.GetProductReview));
            createdResult.RouteValues!["reviewId"].Should().Be(expectedDto.Id);
            createdResult.Value.Should().BeEquivalentTo(expectedDto);

            reviewServiceMock.Verify(r => r.CreateProductReviewAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProductReview_ReturnsOk_WhenExists()
        {
            Guid reviewId = Guid.NewGuid();
            UpdateProductReviewDto updateDto = new() { Comment = "Nice phone" };

            ProductReviewDto expectedDto = new() { Id = reviewId, Comment = updateDto.Comment };

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.UpdateProductReviewAsync(reviewId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductReviewController controller = new(reviewServiceMock.Object);


            var result = await controller.UpdateProductReview(reviewId, updateDto, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            reviewServiceMock.Verify(p => p.UpdateProductReviewAsync(reviewId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateProductReview_ReturnsNotFound_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();
            UpdateProductReviewDto updateDto = new() { Comment = "Nice phone" };

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.UpdateProductReviewAsync(reviewId, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReviewDto?)null);

            ProductReviewController controller = new(reviewServiceMock.Object);


            var result = await controller.UpdateProductReview(reviewId, updateDto, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            reviewServiceMock.Verify(p => p.UpdateProductReviewAsync(reviewId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProductReview_ReturnsOk_WhenExists()
        {
            Guid reviewId = Guid.NewGuid();

            ProductReviewDto expectedDto = new() { Id = reviewId, Comment = "Nice phone" };

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.DeleteProductReviewAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductReviewController controller = new(reviewServiceMock.Object);


            var result = await controller.DeleteProductReview(reviewId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            reviewServiceMock.Verify(p => p.DeleteProductReviewAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteProductReview_ReturnsNotFound_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.DeleteProductReviewAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReviewDto?)null);

            ProductReviewController controller = new(reviewServiceMock.Object);


            var result = await controller.DeleteProductReview(reviewId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            reviewServiceMock.Verify(p => p.DeleteProductReviewAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOwnProductReview_ReturnsUnauthorized_WhenUserIdMissing()
        {
            Guid reviewId = Guid.NewGuid();

            Mock<IProductReviewService> reviewServiceMock = new();

            ProductReviewController controller = new(reviewServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity())
                    }
                }
            };


            var result = await controller.DeleteOwnProductReview(reviewId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<UnauthorizedResult>();
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOwnProductReview_ReturnsOk_WhenExists()
        {
            Guid reviewId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            ProductReviewDto expectedDto = new() { Id = reviewId, Comment = "Nice phone", UserId = userId };

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.DeleteOwnProductReviewAsync(reviewId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductReviewController controller = new(reviewServiceMock.Object)
            {
                ControllerContext = new()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                        [
                            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                        ]))
                    }
                }
            };


            var result = await controller.DeleteOwnProductReview(reviewId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            reviewServiceMock.Verify(p => p.DeleteOwnProductReviewAsync(reviewId, userId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteOwnProductReview_ReturnsNotFound_WhenNotExists()
        {
            Guid reviewId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.DeleteOwnProductReviewAsync(reviewId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductReviewDto?)null);

            ProductReviewController controller = new(reviewServiceMock.Object)
            {
                ControllerContext = new()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                        [
                             new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                        ]))
                    }
                }
            };


            var result = await controller.DeleteOwnProductReview(reviewId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            reviewServiceMock.Verify(p => p.DeleteOwnProductReviewAsync(reviewId, userId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductReviewsByProductId_ReturnsProductReviewDtoList_WhenExists()
        {
            Guid productId = Guid.NewGuid();

            IReadOnlyList<ProductReviewDto> expectedDto =
            [
                new() { Id = Guid.NewGuid(), ProductId = productId, Comment = "Nice phone" }
            ];

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductReviewController controller = new(reviewServiceMock.Object);


            IReadOnlyList<ProductReviewDto> result = await controller.GetAllProductReviewsByProductId(productId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            reviewServiceMock.Verify(p => p.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllProductReviewsByProductId_ReturnsEmptyList_WhenNotExists()
        {
            Guid productId = Guid.NewGuid();

            IReadOnlyList<ProductReviewDto> expectedDto = [];

            Mock<IProductReviewService> reviewServiceMock = new();

            reviewServiceMock
                .Setup(p => p.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            ProductReviewController controller = new(reviewServiceMock.Object);


            IReadOnlyList<ProductReviewDto> result = await controller.GetAllProductReviewsByProductId(productId, It.IsAny<CancellationToken>());

            result.Should().BeEquivalentTo(expectedDto);

            reviewServiceMock.Verify(p => p.GetAllProductReviewsByProductIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
