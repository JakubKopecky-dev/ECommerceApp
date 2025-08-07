using ProductService.Domain.Entity;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IBrandRepository : IBaseRepository<Brand>
    {
        Task<Brand?> FindBrandByIdWithIncludes(Guid brandId);
    }
}
