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
   public class CartRepository:ICartRepository
    {
        private readonly AppdbContext _context;
        public CartRepository(AppdbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetUserCartAsync(int userId)
        {
            return await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Plant)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async  Task<Cart>CreateCartAsync(int userId)
        {
            var cart = new Cart
            {
                UserId = userId
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }
        public async Task<CartItem?>GetCartItemAsync(int userId,int plantId)
        {
            return await _context.CartItems.Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Cart.UserId == userId && ci.PlantId == plantId);
        }
        public async Task AddCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
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

        public async Task ClearCartAsync(int userId)
        {
            var cart = await GetUserCartAsync(userId);
            if (cart != null &&cart.CartItems!= null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
