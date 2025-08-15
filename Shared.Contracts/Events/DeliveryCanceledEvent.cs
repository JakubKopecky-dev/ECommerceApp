namespace Shared.Contracts.Events
{
    public sealed record DeliveryCanceledEvent
    {
       public Guid OrderId { get; set; }

       public Guid UserId { get; set; }

    }
}
