using ProductService.Domain.Common;

namespace ProductService.Domain.Entity
{
    public class Brand : BaseEntity
    {
        public string Description { get; set; } = "";

        public ICollection<Product> Products { get; set; } = [];

    }
}
