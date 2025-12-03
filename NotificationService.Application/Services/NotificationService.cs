using AutoMapper;
using Microsoft.Extensions.Logging;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Services
{
    public class NotificationService(INotificationRepository notificationRepository, IMapper mapper, ILogger<NotificationService> logger) : INotificationService
    {
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<NotificationService> _logger = logger;



        public async Task<IReadOnlyList<NotificationDto>> GetAllNotificationByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all notification by UserId. UserId: {UserId}.", userId);

            IReadOnlyList<Notification> notifications = await _notificationRepository.GetAllNotificationsByUserIdAsync(userId,ct);
            _logger.LogInformation("Retrieved all notofication by userId. Count: {Count}, UserId: {UserId}.", notifications.Count, userId);

            return _mapper.Map<List<NotificationDto>>(notifications);
        }



        public async Task<NotificationDto?> GetNotificationAsync(Guid notificationId, CancellationToken ct = default)
        {
            Notification? notification = await _notificationRepository.FindByIdAsync(notificationId,ct);
            if (notification is null)
            {
                _logger.LogWarning("Notification not found. NotificationId: {NotificationId}", notificationId);
                return null;
            }

            _logger.LogInformation("Notification found. NotificationId: {NotificationId}.", notificationId);

            return _mapper.Map<NotificationDto>(notification);
        }



        public async Task<NotificationDto> CreateNotificationAsync(CreateNofiticationDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating notification. UserId: {UserId}.", createDto.UserId);

            Notification notification = _mapper.Map<Notification>(createDto);

            Notification createdNotification = await _notificationRepository.InsertAsync(notification, ct);
            _logger.LogInformation("Notification created. NotificationId: {NotificationId}, UserId: {UserId}.", createdNotification.Id, createdNotification.UserId);

            return _mapper.Map<NotificationDto>(createdNotification);
        }



    }
}
