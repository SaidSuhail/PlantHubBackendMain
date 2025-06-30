using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface ICartRepository
    {
        Task<Cart> GetUserCartAsync(int userId);               // Get current user's cart
        Task<Cart> CreateCartAsync(int userId);                // Create a new cart
        Task<CartItem> GetCartItemAsync(int userId, int plantId); // Get specific item in user's cart
        Task AddCartItemAsync(CartItem cartItem);              // Add new item to cart
        Task UpdateCartItemAsync(CartItem cartItem);           // Update existing cart item
        Task RemoveCartItemAsync(CartItem cartItem);           // Remove a single cart item
        Task ClearCartAsync(int userId);                       // Remove all items in user's cart
        Task SaveChangesAsync();
        Task<Booking?> GetLastBookingWithPlantsAsync(int userId, DateTime planStart, DateTime planEnd);

    }
}
