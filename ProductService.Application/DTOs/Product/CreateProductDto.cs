using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs.Product
{
    public class CreateProductDto
    {
        [MinLength(2)]
        public string Title { get; set; } = "";

        [MaxLength(2000)]
        public string Description { get; set; } = "";

        public uint QuantityInStock { get; set; }

        public bool IsActive { get; set; }

        public decimal Price { get; set; }

        // [Url]
        public string ImageUrl { get; set; } = "";

        public Guid BrandId { get; set; }


    }
}
