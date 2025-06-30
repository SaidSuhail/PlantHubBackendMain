using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
   public class BookingRepository:IBookingRepository
    {
        private readonly AppdbContext _context;

        public BookingRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task<Booking?>GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.BookingItems)
                .ThenInclude(i => i.Plant)
                 .Include(b => b.ServiceBookingItems) // ✅ Add this
            .ThenInclude(s => s.Service)     // ✅
                .Include(b => b.AssignedProvider)
            .ThenInclude(p => p.User) // To fetch Provider's name
        .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        //public async Task<Booking?> GetBookingByUserAndPlanPeriod(int userId, DateTime planStart, DateTime planEnd)
        //{
        //    return await _context.Bookings
        //        .Where(b => b.UserId == userId &&
        //                    b.BookingDate >= planStart &&
        //                    b.BookingDate <= planEnd)
        //        .FirstOrDefaultAsync();
        //}
        public async Task<Booking?> GetLastBookingWithPlantsAsync(int userId, DateTime planStart, DateTime planEnd)
        {
            return await _context.Bookings
                .Include(b => b.BookingItems)
                    .ThenInclude(i => i.Plant)
                .Where(b => b.UserId == userId &&
                            b.BookingType == BookingType.PlantBooking &&
                            b.BookingDate >= planStart &&
                            b.BookingDate <= planEnd)
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Booking>> GetBookingByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.BookingItems)
                .ThenInclude(i => i.Plant)
                .Include(b => b.AssignedProvider)
            .ThenInclude(p => p.User)
                .Where(b => b.UserId == userId)
                .Include(b => b.ServiceBookingItems)   // ✅ Add this
            .ThenInclude(s => s.Service)       // ✅ And this
                .ToListAsync();
        }
        public async Task AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AssignProviderAsync(int bookingId,int providerId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if(booking != null)
            {
                booking.ProviderId = providerId;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Booking>> GetPendingBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.AssignedProvider)
                .Include(b=>b.BookingItems)
                .Where(b => b.BookingStatus == BookingStatus.Pending)
                 .Include(b => b.ServiceBookingItems)   // ✅ Add this
            .ThenInclude(s => s.Service)
                .ToListAsync();
        }
        public async Task UpdateBookingStatusAsync(int bookingId,BookingStatus status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if(booking != null)
            {
                booking.BookingStatus = status;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.AssignedProvider)
                    .ThenInclude(p => p.User) // For ProviderName
                .Include(b => b.BookingItems)
                    .ThenInclude(i => i.Plant)
                    .Include(b => b.ServiceBookingItems)   // ✅ Add this
            .ThenInclude(s => s.Service)       // ✅ And this
                .ToListAsync();
        }
        public async Task<List<Booking>> GetBookingsByProviderIdAsync(int providerId)
        {
            return await _context.Bookings
                .Where(b => b.ProviderId == providerId)
                .Include(b => b.User)
                .Include(b => b.BookingItems).ThenInclude(i => i.Plant)
                .Include(b => b.ServiceBookingItems).ThenInclude(s => s.Service)
                .ToListAsync();
        }
        public async Task<bool> HasActiveBookingInPlanAsync(int userId, DateTime planStart, DateTime planEnd)
        {
            return await _context.Bookings.AnyAsync(b =>
                b.UserId == userId &&
                b.BookingType == BookingType.PlantBooking &&
                b.BookingDate >= planStart &&
                b.BookingDate <= planEnd);
        }

        public async Task RemoveBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

    }
}
