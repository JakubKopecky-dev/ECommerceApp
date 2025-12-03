using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Api.Hubs;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces.Services;
using Shared.Contracts.Events;
using NotificationService.Domain.Enums;

namespace NotificationService.Api.Consumers
{
    public class OrderStatusChangedConsumer(INotificationService notificationService, ILogger<OrderStatusChangedConsumer> logger, IHubContext<NotificationHub> hubContext) : IConsumer<OrderStatusChangedEvent>
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly ILogger<OrderStatusChangedConsumer> _logger = logger;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;



        public async Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
        {
            var ct = context.CancellationToken;

            OrderStatusChangedEvent message = context.Message;

            CreateNofiticationDto createNofiticationDto = new()
            {
                UserId = message.UserId,
                Title = "Order status changed",
                Message = $"The status of your order #{message.OrderId} has been updated to {message.NewStatus}.",
                CreatedAt = message.UpdatedAt,
                Type = NotificationType.OrderStatusChanged,
            };


            NotificationDto notification = await _notificationService.CreateNotificationAsync(createNofiticationDto, ct);

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
