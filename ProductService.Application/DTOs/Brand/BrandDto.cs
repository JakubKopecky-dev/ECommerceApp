namespace ProductService.Application.DTOs.Brand
{
    public sealed record BrandDto
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = "";

        public string Description { get; init; } = "";

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }
    }
}
