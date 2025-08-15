using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs.Category
{
    public sealed record CreateUpdateCategoryDto
    {
        [MinLength(2)]
        public string Title { get; init; } = "";
    }
}
