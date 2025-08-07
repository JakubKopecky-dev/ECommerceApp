using ProductService.Domain.Common;

namespace ProductService.Domain.Entity
{
    public class Category : BaseEntity
    {
        public ICollection<Product> Products { get; set; } = [];

    }
}
