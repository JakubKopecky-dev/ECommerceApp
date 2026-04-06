using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.ProductReview;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entities;

namespace ProductService.Application.Services
{
    public class ProductReviewService(IProductReviewRepository productReviewRepository,ILogger<ProductReviewService> logger) : IProductReviewService
    {
        private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
        private readonly ILogger<ProductReviewService> _logger = logger;



        public async Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviewsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all productReviews.");

            IReadOnlyList<ProductReview> reviews = await _productReviewRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all productReviews. Count: {Count}.", reviews.Count);

            return [..reviews.Select(x => x.ProductReviewToProductReviewDto())];
        }



        public async Task<ProductReviewDto?> GetProductReviewAsync(Guid reviewId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving productReview. ProductReviewId: {ReviewId}.", reviewId);

            ProductReview? review = await _productReviewRepository.FindByIdAsync(reviewId,ct);
            if (review is null)
                _logger.LogWarning("ProductReview not found. ProductReviewId: {ReviewId}.", reviewId);
            else
            _logger.LogInformation("ProductReview found. ProductReviewId: {ReviewId}.", reviewId);

            return review?.ProductReviewToProductReviewDto();
        }



        public async Task<ProductReviewDto> CreateProductReviewAsync(CreateProductReviewDto createDto, Guid userId, string userName, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new productReview. Title: {Title}.", createDto.Title);

            ProductReview review = ProductReview.Create(createDto.Title, createDto.ProductId, createDto.Rating, createDto.Comment, userId, userName);

             await _productReviewRepository.AddAsync(review,ct);
            await _productReviewRepository.SaveChangesAsync(ct);

            _logger.LogInformation("ProductReview created. ProductReviewId: {ReviewId}.", review.Id);

            return review.ProductReviewToProductReviewDto();
        }



        public async Task<ProductReviewDto?> UpdateProductReviewAsync(Guid reviewId, UpdateProductReviewDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating productReview. ProductReviewId: {reviewId}.", reviewId);

            ProductReview? review = await _productReviewRepository.FindByIdAsync(reviewId, ct);
            if (review is null)
            {
                _logger.LogWarning("Cannot update. ProductReview not found. ProductReviewId: {ReviewId}.", reviewId);
                return null;
            }

            review.Update(updateDto.Title, updateDto.Rating, updateDto.Comment);

            await _productReviewRepository.SaveChangesAsync(ct);
            _logger.LogInformation("ProductReview updated. ProductReviewId: {ReviewId}.", reviewId);

            return review.ProductReviewToProductReviewDto();
        }



        public async Task<bool> DeleteProductReviewAsync(Guid reviewId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting productReview. ProductReviewId: {reviewId}.", reviewId);

            ProductReview? review = await _productReviewRepository.FindByIdAsync(reviewId, ct);
            if (review is null)
            {
                _logger.LogWarning("Cannot delete. ProductReview not found. ProductReviewId: {ReviewId}.", reviewId);
                return false;
            }


            _productReviewRepository.Remove(review);
            await _productReviewRepository.SaveChangesAsync(ct);
            _logger.LogInformation("ProductReview deleted. ProductReviewId: {ReviewId}.", reviewId);

            return true;
        }



        public async Task<bool> DeleteOwnProductReviewAsync(Guid reviewId, Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting own productReview. ProductReviewId: {reviewId}, UserId: {UserId}.", reviewId, userId);

            ProductReview? review = await _productReviewRepository.FindByIdAsync(reviewId, ct);
            if (review is null)
            {
                _logger.LogWarning("Cannot delete. ProductReview not found. ProductReviewId: {ReviewId}, UserId: {UserId}.", reviewId, userId);
                return false;
            }

            if (review.UserId != userId)
            {
                _logger.LogWarning("Cannot delete. User is not owner  ProductReviewId: {ReviewId}, UserId: {UserId}.", reviewId, userId);
                return false;
            }


            _productReviewRepository.Remove(review);
            await _productReviewRepository.SaveChangesAsync(ct);
            _logger.LogInformation("ProductReview deleted. ProductReviewId: {ReviewId}.", reviewId);

            return true;
        }



        public async Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviewsByProductIdAsync(Guid productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all productReviews by productId. ProductId: {ProductId}.", productId);

            IReadOnlyList<ProductReview> productReviews = await _productReviewRepository.GetAllProductReviewsByProductIdAsync(productId, ct);
            _logger.LogInformation("Retrived all productReviews by productId. Count: {Count}, ProductId: {ProductId}.", productReviews.Count,productId);

            return [..productReviews.Select(x => x.ProductReviewToProductReviewDto())];
        }




    }
}
