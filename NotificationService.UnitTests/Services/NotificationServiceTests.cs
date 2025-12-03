using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Application.Services;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationServiceService = NotificationService.Application.Services.NotificationService;

namespace NotificationService.UnitTests.Services
{
    public class NotificationServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllNotificationByUserIdAsync_ReturnsNotificationDtoList_WhenExists()
        {
            Guid userId = Guid.NewGuid();

            List<Notification> notifications =
            [
                new() { Id = Guid.NewGuid(), UserId = userId, Title = "New order", Message = "Order is created", Type = NotificationType.OrderCreated },
                new() { Id = Guid.NewGuid(), UserId = userId, Title = "Delivery status changed", Message = "New delivery status", Type = NotificationType.DeliveryStatusChanged }
            ];

            List<NotificationDto> expectedDto =
            [
                new() { Id = notifications[0].Id, UserId = userId, Title = "New order", Message = "Order is created", Type = NotificationType.OrderCreated },
                new() { Id = notifications[1].Id, UserId = userId, Title = "Delivery status changed", Message = "New delivery status", Type = NotificationType.DeliveryStatusChanged }
            ];

            Mock<INotificationRepository> notificationRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            notificationRepositoryMock
                .Setup(n => n.GetAllNotificationsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notifications);

            mapperMock
                .Setup(m => m.Map<List<NotificationDto>>(notifications))
                .Returns(expectedDto);

            NotificationServiceService service = new(
                notificationRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<NotificationServiceService>>().Object
            );


            IReadOnlyList<NotificationDto> result = await service.GetAllNotificationByUserIdAsync(userId);

            result.Should().BeEquivalentTo(expectedDto);

            notificationRepositoryMock.Verify(n => n.GetAllNotificationsByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<NotificationDto>>(notifications), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllNotificationByUserIdAsync_ReturnsEmptyList_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();
            List<Notification> notifications = [];
            List<NotificationDto> expectedDto = [];

            Mock<INotificationRepository> notificationRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            notificationRepositoryMock
                .Setup(n => n.GetAllNotificationsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notifications);

            mapperMock
                .Setup(m => m.Map<List<NotificationDto>>(notifications))
                .Returns(expectedDto);

            NotificationServiceService service = new(
                notificationRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<NotificationServiceService>>().Object
            );


            IReadOnlyList<NotificationDto> result = await service.GetAllNotificationByUserIdAsync(userId);

            result.Should().BeEmpty();

            notificationRepositoryMock.Verify(n => n.GetAllNotificationsByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<NotificationDto>>(notifications), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetNotificationAsync_ReturnsNotificationDto_WhenExists()
        {
            Guid notificationId = Guid.NewGuid();

            Notification notification = new() { Id = notificationId, UserId = Guid.NewGuid(), Title = "New order", Message = "Order is created", Type = NotificationType.OrderCreated };
            NotificationDto expectedDto = new() { Id = notificationId, UserId = notification.UserId, Title = "New order", Message = "Order is created", Type = NotificationType.OrderCreated };

            Mock<INotificationRepository> notificationRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            notificationRepositoryMock
                .Setup(n => n.FindByIdAsync(notificationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notification);

            mapperMock
                .Setup(m => m.Map<NotificationDto>(notification))
                .Returns(expectedDto);

            NotificationServiceService service = new(
                notificationRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<NotificationServiceService>>().Object
            );


            NotificationDto? result = await service.GetNotificationAsync(notificationId);

            result.Should().BeEquivalentTo(expectedDto);

            notificationRepositoryMock.Verify(n => n.FindByIdAsync(notificationId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<NotificationDto>(notification), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetNotificationAsync_ReturnsNull_WhenNotExists()
        {
            Guid notificationId = Guid.NewGuid();

            Mock<INotificationRepository> notificationRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            notificationRepositoryMock
                .Setup(n => n.FindByIdAsync(notificationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Notification?)null);

            NotificationServiceService service = new(
                notificationRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<NotificationServiceService>>().Object
            );


            NotificationDto? result = await service.GetNotificationAsync(notificationId);

            result.Should().BeNull();

            notificationRepositoryMock.Verify(n => n.FindByIdAsync(notificationId, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<NotificationDto>(It.IsAny<Notification>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateNotificationAsync_ReturnsNotificationDto()
        {
            CreateNofiticationDto createDto = new() { UserId = Guid.NewGuid(), Title = "New order", Message = "Order is created", Type = NotificationType.OrderCreated };

            Notification notification = new() { Id = Guid.Empty, UserId = createDto.UserId, Title = createDto.Title, Message = createDto.Message, Type = createDto.Type };
            Notification createdNotification = new() { Id = Guid.NewGuid(), UserId = createDto.UserId, Title = createDto.Title, Message = createDto.Message, Type = createDto.Type };
            NotificationDto expectedDto = new() { Id = createdNotification.Id, UserId = createDto.UserId, Title = createDto.Title, Message = createDto.Message, Type = createDto.Type };

            Mock<INotificationRepository> notificationRepositoryMock = new();
            Mock<IMapper> mapperMock = new();

            mapperMock
                .Setup(m => m.Map<Notification>(createDto))
                .Returns(notification);

            notificationRepositoryMock
                .Setup(n => n.InsertAsync(notification, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdNotification);

            mapperMock
                .Setup(m => m.Map<NotificationDto>(createdNotification))
                .Returns(expectedDto);

            NotificationServiceService service = new(
                notificationRepositoryMock.Object,
                mapperMock.Object,
                new Mock<ILogger<NotificationServiceService>>().Object
            );


            NotificationDto result = await service.CreateNotificationAsync(createDto);

            result.Should().BeEquivalentTo(expectedDto);

            mapperMock.Verify(m => m.Map<Notification>(createDto), Times.Once);
            notificationRepositoryMock.Verify(n => n.InsertAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<NotificationDto>(createdNotification), Times.Once);
        }
    }
}
