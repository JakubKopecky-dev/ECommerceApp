using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs.Category
{
    public class CreateUpdateCategoryDto
    {
        [MinLength(2)]
        public string Title { get; set; } = "";
    }
}
