using CartService.Domain.Common;

namespace CartService.Domain.Entity
{
    public class CartItem : BaseEntity
    {
        public required Cart Cart { get; set; }
        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }
    }
}
