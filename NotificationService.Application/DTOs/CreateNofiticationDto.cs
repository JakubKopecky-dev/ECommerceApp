using NotificationService.Domain.Enums;

namespace NotificationService.Application.DTOs
{
    public sealed record CreateNofiticationDto
    {
        public Guid UserId { get; init; }

        public string Title { get; init; } = "";

        public string Message { get; init; } = "";

        public NotificationType Type { get; init; }

        public DateTime CreatedAt { get; init; }

    }
}
