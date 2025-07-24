using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.DTOs.ProductReview
{
    public class CreateProductReviewDto
    {
        [MinLength(2)]
        public string Title { get; set; } = "";

        public Guid ProductId { get; set; }

        public Guid UserId { get; set; }

        [MinLength(2)]
        public string UserName { get; set; } = "";

        [Range(0,5)]
        public uint Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = "";
    }
}
