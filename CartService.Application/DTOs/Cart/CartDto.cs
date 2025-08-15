namespace CartService.Application.DTOs.Cart
{
    public sealed record CartDto
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public decimal TotalPrice { get; init; }

    }
    
}
