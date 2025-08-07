using NotificationService.Domain.Entity;

namespace NotificationService.Application.Interfaces.Repositories
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetAllNotificationsByUserIdAsync(Guid userId);
    }
}
