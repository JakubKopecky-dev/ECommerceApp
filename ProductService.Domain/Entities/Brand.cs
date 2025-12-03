using ProductService.Domain.Common;

namespace ProductService.Domain.Entities
{
    public class Brand : BaseEntity
    {
        public string Description { get; set; } = "";

        public ICollection<Product> Products { get; set; } = [];

    }
}
