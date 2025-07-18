using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Domain.Common;

namespace ProductService.Domain.Entity
{
    public class Product : BaseEntity
    {
        public string Description { get; set; } = "";

        public uint QuantityInStock { get; set; }

        public bool IsActive { get; set; }

        public uint SoldCount { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = "";


        public ICollection<Category> Categories = [];

        public Guid BrandId { get; set; }
        public required Brand Brand { get; set; }

    }
}
