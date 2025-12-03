using ProductService.Domain.Common;

namespace ProductService.Domain.Entities
{
    public class ProductReview : BaseEntity
    {
        public Guid ProductId { get; set; }
        public required Product Product { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; } = "";

        public uint Rating { get; set; }

        public string Comment { get; set; } = "";
    }
}
