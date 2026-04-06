using ProductService.Domain.Common;

namespace ProductService.Domain.Entities
{
    public class Category : BaseEntity
    {
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        private readonly List<Product> _products = [];


        private Category() { }


        public static Category Create(string title)
        {
            ValidateTitle(title);

            return new()
            {
                Id = Guid.NewGuid(),
                Title = title,
                CreatedAt = DateTime.UtcNow
            };
        }



        public void Update(string title)
        {
            ValidateTitle(title);

            Title = title;
            UpdatedAt = DateTime.UtcNow;
        }



    }
}
