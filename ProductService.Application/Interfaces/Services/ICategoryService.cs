using ProductService.Application.DTOs.Category;

namespace ProductService.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateUpdateCategoryDto createDto, CancellationToken ct = default);
        Task<CategoryDto?> DeleteCategoryAsync(Guid categoryId, CancellationToken ct = default);
        Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync(CancellationToken ct = default);
        Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, CancellationToken ct = default);
        Task<CategoryDto?> UpdateCategoryAsync(Guid categoryId, CreateUpdateCategoryDto updateDto, CancellationToken ct = default);
    }
}
