using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entity;

namespace ProductService.Persistence.Repositories
{
    public class BrandRepository(ProductDbContext dbContext) : BaseRepository<Brand>(dbContext), IBrandRepository
    {
        public async Task<Brand?> FindBrandByIdWithIncludes(Guid brandId, CancellationToken ct = default) => await _dbSet
            .Include(b => b.Products)
            .ThenInclude(p => p.Reviews)
            .Include(b => b.Products)
            .ThenInclude(p => p.Categories)
            .SingleOrDefaultAsync(b => b.Id == brandId, ct);



    }
}
