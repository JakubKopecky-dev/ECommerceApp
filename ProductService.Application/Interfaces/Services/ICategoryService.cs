using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
