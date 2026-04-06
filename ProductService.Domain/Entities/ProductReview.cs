using ProductService.Domain.Common;

namespace ProductService.Domain.Entities
{
    public class ProductReview : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }

        public Guid UserId { get; private set; }

        public string UserName { get; private set; } = "";

        public uint Rating { get; private set; }

        public string Comment { get; private set; } = "";


        private ProductReview() { }




        public static ProductReview Create(string title, Guid productId, uint rating, string comment, Guid userId, string userName)
        {
            ValidateTitle(title);
            ValidateProductId(productId);
            ValidateUserId(userId);
            ValidateComment(comment);
            ValidateRating(rating);

            return new()
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Title = title,
                UserId = userId,
                Comment = comment,
                Rating = rating,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow,
            };

        }


        public void Update(string title, uint rating, string comment)
        {
            ValidateTitle(title);
            ValidateRating(rating);
            ValidateComment(comment);

            Title = title;
            Rating = rating;
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;
        }


        private static void ValidateComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("Comment is required");

            if (comment.Length > 2000)
                throw new DomainException("Comment is too long");
        }


        private static void ValidateRating(uint rating)
        {
            if (rating > 5)
                throw new DomainException("Rating cannot be greater then 5");
        }


        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId is required");
        }


        private static void ValidateProductId(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new DomainException("ProductId is required");
        }



    }
}
