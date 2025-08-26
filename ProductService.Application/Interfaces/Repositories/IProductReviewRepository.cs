using ProductService.Domain.Entity;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IProductReviewRepository : IBaseRepository<ProductReview>
    {
        Task<IReadOnlyList<ProductReview>> GetAllProductReviewsByProductIdAsync(Guid productId, CancellationToken ct = default);
    }
}
