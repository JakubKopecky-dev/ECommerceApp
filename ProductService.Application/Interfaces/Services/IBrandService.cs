using ProductService.Application.DTOs.Brand;

namespace ProductService.Application.Interfaces.Services
{
    public interface IBrandService
    {
        Task<BrandDto> CreateBrandAsync(CreateUpdateBrandDto createDto, CancellationToken ct = default);
        Task<BrandDto?> DeleteBrandAsync(Guid brandId, CancellationToken ct = default);
        Task<IReadOnlyList<BrandDto>> GetAllBrandsAsync(CancellationToken ct = default);
        Task<BrandDto?> GetBrandByIdAsync(Guid brandId, CancellationToken ct = default);
        Task<BrandDto?> UpdateBrandAsync(Guid brandId, CreateUpdateBrandDto updateDto, CancellationToken ct = default);
    }
}
