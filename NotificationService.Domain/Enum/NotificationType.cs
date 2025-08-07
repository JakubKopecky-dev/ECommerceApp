namespace NotificationService.Domain.Enum
{
    public enum NotificationType
    {
        General,              // General system notification
        OrderCreated,         // Order has been created
        OrderStatusChanged,   // Order status updated (e.g., paid, shipped, completed)
        DeliveryStatusChanged,// Delivery status updated (e.g., dispatched, delivered)
        Promotion,            // Promotional offer or special deal
        AdminMessage,         // Message from admin or support
        SystemAlert           // System alert (maintenance, technical issue)
    }
}
