using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<Category?> FindCategoryByIdWithIncludeProductsAsync(Guid categoryId, CancellationToken ct = default);
        Task<IReadOnlyList<Category>> GetCategoriesByTitle(List<string> titles, CancellationToken ct = default);
        Task<List<Category>>  GetCategoriesByName(List<string> names, CancellationToken ct = default);
    }
}
