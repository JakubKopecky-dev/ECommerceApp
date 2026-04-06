namespace OrderService.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }

        public DateTime CreatedAt { get;protected set; }

        public DateTime? UpdatedAt { get;protected set; }

        private readonly List<IDomainEvent> _domainEvents = [];

        public void AddDomainEvent(IDomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);



        public IReadOnlyCollection<IDomainEvent> PopDomainEvents()
        {
            var events = _domainEvents.ToList();
            _domainEvents.Clear();

            return events;
        }
    }
}
