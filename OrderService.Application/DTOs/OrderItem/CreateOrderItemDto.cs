namespace OrderService.Application.DTOs.OrderItem
{
    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }

        public Guid OrderId { get; set; }

        public string ProductName { get; set; } = "";


    }
}
