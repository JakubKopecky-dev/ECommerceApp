using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.DTOs.ProductReview
{
    public class UpdateProductReviewDto
    {
        [MinLength(2)]
        public string Title { get; set; } = "";


        [Range(0, 5)]
        public uint Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = "";
    }
}
