using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationService.Domain.Enum;

namespace NotificationService.Application.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = "";

        public Guid UserId { get; set; }

        public string Message { get; set; } = "";

        public NotificationType Type { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
