using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs.Brand
{
    public  class CreateUpdateBrandDto
    {
        [MinLength(2)]
        public string Title { get; set; } = "";

        [MaxLength(2000)]
        public string Description { get; set; } = "";

    }
}
