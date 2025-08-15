namespace CartService.Application.DTOs.Cart
{
    public sealed record CreateCartDto
    {
        public Guid UserId { get; init; }

    }
}
