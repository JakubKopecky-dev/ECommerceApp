using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Brand;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entities;

namespace ProductService.Application.Services
{
    public class BrandService(IBrandRepository brandRepository, IProductRepository productRepository, IProductReviewRepository productReviewRepository, ILogger<BrandService> logger) : IBrandService
    {
        private readonly IBrandRepository _brandRepository = brandRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
        private readonly ILogger<BrandService> _logger = logger;



        public async Task<IReadOnlyList<BrandDto>> GetAllBrandsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all brands.");

            IReadOnlyList<Brand> brands = await _brandRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all brands. Count: {Count.}", brands.Count);

            return [.. brands.Select(x => x.BrandToBrandDto())];
        }



        public async Task<BrandDto?> GetBrandByIdAsync(Guid brandId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving brand. BrandId: {BrandId}.", brandId);

            Brand? brand = await _brandRepository.FindByIdAsync(brandId, ct);
            if (brand is null)
                _logger.LogWarning("Brand not found. BrandId: {BrandId}.", brandId);
            else
                _logger.LogInformation("Brand found. BrandId: {BrandId}.", brandId);

            return brand?.BrandToBrandDto();
        }



        public async Task<BrandDto> CreateBrandAsync(CreateUpdateBrandDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new brand. Title: {Title}.", createDto.Title);

            Brand brand = Brand.Create(createDto.Title, createDto.Description);

            await _brandRepository.AddAsync(brand, ct);
            await _brandRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Brand created. BrandId: {BrandId}.", brand.Id);

            return brand.BrandToBrandDto();
        }



        public async Task<BrandDto?> UpdateBrandAsync(Guid brandId, CreateUpdateBrandDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating brand. BrandId: {BrandId}", brandId);

            Brand? brand = await _brandRepository.FindByIdAsync(brandId, ct);
            if (brand is null)
            {
                _logger.LogWarning("Cannot update. Brand not found. BrandId: {BrandId}.", brandId);
                return null;
            }

            brand.Update(updateDto.Title, updateDto.Description);

            await _brandRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Brand updated. BrandId: {BrandId}.", brandId);

            return brand.BrandToBrandDto();
        }



        public async Task<bool> DeleteBrandAsync(Guid brandId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting brand. BrandId: {BrandId}.", brandId);

            Brand? brand = await _brandRepository.FindBrandByIdWithIncludes(brandId, ct);
            if (brand is null)
            {
                _logger.LogWarning("Cannot delete. Brand not foud. BrandId: {BrandId}.", brandId);
                return false;
            }


            _brandRepository.Remove(brand);
            await _brandRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Brand deleted. BrandId: {BrandId}.", brandId);

            return true;
        }



    }
}
