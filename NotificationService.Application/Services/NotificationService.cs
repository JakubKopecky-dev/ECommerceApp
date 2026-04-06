using Microsoft.Extensions.Logging;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Services
{
    public class NotificationService(INotificationRepository notificationRepository, ILogger<NotificationService> logger) : INotificationService
    {
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        private readonly ILogger<NotificationService> _logger = logger;



        public async Task<IReadOnlyList<NotificationDto>> GetAllNotificationByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all notification by UserId. UserId: {UserId}.", userId);

            IReadOnlyList<Notification> notifications = await _notificationRepository.GetAllNotificationsByUserIdAsync(userId, ct);
            _logger.LogInformation("Retrieved all notofication by userId. Count: {Count}, UserId: {UserId}.", notifications.Count, userId);

            return [.. notifications.Select(x => x.NotificationToNotificationDto())];
        }



        public async Task<NotificationDto?> GetNotificationAsync(Guid notificationId, CancellationToken ct = default)
        {
            Notification? notification = await _notificationRepository.FindByIdAsync(notificationId, ct);
            if (notification is null)
                _logger.LogWarning("Notification not found. NotificationId: {NotificationId}", notificationId);

            else
                _logger.LogInformation("Notification found. NotificationId: {NotificationId}.", notificationId);

            return notification?.NotificationToNotificationDto();
        }



        public async Task<NotificationDto> CreateNotificationAsync(CreateNofiticationDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating notification. UserId: {UserId}.", createDto.UserId);

            Notification notification = Notification.Create(createDto.Title, createDto.UserId, createDto.Message, createDto.Type);

            await _notificationRepository.AddAsync(notification, ct);
            await _notificationRepository.SaveChangesAsync(ct); 

            _logger.LogInformation("Notification created. NotificationId: {NotificationId}, UserId: {UserId}.", notification.Id, notification.UserId);

            return notification.NotificationToNotificationDto();
        }



    }
}
