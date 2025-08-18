using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Category;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entity;

namespace ProductService.Application.Services
{
    public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger) : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CategoryService> _logger = logger;




        public async Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all categories.");

            IReadOnlyList<Category> categories = await _categoryRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all categories. Count: {Count}.", categories.Count);

            return _mapper.Map<List<CategoryDto>>(categories);
        }



        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving category. CategoryId: {CategoryId}.", categoryId);

            Category? category = await _categoryRepository.FindByIdAsync(categoryId,ct);
            if (category is null)
            {
                _logger.LogWarning("Category not found. CategoryId: {CategoryId}.", categoryId);
                return null;
            }

            _logger.LogInformation("Category found. CategoryId: {CategoryId}.", categoryId);

            return _mapper.Map<CategoryDto>(category);
        }



        public async Task<CategoryDto> CreateCategoryAsync(CreateUpdateCategoryDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new category. Title: {Title}.", createDto.Title);

            Category category = _mapper.Map<Category>(createDto);
            category.Id = Guid.Empty;
            category.CreatedAt = DateTime.UtcNow;

            Category addedCategory = await _categoryRepository.InsertAsync(category,ct);
            _logger.LogInformation("Category created. CategoryId: {CategoryId}.", addedCategory.Id);

            return _mapper.Map<CategoryDto>(addedCategory);
        }



        public async Task<CategoryDto?> UpdateCategoryAsync(Guid categoryId, CreateUpdateCategoryDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating category. CategoryId: {CategoryId}.", categoryId);

            Category? categoryDb = await _categoryRepository.FindByIdAsync(categoryId, ct);
            if (categoryDb is null)
            {
                _logger.LogWarning("Cannot update. Category not foud. CategoryId: {CategoryId}.", categoryId);
                return null;
            }

            _mapper.Map<CreateUpdateCategoryDto, Category>(updateDto, categoryDb);

            categoryDb.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Category updated. CategoryId: {CategoryId}.", categoryId);

            return _mapper.Map<CategoryDto>(categoryDb);
        }



        public async Task<CategoryDto?> DeleteCategoryAsync(Guid categoryId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting category. CategoryId: {CategoryId}.", categoryId);

            Category? category = await _categoryRepository.FindCategoryByIdWithIncludeProductsAsync(categoryId, ct);
            if (category is null)
            {
                _logger.LogWarning("Cannot delete. Category not foud. CategoryId: {CategoryId}.", categoryId);
                return null;
            }

            CategoryDto deletedCategory = _mapper.Map<CategoryDto>(category);

            _logger.LogInformation("Clearing all related products");

            category.Products.Clear();
            _categoryRepository.Remove(category);

            await _categoryRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Category deleted. CategoryId: {CategoryId}.", categoryId);

            return deletedCategory;
        }



    }
}
