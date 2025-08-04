using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationService.Domain.Entity;

namespace NotificationService.Application.Interfaces.Repositories
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetAllNotificationsByUserIdAsync(Guid userId);
    }
}
