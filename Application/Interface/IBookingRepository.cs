using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;
using Domain.Model;

namespace Application.Interface
{
   public interface IBookingRepository
    {
        Task<Booking> GetBookingByIdAsync(int id);
        Task<List<Booking>> GetBookingByUserIdAsync(int userId);
        Task AddBookingAsync(Booking booking);
        //Task<Booking?> GetBookingByUserAndPlanPeriod(int userId, DateTime planStart, DateTime planEnd);
        Task<Booking?> GetLastBookingWithPlantsAsync(int userId, DateTime planStart, DateTime planEnd);

        Task SaveChangesAsync();
        Task AssignProviderAsync(int bookingId, int ProviderId);
        Task<List<Booking>> GetPendingBookingsAsync();
        Task UpdateBookingStatusAsync(int bookingId, BookingStatus status);
        Task<List<Booking>> GetAllBookingsAsync();
        Task<List<Booking>> GetBookingsByProviderIdAsync(int providerId);
        Task<bool> HasActiveBookingInPlanAsync(int userId, DateTime planStart, DateTime planEnd);
        Task RemoveBookingAsync(int bookingId);

    }
}
