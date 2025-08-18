using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.ProductReview;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entity;

namespace ProductService.Application.Services
{
    public class ProductReviewService(IProductReviewRepository productReviewRepository, IMapper mapper, ILogger<ProductReviewService> logger) : IProductReviewService
    {
        private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProductReviewService> _logger = logger;



        public async Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviewsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all productReviews.");

            IReadOnlyList<ProductReview> reviews = await _productReviewRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all productReviews. Count: {Count}.", reviews.Count);

            return _mapper.Map<List<ProductReviewDto>>(reviews);
        }



        public async Task<ProductReviewDto?> GetProductReviewAsync(Guid reviewId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving productReview. ProductReviewId: {ReviewId}.", reviewId);

            ProductReview? review = await _productReviewRepository.FindByIdAsync(reviewId,ct);
            if (review is null)
            {
                _logger.LogWarning("ProductReview not found. ProductReviewId: {ReviewId}.", reviewId);
                return null;
            }

            _logger.LogInformation("ProductReview found. ProductReviewId: {ReviewId}.", reviewId);
            return _mapper.Map<ProductReviewDto>(review);
        }



        public async Task<ProductReviewDto> CreateProductReviewAsync(CreateProductReviewDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new productReview. Title: {Title}.", createDto.Title);

            ProductReview review = _mapper.Map<ProductReview>(createDto);
            review.Id = Guid.Empty;
            review.CreatedAt = DateTime.UtcNow;

            ProductReview createdReview = await _productReviewRepository.InsertAsync(review,ct);
            _logger.LogInformation("ProductReview created. ProductReviewId: {ReviewId}.", createdReview.Id);


            return _mapper.Map<ProductReviewDto>(review);
        }



        public async Task<ProductReviewDto?> UpdateProductReviewAsync(Guid reviewId, UpdateProductReviewDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating productReview. ProductReviewId: {reviewId}.", reviewId);

            ProductReview? reviewDb = await _productReviewRepository.FindByIdAsync(reviewId, ct);
            if (reviewDb is null)
            {
                _logger.LogWarning("Cannot update. ProductReview not found. ProductReviewId: {ReviewId}.", reviewId);
                return null;
            }

            _mapper.Map<UpdateProductReviewDto, ProductReview>(updateDto, reviewDb);

            reviewDb.UpdatedAt = DateTime.UtcNow;

            await _productReviewRepository.SaveChangesAsync(ct);
            _logger.LogInformation("ProductReview updated. ProductReviewId: {ReviewId}.", reviewId);

            return _mapper.Map<ProductReviewDto>(reviewDb);
        }



        public async Task<ProductReviewDto?> DeleteProductReviewAsync(Guid reviewId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting productReview. ProductReviewId: {reviewId}.", reviewId);

            ProductReview? review = await _productReviewRepository.FindByIdAsync(reviewId, ct);
            if (review is null)
            {
                _logger.LogWarning("Cannot delete. ProductReview not found. ProductReviewId: {ReviewId}.", reviewId);
                return null;
            }

            ProductReviewDto deletedReview = _mapper.Map<ProductReviewDto>(review);

            _productReviewRepository.Remove(review);

            await _productReviewRepository.SaveChangesAsync(ct);
            _logger.LogInformation("ProductReview deleted. ProductReviewId: {ReviewId}.", reviewId);

            return deletedReview;
        }



        public async Task<ProductReviewDto?> DeleteOwnProductReviewAsync(Guid reviewId, Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting own productReview. ProductReviewId: {reviewId}, UserId: {UserId}.", reviewId, userId);

            ProductReview? review = await _productReviewRepository.FindByIdAsync(reviewId, ct);
            if (review is null)
            {
                _logger.LogWarning("Cannot delete. ProductReview not found. ProductReviewId: {ReviewId}, UserId: {UserId}.", reviewId, userId);
                return null;
            }


            if (review.UserId != userId)
            {
                _logger.LogWarning("Cannot delete. User is not owner  ProductReviewId: {ReviewId}, UserId: {UserId}.", reviewId, userId);
                return null;
            }

            ProductReviewDto deletedReview = _mapper.Map<ProductReviewDto>(review);

            _productReviewRepository.Remove(review);
            await _productReviewRepository.SaveChangesAsync(ct);
            _logger.LogInformation("ProductReview deleted. ProductReviewId: {ReviewId}.", reviewId);

            return deletedReview;
        }





    }
}
