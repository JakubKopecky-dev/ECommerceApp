using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entity;

namespace ProductService.Persistence.Repositories
{
    public class ProductReviewRepository(ProductDbContext dbContext) : BaseRepository<ProductReview>(dbContext), IProductReviewRepository
    {
    }
}
