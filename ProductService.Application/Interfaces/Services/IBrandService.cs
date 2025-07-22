using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Application.DTOs.Brand;

namespace ProductService.Application.Interfaces.Services
{
    public interface IBrandService
    {
        Task<BrandDto> CreateBrandAsync(CreateUpdateBrandDto createDto);
        Task<BrandDto?> DeleteBrandAsync(Guid brandId);
        Task<IReadOnlyList<BrandDto>> GetAllBrandsAsync();
        Task<BrandDto?> GetBrandByIdAsync(Guid brandId);
        Task<BrandDto?> UpdateBrandAsync(Guid brandId, CreateUpdateBrandDto updateDto);
    }
}
