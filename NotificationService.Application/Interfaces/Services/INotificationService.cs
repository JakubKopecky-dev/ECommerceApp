using NotificationService.Application.DTOs;

namespace NotificationService.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateNotificationAsync(CreateNofiticationDto createDto, CancellationToken ct = default);
        Task<IReadOnlyList<NotificationDto>> GetAllNotificationByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<NotificationDto?> GetNotificationAsync(Guid notificationId, CancellationToken ct = default);
    }
}
