using OrderService.Domain.Common;
using OrderService.Domain.Enums;
using OrderService.Domain.Events;

namespace OrderService.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get;private set; }

        public decimal TotalPrice { get;private set; }

        public OrderStatus Status { get;private set; }

        public InternalOrderStatus InternalStatus { get;private set; }

        public string? Note { get;private set; }

        public Guid? DeliveryId { get;private set; }

        private readonly List<OrderItem> _items = [];
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();


        private Order() { }

        

        public static Order Create(Guid userId, string? note)
        {
            ValidateUserId(userId);
            ValidateNote(note);

            return new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Note = note,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow,

            };
        }



        public static Order CreateFromCart(Guid userId, decimal totalPrice, string? note, List<OrderItem> items)
        {
            ValidateUserId(userId);
            ValidateTotalPrice(totalPrice);
            ValidateNote(note);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TotalPrice = totalPrice,
                Note = note,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow,
            };

            order._items.AddRange(items);
            return order;
        }


        public void NoteUpdate(string? note)
        {
            ValidateNote(note);

            Note = note;
            UpdatedAt = DateTime.UtcNow;
        }


        public void ChangeStatus(OrderStatus newStatus)
        {
            if (!IsStatusChangeValid(Status, newStatus))
                throw new DomainException($"Cannot change status from {Status} to {newStatus}");

            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;


            AddDomainEvent(new OrderStatusChangedDomainEvent
            {
                OrderId = Id,
                UserId = UserId,
                NewStatus = newStatus
            });
        }



        public void ChangeInternalStatus(InternalOrderStatus status)
        {
            InternalStatus = status;
            UpdatedAt = DateTime.UtcNow;
        }


        private static bool IsStatusChangeValid(OrderStatus current, OrderStatus next)
        {
            return (current, next) switch
            {
                (OrderStatus.Created, OrderStatus.Paid) => true,
                (OrderStatus.Paid, OrderStatus.Accepted) => true,
                (OrderStatus.Accepted, OrderStatus.Shipped) => true,
                (OrderStatus.Shipped, OrderStatus.Completed) => true,
                (OrderStatus.Created, OrderStatus.Cancelled) => true,
                (OrderStatus.Paid, OrderStatus.Rejected) => true,
                _ => false
            };
        }


        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId is required");
        }


        private static void ValidateTotalPrice(decimal unitPrice)
        {
            if (unitPrice <= 0)
                throw new DomainException("TotalPrice must be greater than zero");
        }


        private static void ValidateNote(string? note)
        {
            if (note?.Length > 1000)
                throw new DomainException("Note is too long");
        }


        private static void ValidateDeliveryId(Guid deliveryId)
        {
            if (deliveryId == Guid.Empty)
                throw new DomainException("DeliveryId is required");
        }



    }
}
