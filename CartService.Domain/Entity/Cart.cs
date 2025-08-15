using CartService.Domain.Common;

namespace CartService.Domain.Entity
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get; set; }

        public ICollection<CartItem> Items { get; set; } = [];
    }
}
