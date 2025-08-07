namespace OrderService.Application.DTOs.OrderItem
{
    public class OrderItemForExtendedDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }

    }
}
