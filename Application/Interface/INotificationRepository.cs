using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface INotificationRepository
    {
        Task<List<Notification>> GetUserNotificationsAsync(int userId);
        Task AddNotificationAsync(Notification notification);
        Task MarkAsReadAsync(int notificationId);
    }
}
