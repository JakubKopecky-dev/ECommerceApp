using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = "";

        public Guid UserId { get; set; }

        public string Message { get; set; } = "";

        public NotificationType Type { get; set; } 

        public DateTime CreatedAt { get; set; }
    }

}
