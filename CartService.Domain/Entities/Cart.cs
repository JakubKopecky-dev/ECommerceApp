using CartService.Domain.Common;

namespace CartService.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get; set; }

        private readonly List<CartItem> _items = [];
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        public decimal TotalPrice => Items.Sum(i => i.UnitPrice * i.Quantity);



        private Cart() { }

        public static Cart Create(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId is required");

            return new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
            };

        }







    }
}
