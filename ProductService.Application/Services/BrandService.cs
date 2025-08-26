using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Brand;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entity;

namespace ProductService.Application.Services
{
    public class BrandService(IBrandRepository brandRepository, IProductRepository productRepository, IProductReviewRepository productReviewRepository, IMapper mapper, ILogger<BrandService> logger) : IBrandService
    {
        private readonly IBrandRepository _brandRepository = brandRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<BrandService> _logger = logger;



        public async Task<IReadOnlyList<BrandDto>> GetAllBrandsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all brands.");

            IReadOnlyList<Brand> brands = await _brandRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all brands. Count: {Count.}", brands.Count);

            return _mapper.Map<List<BrandDto>>(brands);
        }



        public async Task<BrandDto?> GetBrandByIdAsync(Guid brandId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving brand. BrandId: {BrandId}.", brandId);

            Brand? brand = await _brandRepository.FindByIdAsync(brandId,ct);
            if (brand is null)
            {
                _logger.LogWarning("Brand not found. BrandId: {BrandId}.", brandId);
                return null;
            }

            _logger.LogInformation("Brand found. BrandId: {BrandId}.", brandId);

            return _mapper.Map<BrandDto>(brand);
        }



        public async Task<BrandDto> CreateBrandAsync(CreateUpdateBrandDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new brand. Title: {Title}.", createDto.Title);

            Brand brand = _mapper.Map<Brand>(createDto);
            brand.Id = Guid.Empty;
            brand.CreatedAt = DateTime.UtcNow;

            Brand createdBrand = await _brandRepository.InsertAsync(brand,ct);
            _logger.LogInformation("Brand created. BrandId: {BrandId}.", createdBrand.Id);

            return _mapper.Map<BrandDto>(createdBrand);
        }



        public async Task<BrandDto?> UpdateBrandAsync(Guid brandId, CreateUpdateBrandDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating brand. BrandId: {BrandId}", brandId);

            Brand? brandDb = await _brandRepository.FindByIdAsync(brandId, ct);
            if (brandDb is null)
            {
                _logger.LogWarning("Cannot update. Brand not found. BrandId: {BrandId}.", brandId);
                return null;
            }

            _mapper.Map<CreateUpdateBrandDto, Brand>(updateDto, brandDb);

            brandDb.UpdatedAt = DateTime.UtcNow;

            await _brandRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Brand updated. BrandId: {BrandId}.", brandId);

            return _mapper.Map<BrandDto>(brandDb);
        }



        public async Task<BrandDto?> DeleteBrandAsync(Guid brandId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting brand. BrandId: {BrandId}.", brandId);

            Brand? brand = await _brandRepository.FindBrandByIdWithIncludes(brandId, ct);
            if (brand is null)
            {
                _logger.LogWarning("Cannot delete. Brand not foud. BrandId: {BrandId}.", brandId);
                return null;
            }

            BrandDto deletedBrand = _mapper.Map<BrandDto>(brand);

            List<Product> products = [.. brand.Products];
            List<ProductReview> reviews = [.. brand.Products.SelectMany(p => p.Reviews)];

            if (products.Count > 0)
            {
                _logger.LogInformation("Deleting all related products before deleting brand. BrandId: {BrandId}, Product count: {Count}.", brandId, products.Count);
                
                if (reviews.Count > 0)
                {
                    _logger.LogInformation("Deleting all related reviews before deleting products. BrandId: {BrandId}, Product reviews count: {Count}.", brandId, reviews.Count);

                    var deletedReviewTasks = reviews.Select(r => _productReviewRepository.DeleteAsync(r.Id,ct));
                    await Task.WhenAll(deletedReviewTasks);
                    _logger.LogInformation("All related reviews deleted.");
                }


                _logger.LogInformation("Clearing all related categories before deleting products.");

                products.ForEach(p => p.Categories.Clear());
                var updatedProductTasks = products.Select(p => _productRepository.UpdateAsync(p,ct));
                await Task.WhenAll(updatedProductTasks);
                _logger.LogInformation("All related categories cleared.");


                var deletedProductTasks = products.Select(p => _productRepository.DeleteAsync(p.Id, ct));
                await Task.WhenAll(deletedProductTasks);
                _logger.LogInformation("All related products deleted.");
            }

            _brandRepository.Remove(brand);
            await _brandRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Brand deleted. BrandId: {BrandId}.", brandId);

            return deletedBrand;
        }



    }
}
