using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs.Category
{
    public sealed record CreateUpdateCategoryDto
    {
        [MaxLength(150)]
        public string Title { get; init; } = "";
    }
}
