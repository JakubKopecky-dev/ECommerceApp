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


        public ICollection<ProductReview> Reviews = [];

        public Guid BrandId { get; set; }
        public required Brand Brand { get; set; }

    }
}
