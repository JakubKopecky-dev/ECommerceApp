namespace Shared.Contracts.Events
{
    // unused for now (can be used for orderItems in notification (probably mail))
    public class OrderItemEvent
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }

    }
}
