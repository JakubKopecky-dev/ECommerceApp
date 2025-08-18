namespace Shared.Contracts.Events
{
    public sealed record DeliveryCanceledEvent
    {
       public Guid OrderId { get; init; }

       public Guid UserId { get; init; }

    }
}
