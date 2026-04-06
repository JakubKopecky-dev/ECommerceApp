namespace ProductService.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }

        public string Title { get; protected set; } = "";

        public DateTime CreatedAt { get;protected set; }

        public DateTime? UpdatedAt { get;protected set; }



        protected static void ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Title is required");

            if (title.Length > 150)
                throw new DomainException("Title is too long");
        }

    }
}
