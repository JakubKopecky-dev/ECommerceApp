using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.Category;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Enums;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;



        [HttpGet]
        public async Task<IReadOnlyList<CategoryDto>> GetAllCategories(CancellationToken ct) => await _categoryService.GetAllCategoriesAsync(ct);



        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategory(Guid categoryId, CancellationToken ct)
        {
            CategoryDto? category = await _categoryService.GetCategoryByIdAsync(categoryId, ct);

            return category is not null ? Ok(category) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateUpdateCategoryDto createDto, CancellationToken ct)
        {
            CategoryDto category = await _categoryService.CreateCategoryAsync(createDto, ct);

            return CreatedAtAction(nameof(GetCategory), new { categoryId = category.Id }, category);
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpadteCategory(Guid categoryId, [FromBody] CreateUpdateCategoryDto updateDto, CancellationToken ct)
        {
            CategoryDto? category = await _categoryService.UpdateCategoryAsync(categoryId, updateDto, ct);

            return category is not null ? Ok(category) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken ct)
        {
            CategoryDto? category = await _categoryService.DeleteCategoryAsync(categoryId, ct);

            return category is not null ? Ok(category) : NotFound();
        }



    }
}
