namespace OrderService.Domain.Enum
{
    public enum OrderStatus
    {
        Created,    // The order was created by the customer
        Paid,       // The order has been paid by the customer
        Accepted,   // The order was accepted/confirmed by the merchant
        Rejected,   // The order was rejected/cancelled by the merchant (e.g., out of stock)
        Shipped,    // The order was shipped to the customer
        Completed,  // The order was delivered and completed by the customer
        Cancelled   // The order was cancelled by the customer
    }
}
