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
        Task<ProductReviewDto> CreateProductReviewAsync(CreateUpdateProductReviewDto createDto);
        Task<ProductReviewDto?> DeleteProductReviewAsync(Guid reviewId);
        Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviewsAsync();
        Task<ProductReviewDto?> GetProductReviewAsync(Guid reviewId);
        Task<ProductReviewDto?> UpdateProductReviewAsync(Guid reviewId, CreateUpdateProductReviewDto updateDto);
    }
}
