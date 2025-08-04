using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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



        public async Task<IReadOnlyList<BrandDto>> GetAllBrandsAsync()
        {
            _logger.LogInformation("Retrieving all brands.");

            IReadOnlyList<Brand> brands = await _brandRepository.GetAllAsync();
            _logger.LogInformation("Retrieved all brands. Count: {Count.}", brands.Count);

            return _mapper.Map<List<BrandDto>>(brands);
        }



        public async Task<BrandDto?> GetBrandByIdAsync(Guid brandId)
        {
            _logger.LogInformation("Retrieving brand. BrandId: {BrandId}.", brandId);

            Brand? brand = await _brandRepository.FindByIdAsync(brandId);
            if (brand is null)
            {
                _logger.LogWarning("Brand not found. BrandId: {BrandId}.", brandId);
                return null;
            }

            _logger.LogInformation("Brand found. BrandId: {BrandId}.", brandId);

            return _mapper.Map<BrandDto>(brand);
        }




        public async Task<BrandDto> CreateBrandAsync(CreateUpdateBrandDto createDto)
        {
            _logger.LogInformation("Creating new brand. Title: {Title}.", createDto.Title);

            Brand brand = _mapper.Map<Brand>(createDto);
            brand.Id = default;
            brand.CreatedAt = DateTime.UtcNow;

            Brand addedBrand = await _brandRepository.InsertAsync(brand);
            _logger.LogInformation("Brand created. BrandId: {BrandId}.", addedBrand.Id);

            return _mapper.Map<BrandDto>(addedBrand);
        }




        public async Task<BrandDto?> UpdateBrandAsync(Guid brandId, CreateUpdateBrandDto updateDto)
        {
            _logger.LogInformation("Updating brand. BrandId: {BrandId}", brandId);

            Brand? brandDb = await _brandRepository.FindByIdAsync(brandId);
            if (brandDb is null)
            {
                _logger.LogWarning("Cannot update. Brand not found. BrandId: {BrandId}.", brandId);
                return null;
            }

            _mapper.Map<CreateUpdateBrandDto, Brand>(updateDto, brandDb);

            brandDb.UpdatedAt = DateTime.UtcNow;

            Brand updatedBrand = await _brandRepository.UpdateAsync(brandDb);
            _logger.LogInformation("Brand updated. BrandId: {BrandId}.", brandId);

            return _mapper.Map<BrandDto>(updatedBrand);
        }




        public async Task<BrandDto?> DeleteBrandAsync(Guid brandId)
        {
            _logger.LogInformation("Deleting brand. BrandId: {BrandId}.", brandId);

            Brand? brand = await _brandRepository.FindBrandByIdWithIncludes(brandId);
            if (brand is null)
            {
                _logger.LogWarning("Cannot delete. Brand not foud. BrandId: {BrandId}.", brandId);
                return null;
            }

            BrandDto deletedBrand = _mapper.Map<BrandDto>(brand);


            var products = brand.Products.ToList();
            var reviews = brand.Products.SelectMany(p => p.Reviews).ToList();


            if (products.Count > 0)
            {
                _logger.LogInformation("Deleting all related products before deleting brand. BrandId: {BrandId}, Product count: {Count}.", brandId, products.Count);


                if (reviews.Count > 0)
                {
                    _logger.LogInformation("Deleting all related reviews before deleting products. BrandId: {BrandId}, Product reviews count: {Count}.", brandId, reviews.Count);

                    var deletedReviewTasks = reviews.Select(r => _productReviewRepository.DeleteAsync(r.Id));
                    await Task.WhenAll(deletedReviewTasks);
                    _logger.LogInformation("All related reviews deleted.");
                }


                _logger.LogInformation("Clearing all related categories before deleting products.");

                products.ForEach(p => p.Categories.Clear());
                var updatedProductTasks = products.Select(_productRepository.UpdateAsync);
                await Task.WhenAll(updatedProductTasks);
                _logger.LogInformation("All related categories cleared.");


                var deletedProductTasks = products.Select(p => _productRepository.DeleteAsync(p.Id));
                await Task.WhenAll(deletedProductTasks);
                _logger.LogInformation("All related products deleted.");
            }


            await _brandRepository.DeleteAsync(brandId);
            _logger.LogInformation("Brand deleted. BrandId: {BrandId}.", brandId);

            return deletedBrand;
        }



    }
}
