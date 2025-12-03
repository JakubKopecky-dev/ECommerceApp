using OrderService.Domain.Common;
using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public InternalOrderStatus InternalStatus { get; set; }

        public string? Note { get; set; }

        public Guid? DeliveryId { get; set; }

        public ICollection<OrderItem> Items { get; set; } = [];
    }
}
