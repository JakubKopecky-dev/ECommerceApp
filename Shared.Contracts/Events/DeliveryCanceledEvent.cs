namespace Shared.Contracts.Events
{
    public class DeliveryCanceledEvent
    {
       public Guid OrderId { get; set; }

       public Guid UserId { get; set; }

    }
}
