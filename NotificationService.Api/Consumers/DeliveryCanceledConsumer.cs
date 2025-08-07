using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Api.Hubs;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Enum;
using Shared.Contracts.Events;

namespace NotificationService.Api.Consumers
{
    public class DeliveryCanceledConsumer(INotificationService notificationService, ILogger<DeliveryCanceledConsumer> logger, IHubContext<NotificationHub> hubContext) : IConsumer<DeliveryCanceledEvent>
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly ILogger<DeliveryCanceledConsumer> _logger = logger;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;



        public async Task Consume(ConsumeContext<DeliveryCanceledEvent> context)
        {
            DeliveryCanceledEvent message = context.Message;

            CreateNofiticationDto createNotification = new()
            {
                UserId = message.UserId,
                Title = "Order created",
                Message = $"Your order #{message.OrderId} delivery was canceled.",
                CreatedAt = DateTime.UtcNow,
                Type = NotificationType.DeliveryStatusChanged
            };

            NotificationDto notification = await _notificationService.CreateNotificationAsync(createNotification);

            await _hubContext.Clients.User(message.UserId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    notification.Id,
                    notification.Title,
                    notification.Message,
                    notification.CreatedAt,
                });

            _logger.LogInformation("Notification created and sent via SignalR. NotificationId: {NotificationId}", notification.Id);
        }



    }
}
