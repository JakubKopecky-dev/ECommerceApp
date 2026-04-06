using ProductService.Domain.Common;
using ProductService.Domain.ValueObject;
using System.Globalization;

namespace ProductService.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Description { get; private set; } = "";

        public uint QuantityInStock { get; private set; }

        public bool IsActive { get; private set; }

        public uint SoldCount { get; private set; }

        public decimal Price { get; private set; }

        public ImageUrl ImageUrl { get; private set; } = null!;


        public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();
        private readonly List<Category> _categories = [];


        public IReadOnlyCollection<ProductReview> Reviews => _reviews.AsReadOnly();
        private readonly List<ProductReview> _reviews = [];

        public Guid BrandId { get; private set; }
        public Brand? Brand { get; private set; }



        public static Product Create(string title, string description, decimal price, string imageUrl, Guid brandId)
        {
            ValidateTitle(title);
            ValidateDescription(description);
            ValidatePrice(price);
            ValidateBrandId(brandId);

            return new()
            {
                Id = Guid.NewGuid(),
                IsActive = false,
                Title = title,
                Description = description,
                Price = price,
                ImageUrl = new ImageUrl(imageUrl),
                BrandId = brandId,
                CreatedAt = DateTime.UtcNow,
            };

        }




        public void Update(string title, string description, decimal price, string imageUrl, Guid brandId)
        {
            ValidateTitle(title);
            ValidateDescription(description);
            ValidatePrice(price);
            ValidateBrandId(brandId);

            Title = title;
            Description = description;
            Price = price;
            ImageUrl = new ImageUrl(imageUrl);
            BrandId = brandId;
            UpdatedAt = DateTime.UtcNow;
        }


        public void AddStock(uint quantity)
        {
            UpdatedAt = DateTime.UtcNow;
            QuantityInStock += quantity;
        }


        public void ReduceStock(uint quantity)
        {
            if (quantity > QuantityInStock)
                throw new DomainException("Not enough stock");

            QuantityInStock -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }


        public void IncreaseSoldCount(uint quantity)
        {
            SoldCount += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetCategories(IEnumerable<Category> categories)
        {
            _categories.Clear();
            _categories.AddRange(categories);
            UpdatedAt = DateTime.UtcNow;
        }


        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }


        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }





        private static void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description is required");
        }


        private static void ValidatePrice(decimal price)
        {
            if (price <= 0)
                throw new DomainException("UnitPrice must be greater than zero");

        }


        private static void ValidateBrandId(Guid brandId)
        {
            if (brandId == Guid.Empty)
                throw new DomainException("BrandId is required");
        }










    }
}
