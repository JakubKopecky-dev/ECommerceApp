namespace ProductService.Application.DTOs.Product
{
    public sealed record ProductDto
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = "";

        public string Description { get; init; } = "";

        public uint QuantityInStock { get; init; }

        public bool IsActive { get; init; }

        public uint SoldCount { get; init; }

        public decimal Price { get; init; }

        public string ImageUrl { get; init; } = "";

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }

        public Guid BrandId { get; init; }


    }
}
