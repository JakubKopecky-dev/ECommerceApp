using ProductService.Application.DTOs.ProductReview;

namespace ProductService.Application.Interfaces.Services
{
    public interface IProductReviewService
    {
        Task<ProductReviewDto> CreateProductReviewAsync(CreateProductReviewDto createDto,Guid userId,string userName, CancellationToken ct = default);
        Task<ProductReviewDto?> DeleteOwnProductReviewAsync(Guid reviewId, Guid userId, CancellationToken ct = default);
        Task<ProductReviewDto?> DeleteProductReviewAsync(Guid reviewId, CancellationToken ct = default);
        Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviewsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviewsByProductIdAsync(Guid productId, CancellationToken ct = default);
        Task<ProductReviewDto?> GetProductReviewAsync(Guid reviewId, CancellationToken ct = default);
        Task<ProductReviewDto?> UpdateProductReviewAsync(Guid reviewId, UpdateProductReviewDto updateDto, CancellationToken ct = default);
    }
}
