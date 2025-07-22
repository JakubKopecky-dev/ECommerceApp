using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Domain.Entity;

namespace ProductService.Application.DTOs.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public uint QuantityInStock { get; set; }

        public bool IsActive { get; set; }

        public uint SoldCount { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid BrandId { get; set; }


    }
}
