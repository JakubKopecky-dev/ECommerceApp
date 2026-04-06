using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs.Product
{
    public sealed record UpdateProductDto
    {
        [Length(2,150)]
        public string Title { get; init; } = "";

        public string Description { get; init; } = "";

        public decimal Price { get; init; }

        [Url]
        public string ImageUrl { get; init; } = "";

        public Guid BrandId { get; init; }


        public List<string> Categories { get; init; } = [];
    }
}
