using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entity;

namespace ProductService.Persistence.Repositories
{
    public class CategoryRepository(ProductDbContext dbContext) : BaseRepository<Category>(dbContext), ICategoryRepository
    {
        public async Task<Category?> FindCategoryByIdWithIncludeProductsAsync(Guid categoryId, CancellationToken ct = default) => await _dbSet
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == categoryId, ct);


        public async Task<IReadOnlyList<Category>> GetCategoriesByTitle(List<string> titles, CancellationToken ct = default) => await _dbSet
            .Where(c => titles.Contains(c.Title))
            .ToListAsync(ct);


        public async Task<List<Category>> GetCategoriesByName(List<string> names, CancellationToken ct = default) => await _dbSet
            .Where(c => names.Contains(c.Title))
            .ToListAsync(ct);
    }
}