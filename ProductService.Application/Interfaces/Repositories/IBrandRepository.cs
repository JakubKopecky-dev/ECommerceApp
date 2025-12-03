using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IBrandRepository : IBaseRepository<Brand>
    {
        Task<Brand?> FindBrandByIdWithIncludes(Guid brandId, CancellationToken ct = default);
    }
}
