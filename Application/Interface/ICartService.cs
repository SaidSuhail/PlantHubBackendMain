using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interface
{
   public interface ICartService
    {
        Task<ApiResponse<CartWithTotalPriceDto>> GetCartItemsAsync(int userId);
        Task<ApiResponse<CartItemDto>> AddToCartAsync(int userId, int plantId);
        Task<ApiResponse<string>> RemoveFromCartAsync(int userId, int plantId);
        Task<ApiResponse<string>> IncreaseQuantityAsync(int userId, int plantId);
        Task<ApiResponse<string>> DecreaseQuantityAsync(int userId, int plantId);
        Task<ApiResponse<string>> ClearCartAsync(int userId);
        Task<ApiResponse<string>> ReplaceCartWithBookingItemsAsync(int userId);

    }
}
