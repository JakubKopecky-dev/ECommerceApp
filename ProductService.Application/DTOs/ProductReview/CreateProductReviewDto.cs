using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs.ProductReview
{
    public class CreateProductReviewDto
    {
        [MinLength(2)]
        public string Title { get; set; } = "";

        public Guid ProductId { get; set; }

        [Range(0,5)]
        public uint Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = "";
    }
}
