namespace CartService.Application.DTOs.External
{
    public  class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }
    }
}
