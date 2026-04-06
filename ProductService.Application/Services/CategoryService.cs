using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Category;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entities;

namespace ProductService.Application.Services
{
    public class CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger) : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly ILogger<CategoryService> _logger = logger;




        public async Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all categories.");

            IReadOnlyList<Category> categories = await _categoryRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all categories. Count: {Count}.", categories.Count);

            return [.. categories.Select(x => x.CategoryToCategoryDto())];
        }



        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving category. CategoryId: {CategoryId}.", categoryId);

            Category? category = await _categoryRepository.FindByIdAsync(categoryId, ct);
            if (category is null)
                _logger.LogWarning("Category not found. CategoryId: {CategoryId}.", categoryId);
            else
                _logger.LogInformation("Category found. CategoryId: {CategoryId}.", categoryId);

            return category?.CategoryToCategoryDto();
        }



        public async Task<CategoryDto> CreateCategoryAsync(CreateUpdateCategoryDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new category. Title: {Title}.", createDto.Title);

            Category category = Category.Create(createDto.Title);

            await _categoryRepository.AddAsync(category, ct);
            await _categoryRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Category created. CategoryId: {CategoryId}.", category.Id);

            return category.CategoryToCategoryDto();
        }



        public async Task<CategoryDto?> UpdateCategoryAsync(Guid categoryId, CreateUpdateCategoryDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating category. CategoryId: {CategoryId}.", categoryId);

            Category? category = await _categoryRepository.FindByIdAsync(categoryId, ct);
            if (category is null)
            {
                _logger.LogWarning("Cannot update. Category not foud. CategoryId: {CategoryId}.", categoryId);
                return null;
            }

            category.Update(updateDto.Title);

            await _categoryRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Category updated. CategoryId: {CategoryId}.", categoryId);

            return category.CategoryToCategoryDto();
        }



        public async Task<bool> DeleteCategoryAsync(Guid categoryId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting category. CategoryId: {CategoryId}.", categoryId);

            Category? category = await _categoryRepository.FindCategoryByIdWithIncludeProductsAsync(categoryId, ct);
            if (category is null)
            {
                _logger.LogWarning("Cannot delete. Category not foud. CategoryId: {CategoryId}.", categoryId);
                return false;
            }

            _categoryRepository.Remove(category);

            await _categoryRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Category deleted. CategoryId: {CategoryId}.", categoryId);

            return true;
        }



    }
}
