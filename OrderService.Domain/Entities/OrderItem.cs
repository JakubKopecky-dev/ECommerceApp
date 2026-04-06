using OrderService.Domain.Common;

namespace OrderService.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid ProductId { get; private set; }

        public string ProductName { get; private set; } = "";

        public decimal UnitPrice { get; private set; }

        public uint Quantity { get; private set; }

        public Guid OrderId { get; private set; }
        public Order? Order { get; private set; }



        private OrderItem() { }


        public static OrderItem Create(Guid productId, string productName, decimal unitPrice, uint quantity, Guid orderId)
        {
            ValidateProductId(productId);
            ValidateProductName(productName);
            ValidateUnitPrice(unitPrice);
            ValidateOrderId(orderId);

            return new()
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ProductName = productName,
                UnitPrice = unitPrice,
                Quantity = quantity,
                OrderId = orderId,
                CreatedAt = DateTime.UtcNow
            };
        }



        public static OrderItem CreateFromOrder(Guid productId, string productName, decimal unitPrice, uint quantity)
        {
            ValidateProductId(productId);
            ValidateProductName(productName);
            ValidateUnitPrice(unitPrice);

            return new()
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ProductName = productName,
                UnitPrice = unitPrice,
                Quantity = quantity,
                CreatedAt = DateTime.UtcNow
            };
        }


        public void ChangeQuantity(uint quaitity)
        {
            if (quaitity < 0)
                throw new DomainException("Quantity must be greater than zero");

            Quantity = quaitity;
            UpdatedAt = DateTime.UtcNow;
        }



        private static void ValidateOrderId(Guid orderId)
        {
            if (orderId == Guid.Empty)
                throw new DomainException("OrderId is required");
        }


        private static void ValidateProductId(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new DomainException("ProductId is required");
        }


        private static void ValidateProductName(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new DomainException("ProductName is required");

            if (productName.Length > 150)
                throw new DomainException("ProductName is too long");
        }

        private static void ValidateUnitPrice(decimal unitPrice)
        {
            if (unitPrice <= 0)
                throw new DomainException("UnitPrice must be greater than zero");
        }

    }








}
