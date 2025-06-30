using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Enum;
using Razorpay.Api;

namespace Application.Interface
{
   public interface IBookingService
    {
        Task<ApiResponse<BookingDto>> CreateBookingAsync(int userId, int addressId, string transactionId);
        Task<ApiResponse<List<BookingDto>>> GetUserBookingAsync(int userId);
        Task<ApiResponse<BookingDto>> GetBookingByIdAsync(int id);
        Task<ApiResponse<string>> AssignProviderToBookingAsync(int bookingId, int providerId);
        Task<ApiResponse<List<BookingDto>>> GetPendingBookingsAsync();
        Task UpdateBookingStatusAsync(int bookingId, BookingStatus status);
        Task<ApiResponse<List<BookingDto>>> GetAllBookingsAsync();
        Task<(string orderId, string razorpayKey)> CreateRazorpayOrderAsync(long amount);
        Task<bool> VerifyRazorpayPaymentAsync(PaymentDto payment);
        Task<ApiResponse<List<BookingDto>>> GetBookingsByProviderAsync(int providerId);
        //Task<ApiResponse<BookingDto>> SwapPlantsAsync(int userId, int addressId, string transactionId);
        Task<ApiResponse<SwapBookingResponseDto>> SwapPlantsAsync(int userId, int addressId, string transactionId);


        Task<ApiResponse<BookingDto>> CreateServiceBookingAsync(int userId, int addressId, string transactionId, List<int> serviceIds);

    }
}
