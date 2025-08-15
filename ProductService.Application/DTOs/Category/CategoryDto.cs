namespace ProductService.Application.DTOs.Category
{
    public sealed record CategoryDto
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = "";

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }
    }
}
