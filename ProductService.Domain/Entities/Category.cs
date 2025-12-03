using ProductService.Domain.Common;

namespace ProductService.Domain.Entities
{
    public class Category : BaseEntity
    {
        public ICollection<Product> Products { get; set; } = [];

    }
}
