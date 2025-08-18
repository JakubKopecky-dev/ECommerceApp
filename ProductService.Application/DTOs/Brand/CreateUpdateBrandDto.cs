using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs.Brand
{
    public sealed record CreateUpdateBrandDto
    {
        [MaxLength(500)]
        public string Title { get; init; } = "";

        [MaxLength(2000)]
        public string Description { get; init; } = "";

    }
}
