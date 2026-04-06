using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Application;
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
                Notification.Create("New order",userId ,"Order is created", NotificationType.OrderCreated),
                Notification.Create("Delivery status changed",userId ,"New delivery status", NotificationType.DeliveryStatusChanged),
            ];

            List<NotificationDto> expectedDto =
            [
               notifications[0].NotificationToNotificationDto(),
               notifications[1].NotificationToNotificationDto()
            ];

            Mock<INotificationRepository> notificationRepositoryMock = new();

            notificationRepositoryMock
                .Setup(n => n.GetAllNotificationsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notifications);



            NotificationServiceService service = new(
                notificationRepositoryMock.Object,
                new Mock<ILogger<NotificationServiceService>>().Object
            );


            IReadOnlyList<NotificationDto> result = await service.GetAllNotificationByUserIdAsync(userId);

            result.Should().BeEquivalentTo(expectedDto);

            notificationRepositoryMock.Verify(n => n.GetAllNotificationsByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllNotificationByUserIdAsync_ReturnsEmptyList_WhenNotExists()
        {
            Guid userId = Guid.NewGuid();
            List<Notification> notifications = [];
            List<NotificationDto> expectedDto = [];

            Mock<INotificationRepository> notificationRepositoryMock = new();

            notificationRepositoryMock
                .Setup(n => n.GetAllNotificationsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notifications);

            NotificationServiceService service = new(
                notificationRepositoryMock.Object,
                new Mock<ILogger<NotificationServiceService>>().Object
            );


            IReadOnlyList<NotificationDto> result = await service.GetAllNotificationByUserIdAsync(userId);

            result.Should().BeEmpty();

            notificationRepositoryMock.Verify(n => n.GetAllNotificationsByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetNotificationAsync_ReturnsNotificationDto_WhenExists()
        {

            Notification notification = Notification.Create("New order", Guid.NewGuid(), "Order is created", NotificationType.OrderCreated);
;
            NotificationDto expectedDto = notification.NotificationToNotificationDto();

            Mock<INotificationRepository> notificationRepositoryMock = new();

            notificationRepositoryMock
                .Setup(n => n.FindByIdAsync(notification.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notification);


            NotificationServiceService service = new(
                notificationRepositoryMock.Object,
                new Mock<ILogger<NotificationServiceService>>().Object
            );

            NotificationDto? result = await service.GetNotificationAsync(notification.Id);

            result.Should().BeEquivalentTo(expectedDto);

            notificationRepositoryMock.Verify(n => n.FindByIdAsync(notification.Id, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetNotificationAsync_ReturnsNull_WhenNotExists()
        {
            Guid notificationId = Guid.NewGuid();

            Mock<INotificationRepository> notificationRepositoryMock = new();

            notificationRepositoryMock
                .Setup(n => n.FindByIdAsync(notificationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Notification?)null);

            NotificationServiceService service = new(
                notificationRepositoryMock.Object,
                new Mock<ILogger<NotificationServiceService>>().Object
            );


            NotificationDto? result = await service.GetNotificationAsync(notificationId);

            result.Should().BeNull();

            notificationRepositoryMock.Verify(n => n.FindByIdAsync(notificationId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateNotificationAsync_ReturnsNotificationDto()
        {
            CreateNofiticationDto createDto = new() { UserId = Guid.NewGuid(), Title = "New order", Message = "Order is created", Type = NotificationType.OrderCreated };

            Notification notification = Notification.Create(createDto.Title,createDto.UserId,createDto.Message,createDto.Type);
            NotificationDto expectedDto = notification.NotificationToNotificationDto();

            Mock<INotificationRepository> notificationRepositoryMock = new();


            notificationRepositoryMock
                .Setup(n => n.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            notificationRepositoryMock
                .Setup(n => n.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            NotificationServiceService service = new(
                notificationRepositoryMock.Object,
                new Mock<ILogger<NotificationServiceService>>().Object
            );


            NotificationDto result = await service.CreateNotificationAsync(createDto);

            result.Should().BeEquivalentTo(expectedDto, x => x.Excluding(x => x.Id).Excluding(x => x.CreatedAt));

            notificationRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
            notificationRepositoryMock.Verify(n => n.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
        }
    }
}
