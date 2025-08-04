using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateNotificationAsync(CreateNofiticationDto createDto);
        Task<IReadOnlyList<NotificationDto>> GetAllNotificationByUserIdAsync(Guid userId);
        Task<NotificationDto?> GetNotificationAsync(Guid notificationId);
    }
}
