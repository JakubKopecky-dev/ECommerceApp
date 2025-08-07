namespace OrderService.Application.DTOs.OrderItem
{
    public class ExternalCreateOrderItemDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }
    }
}
