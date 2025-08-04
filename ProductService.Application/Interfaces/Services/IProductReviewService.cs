using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Application.DTOs.ProductReview;

namespace ProductService.Application.Interfaces.Services
{
    public interface IProductReviewService
    {
        Task<ProductReviewDto> CreateProductReviewAsync(CreateProductReviewDto createDto);
        Task<ProductReviewDto?> DeleteOwnProductReviewAsync(Guid reviewId, Guid userId);
        Task<ProductReviewDto?> DeleteProductReviewAsync(Guid reviewId);
        Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviewsAsync();
        Task<ProductReviewDto?> GetProductReviewAsync(Guid reviewId);
        Task<ProductReviewDto?> UpdateProductReviewAsync(Guid reviewId, UpdateProductReviewDto updateDto);
    }
}
