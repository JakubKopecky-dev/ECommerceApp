using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.DTOs.ProductReview
{
    public class ProductReviewDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = "";

        public Guid ProductId { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; } = "";

        public uint Rating { get; set; }

        public string Comment { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
