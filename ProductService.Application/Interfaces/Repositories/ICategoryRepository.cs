using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Domain.Entity;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<Category?> FindCategoryByIdWithIncludeProductsAsync(Guid categoryId);
        Task<IReadOnlyList<Category>> GetCategoriesByTitle(List<string> titles);
    }
}
