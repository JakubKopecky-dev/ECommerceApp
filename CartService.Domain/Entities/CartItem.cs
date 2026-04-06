using CartService.Domain.Common;
using System.Text.Json.Serialization;

namespace CartService.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public Guid CartId { get; private set; }
        public Cart? Cart { get; private set; }

        public Guid ProductId { get; private set; }

        public string ProductName { get; private set; } = "";

        public decimal UnitPrice { get; private set; }

        public uint Quantity { get; private set; }


        private CartItem() { }


        public static CartItem Create(Guid cartId, Guid producetId, string productName, decimal unitPrice, uint quantity)
        {
            ValidateCartId(cartId);
            ValidateProductId(producetId);
            ValidateProductName(productName);
            ValidateUnitPrice(unitPrice);


            return new()
            {
                Id = Guid.NewGuid(),
                ProductId = producetId,
                ProductName = productName,
                UnitPrice = unitPrice,
                Quantity = quantity,
                CartId = cartId,
                CreatedAt = DateTime.UtcNow
            };
        }



        public void ChangeQuantity(uint quantity)
        {
            Quantity = quantity;
            UpdatedAt = DateTime.UtcNow;
        }



        private static void ValidateCartId(Guid cartId)
        {
            if (cartId == Guid.Empty)
                throw new DomainException("CartId is required");
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
