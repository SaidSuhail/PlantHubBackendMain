using System.Security.Claims;
using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController:ControllerBase
    {
        private readonly INotificationRepository _notificationRepo;
        public NotificationController(INotificationRepository notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }
        [HttpGet("user")]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var notifications = await _notificationRepo.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationRepo.MarkAsReadAsync(id);
            return Ok(new { message = "Notification marked as read." });
        }
    }
}
