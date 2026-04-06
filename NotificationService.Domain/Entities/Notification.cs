using NotificationService.Domain.Common;
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


        private Notification() { }  


        public static Notification Create(string title, Guid userId, string message, NotificationType type)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Title is required");

            if (string.IsNullOrWhiteSpace(message))
                throw new DomainException("Message is required");

            if (userId == Guid.Empty)
                throw new DomainException("UserId is required");


            return new()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Message = message,
                Type = type,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

        }









    }

}
