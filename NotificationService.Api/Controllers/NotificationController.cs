using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Enum;

namespace NotificationService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoles.User)]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;



        [HttpGet]
        public async Task<IActionResult> GetAllUserNotifications(CancellationToken ct)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            IReadOnlyList<NotificationDto> notifications = await _notificationService.GetAllNotificationByUserIdAsync(userId, ct);

            return Ok(notifications);
        }



        [HttpGet("detail/{notificationId}")]
        public async Task<IActionResult> GetNotification(Guid notificationId, CancellationToken ct)
        {
            NotificationDto? notification = await _notificationService.GetNotificationAsync(notificationId, ct);

            return notification is not null ? Ok(notification) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNofiticationDto createNofiticationDto,CancellationToken ct)
        {
            NotificationDto notification = await _notificationService.CreateNotificationAsync(createNofiticationDto, ct);

            return CreatedAtAction(nameof(GetNotification),new {notificationId = notification.Id }, notification);
        }



    }
}
