using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entity;

namespace ProductService.Persistence.Repositories
{
    public class CategoryRepository(ProductDbContext dbContext) : BaseRepository<Category>(dbContext), ICategoryRepository
    {
        public async Task<Category?> FindCategoryByIdWithIncludeProductsAsync(Guid categoryId) => await _dbSet
                                                                                                    .Include(c => c.Products)
                                                                                                    .FirstOrDefaultAsync(c => c.Id == categoryId);



        public async Task<IReadOnlyList<Category>> GetCategoriesByTitle(List<string> titles) => await _dbSet
                                                                                                        .Where(c => titles.Contains(c.Title))
                                                                                                        .ToListAsync();

    }
}
