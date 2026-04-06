using ProductService.Domain.Common;

namespace ProductService.Domain.Entities
{
    public class Brand : BaseEntity
    {
        public string Description { get;private set; } = "";

        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
        private readonly List<Product> _products = [];


        private Brand() { }


        private static void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description is required");

            if (description.Length > 2000)
                throw new DomainException("Description is too long");
        }



        public static Brand Create(string title, string description)
        {
            ValidateDescription(description);
            ValidateTitle(title);

            return new()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                CreatedAt = DateTime.Now,
            };
        }



        public void Update(string title, string description)
        {
            ValidateDescription(description);
            ValidateTitle(title);

            Title = title;
            Description = description;
            UpdatedAt = DateTime.Now;
        }




    }
}
