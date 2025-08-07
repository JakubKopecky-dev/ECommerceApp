using ProductService.Application.DTOs.Category;

namespace ProductService.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateUpdateCategoryDto createDto);
        Task<CategoryDto?> DeleteCategoryAsync(Guid categoryId);
        Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId);
        Task<CategoryDto?> UpdateCategoryAsync(Guid categoryId, CreateUpdateCategoryDto updateDto);
    }
}
