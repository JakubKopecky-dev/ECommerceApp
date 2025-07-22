using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.Category;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;



        [HttpGet]
        public async Task<IReadOnlyList<CategoryDto>> GetAllCategories() => await _categoryService.GetAllCategoriesAsync();



        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategory(Guid categoryId)
        {
            CategoryDto? category = await _categoryService.GetCategoryByIdAsync(categoryId);

            return category is not null ? Ok(category) : NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateUpdateCategoryDto createDto)
        {
            CategoryDto category = await _categoryService.CreateCategoryAsync(createDto);

            return CreatedAtAction(nameof(CreateCategory), new { categoryId = category.Id }, category);
        }



        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpadteCategory(Guid categoryId, [FromBody] CreateUpdateCategoryDto updateDto)
        {
            CategoryDto? category = await _categoryService.UpdateCategoryAsync(categoryId, updateDto);

            return category is not null ? Ok(category) : NotFound();
        }



        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            CategoryDto? category = await _categoryService.DeleteCategoryAsync(categoryId);

            return category is not null ? Ok(category) : NotFound();
        }





    }
}
