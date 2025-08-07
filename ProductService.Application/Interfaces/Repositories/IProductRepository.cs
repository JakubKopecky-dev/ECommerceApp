using ProductService.Domain.Entity;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Product?> FindProductByIdIncludeCategoriesAsync(Guid productId);
        Task<Product?> FindProductByIdIncludeCategoriesAndReviewsAsync(Guid productId);
    }
}
