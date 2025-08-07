namespace CartService.Application.DTOs.External
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }

        public string? Note { get; set; }

        public decimal TotalPrice { get; set; }

        public List<CreateOrderItemDto> Items { get; set; } = [];
    }
}
