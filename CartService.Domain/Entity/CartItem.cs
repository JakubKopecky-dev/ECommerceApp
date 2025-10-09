using CartService.Domain.Common;
using System.Text.Json.Serialization;

namespace CartService.Domain.Entity
{
    public class CartItem : BaseEntity
    {
        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }
    }
}
