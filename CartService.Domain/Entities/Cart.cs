using CartService.Domain.Common;

namespace CartService.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get; set; }

        public ICollection<CartItem> Items { get; set; } = [];
    }
}
