using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application
{
    public static class Mapper
    {

        public static NotificationDto NotificationToNotificationDto(this Notification notification) =>
            new()
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                UserId = notification.UserId,
                CreatedAt = notification.CreatedAt
            };



    }
}
