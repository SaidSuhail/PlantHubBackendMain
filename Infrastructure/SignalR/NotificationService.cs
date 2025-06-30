using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Model;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
   public class NotificationService:INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationRepository _notificationRepo;
        public NotificationService(IHubContext<NotificationHub> hubContext,INotificationRepository notificationRepo)
        {
            _hubContext = hubContext;
            _notificationRepo = notificationRepo;
        }
        public async Task SendNotificationAsync(int userId,string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationRepo.AddNotificationAsync(notification);
            await _hubContext.Clients.User(userId.ToString()).SendAsync("RecieveNotification", message);
        }
    }
}
