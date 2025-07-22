using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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



        public async Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviewsAsync()
        {
            _logger.LogInformation("Retrieving all productReviews.");

            IReadOnlyList<ProductReview> reviews = await _productReviewRepository.GetAllAsync();
            _logger.LogInformation("Retrieved all productReviews. Count: {Count}.", reviews.Count);

            return _mapper.Map<List<ProductReviewDto>>(reviews);
        }



        public async Task<ProductReviewDto?> GetProductReviewAsync(Guid reviewId)
        {
            _logger.LogInformation("Retrieving productReview. ProductReviewId: {ReviewId}.", reviewId);

            ProductReview? review = await _productReviewRepository.FindByIdAsync(reviewId);
            if (review is null)
            {
                _logger.LogWarning("ProductReview not found. ProductReviewId: {ReviewId}.", reviewId);
                return null;
            }

            _logger.LogInformation("ProductReview found. ProductReviewId: {ReviewId}.", reviewId);
            return _mapper.Map<ProductReviewDto>(review);
        }




        public async Task<ProductReviewDto> CreateProductReviewAsync(CreateUpdateProductReviewDto createDto)
        {
            _logger.LogInformation("Creating productReview. Title: {Title}.", createDto.Title);

            ProductReview review = _mapper.Map<ProductReview>(createDto);
            review.Id = default;
            review.CreatedAt = DateTime.UtcNow;

            ProductReview createdReview = await _productReviewRepository.InsertAsync(review);
            _logger.LogInformation("ProductReview created. ProductReviewId: {ReviewId}.", createdReview.Id);


            return _mapper.Map<ProductReviewDto>(review);
        }



        public async Task<ProductReviewDto?> UpdateProductReviewAsync(Guid reviewId, CreateUpdateProductReviewDto updateDto)
        {
            _logger.LogInformation("Updating productReview. ProductReviewId: {reviewId}.", reviewId);

            ProductReview? reviewDb = await _productReviewRepository.FindByIdAsync(reviewId);
            if (reviewDb is null)
            {
                _logger.LogWarning("Cannot update. ProductReview not found. ProductReviewId: {ReviewId}.", reviewId);
                return null;
            }

            _mapper.Map<CreateUpdateProductReviewDto, ProductReview>(updateDto, reviewDb);

            reviewDb.UpdatedAt = DateTime.UtcNow;

            ProductReview updatedReview = await _productReviewRepository.UpdateAsync(reviewDb);
            _logger.LogInformation("ProductReview updated. ProductReviewId: {ReviewId}.", reviewId);

            return _mapper.Map<ProductReviewDto>(updatedReview);
        }



        public async Task<ProductReviewDto?> DeleteProductReviewAsync(Guid reviewId)
        {
            _logger.LogInformation("Deleting productReview. ProductReviewId: {reviewId}.", reviewId);

            ProductReview? review = await _productReviewRepository.FindByIdAsync(reviewId);
            if (review is null)
            {
                _logger.LogWarning("Cannot delete. ProductReview not found. ProductReviewId: {ReviewId}.", reviewId);
                return null;
            }

            ProductReviewDto deletedReview = _mapper.Map<ProductReviewDto>(review);

            await _productReviewRepository.DeleteAsync(reviewId);
            _logger.LogInformation("ProductReview deleted. ProductReviewId: {ReviewId}.", reviewId);

            return deletedReview;
        }







    }
}
