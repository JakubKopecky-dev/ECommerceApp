using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using NotificationService.Api.Controllers;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Enum;

namespace NotificationService.UnitTests.Controllers
{
    public class NotificationControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllUserNotifications_ReturnsOk_WhenUserAuthorized()
        {
            Guid userId = Guid.NewGuid();

            IReadOnlyList<NotificationDto> expectedDto =
            [
                new() { Id = Guid.NewGuid(), UserId = userId, Title = "New order", Message = "Order is created", Type = NotificationType.OrderCreated },
                new() { Id = Guid.NewGuid(), UserId = userId, Title = "Delivery status changed", Message = "New delivery status", Type = NotificationType.DeliveryStatusChanged }
            ];

            Mock<INotificationService> notificationServiceMock = new();

            notificationServiceMock
                .Setup(n => n.GetAllNotificationByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            NotificationController controller = new(notificationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(
                            [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
                            "mock"))
                    }
                }
            };


            var result = await controller.GetAllUserNotifications(It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            notificationServiceMock.Verify(n => n.GetAllNotificationByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllUserNotifications_ReturnsUnauthorized_WhenUserIdMissing()
        {
            Mock<INotificationService> notificationServiceMock = new();

            NotificationController controller = new(notificationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity())
                    }
                }
            };


            var result = await controller.GetAllUserNotifications(It.IsAny<CancellationToken>());

            result.Should().BeOfType<UnauthorizedResult>();

            notificationServiceMock.Verify(n => n.GetAllNotificationByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetNotification_ReturnsOk_WhenExists()
        {
            Guid notificationId = Guid.NewGuid();

            NotificationDto expectedDto = new() { Id = notificationId, UserId = Guid.NewGuid(), Title = "New order", Message = "Order is created", Type = NotificationType.OrderCreated };

            Mock<INotificationService> notificationServiceMock = new();

            notificationServiceMock
                .Setup(n => n.GetNotificationAsync(notificationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            NotificationController controller = new(notificationServiceMock.Object);


            var result = await controller.GetNotification(notificationId, It.IsAny<CancellationToken>());

            (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedDto);

            notificationServiceMock.Verify(n => n.GetNotificationAsync(notificationId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetNotification_ReturnsNotFound_WhenNotExists()
        {
            Guid notificationId = Guid.NewGuid();

            Mock<INotificationService> notificationServiceMock = new();

            notificationServiceMock
                .Setup(n => n.GetNotificationAsync(notificationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((NotificationDto?)null);

            NotificationController controller = new(notificationServiceMock.Object);


            var result = await controller.GetNotification(notificationId, It.IsAny<CancellationToken>());

            result.Should().BeOfType<NotFoundResult>();

            notificationServiceMock.Verify(n => n.GetNotificationAsync(notificationId, It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateNotification_ReturnsCreatedAtAction_WithNotificationDto()
        {
            CreateNofiticationDto createDto = new()
            {
                UserId = Guid.NewGuid(),
                Title = "New order",
                Message = "Order is created",
                Type = NotificationType.OrderCreated,
                CreatedAt = DateTime.UtcNow
            };

            NotificationDto expectedDto = new()
            {
                Id = Guid.NewGuid(),
                UserId = createDto.UserId,
                Title = createDto.Title,
                Message = createDto.Message,
                Type = createDto.Type
            };

            Mock<INotificationService> notificationServiceMock = new();

            notificationServiceMock
                .Setup(n => n.CreateNotificationAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            NotificationController controller = new(notificationServiceMock.Object);


            var result = await controller.CreateNotification(createDto, It.IsAny<CancellationToken>());
            var createdResult = result as CreatedAtActionResult;

            createdResult!.ActionName.Should().Be(nameof(NotificationController.GetNotification));
            createdResult.RouteValues!["notificationId"].Should().Be(expectedDto.Id);
            createdResult.Value.Should().Be(expectedDto);

            notificationServiceMock.Verify(n => n.CreateNotificationAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
