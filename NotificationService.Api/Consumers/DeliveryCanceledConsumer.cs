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
            _logger.LogInformation("Received DeliveryCancaledEvent");

            var ct = context.CancellationToken;

            DeliveryCanceledEvent message = context.Message;

            CreateNofiticationDto createNotification = new()
            {
                UserId = message.UserId,
                Title = "Delivery canceled",
                Message = $"Your order #{message.OrderId} delivery was canceled.",
                CreatedAt = DateTime.UtcNow,
                Type = NotificationType.DeliveryStatusChanged
            };

            NotificationDto notification = await _notificationService.CreateNotificationAsync(createNotification, ct);

            await _hubContext.Clients.User(message.UserId.ToString())
                .SendAsync("ReceiveNotification", new NotificationSiganlRDto
                {
                    Id = notification.Id,
                    Title = notification.Title,
                    Message = notification.Message,
                    CreatedAt = notification.CreatedAt,
                }, ct);

            _logger.LogInformation("Notification created and sent via SignalR. NotificationId: {NotificationId}", notification.Id);
        }



    }
}
