﻿using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Api.Hubs;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces.Services;
using Shared.Contracts.Events;
using NotificationService.Domain.Enum;

namespace NotificationService.Api.Consumers
{
    public class OrderCreatedConsumer(INotificationService notificationService, ILogger<OrderCreatedConsumer> logger, IHubContext<NotificationHub> hubContext) : IConsumer<OrderCreatedEvent>
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly ILogger<OrderCreatedConsumer> _logger = logger;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;



        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            OrderCreatedEvent message = context.Message;

            _logger.LogInformation("Received OrderCreatedEvent. OrderId: {OrderId}, UserId: {UserId}.", message.OrderId, message.UserId);


            CreateNofiticationDto createNotification = new()
            {
                UserId = message.UserId,
                Title = "Order created",
                Message = $"Your order #{message.OrderId} was successfully created. Total: {message.TotalPrice} $.",
                CreatedAt = message.CreatedAt,
                Type = NotificationType.OrderCreated
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
