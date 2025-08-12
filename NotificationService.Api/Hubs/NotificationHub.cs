using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Api.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
