using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entity;

namespace ProductService.Persistence.Repositories
{
    public class ProductReviewRepository(ProductDbContext dbContext) : BaseRepository<ProductReview>(dbContext), IProductReviewRepository
    {
    }
}
