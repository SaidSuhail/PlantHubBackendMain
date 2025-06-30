using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;

namespace Application.Services
{
   public class BookingService:IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IUserPlanRepository _userPlanRepo;
        private readonly IMapper _mapper;
        private readonly IServiceRepository _serviceRepo;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        public BookingService(IBookingRepository bookingRepo,IServiceRepository serviceRepo, ICartRepository cartRepo, IUserPlanRepository userPlanRepo, IMapper mapper, INotificationService notificationService,IConfiguration configuration)
        {
            _bookingRepo = bookingRepo;
            _cartRepo = cartRepo;
            _userPlanRepo = userPlanRepo;
            _mapper = mapper;
            _serviceRepo = serviceRepo;
            _notificationService = notificationService;
            _configuration = configuration;
        }

        public async Task<ApiResponse<BookingDto>>CreateBookingAsync(int userId,int addressId,string transactionId)
        {
            var userPlan = (await _userPlanRepo.GetUserPlansByUserId(userId))
                .FirstOrDefault(p => p.IsActive && p.EndDate >= DateTime.UtcNow);
            if(userPlan == null || userPlan.EndDate<DateTime.UtcNow)
                return new ApiResponse<BookingDto>(false, "No valid plan Found",null);

            var existingBooking = await _bookingRepo.GetLastBookingWithPlantsAsync(userId, userPlan.StartDate, userPlan.EndDate);
            if (existingBooking != null)
                return new ApiResponse<BookingDto>(false, "You Already Habe  A booking during this plan perioud", null);

            var cart = await _cartRepo.GetUserCartAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                return new ApiResponse<BookingDto>(false, "Cart is empty", null);

            var booking = new Booking
            {
                UserId = userId,
                PlanId = userPlan.PlanId,
                UserAddressId = addressId,
                BookingDate = DateTime.UtcNow,
                BookingType = Domain.Enum.BookingType.PlantBooking,
                BookingStatus = Domain.Enum.BookingStatus.Pending,
                TransactionId = transactionId,
                TotalPrice = cart.CartItems.Sum(i => i.Plant.Price * i.Quantity),
                BookingItems = cart.CartItems.Select(i => new BookingItem
                {
                    PlantId = i.PlantId,
                    Quantity = i.Quantity,
                    TotalPrice = i.Plant.Price * i.Quantity,
                    PlantImage = i.Plant.ImageUrl
                }).ToList()
            };
            await _bookingRepo.AddBookingAsync(booking);
            await _cartRepo.ClearCartAsync(userId);
            await _notificationService.SendNotificationAsync(booking.UserId, "Your Booking Was Successfully Created");

            var result = _mapper.Map<BookingDto>(booking);
            return new ApiResponse<BookingDto>(true, "Booking Created", result);    
        }
     
        public async Task<ApiResponse<BookingDto>> CreateServiceBookingAsync(
    int userId, int addressId, string transactionId, List<int> serviceIds)
        {
            // ✅ 1. Check for active plan (required, but no limits enforced)
            var userPlan = (await _userPlanRepo.GetUserPlansByUserId(userId))
                .FirstOrDefault(p => p.IsActive && p.EndDate >= DateTime.UtcNow);

            if (userPlan == null)
                return new ApiResponse<BookingDto>(false, "No active plan found", null);

            // ✅ 2. Fetch the selected services
            var services = await _serviceRepo.GetServicesByIdsAsync(serviceIds);
            if (services == null || !services.Any())
                return new ApiResponse<BookingDto>(false, "No valid services found", null);

            // ✅ 3. Create the booking (with PlanId but no count limit check)
            var booking = new Booking
            {
                UserId = userId,
                PlanId = userPlan.PlanId, // ✅ Required
                UserAddressId = addressId,
                BookingDate = DateTime.UtcNow,
                BookingType = BookingType.ServiceBooking,
                BookingStatus = BookingStatus.Pending,
                TransactionId = transactionId,
                TotalPrice = services.Sum(s => s.Price),
                ServiceBookingItems = services.Select(s => new ServiceBookingItem
                {
                    ServiceId = s.Id,
                    TotalPrice = s.Price
                }).ToList()
            };

            await _bookingRepo.AddBookingAsync(booking);
            await _notificationService.SendNotificationAsync(booking.UserId, "Your Service Booking Was SuccessFully Created");
            var result = _mapper.Map<BookingDto>(booking);
            return new ApiResponse<BookingDto>(true, "Service Booking Created", result);
        }

        public async Task<ApiResponse<List<BookingDto>>> GetUserBookingAsync(int userId)
        {
            var bookings = await _bookingRepo.GetBookingByUserIdAsync(userId);
            var result = _mapper.Map<List<BookingDto>>(bookings);
            return new ApiResponse<List<BookingDto>>(true,"Booking Fetched",result);

        }
        public async Task<ApiResponse<BookingDto>>GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepo.GetBookingByIdAsync(id);
            if (booking == null)
                return new ApiResponse<BookingDto>(false, "Booking not found", null);
            var dto = _mapper.Map<BookingDto>(booking);
            return new ApiResponse<BookingDto>(true, "Booking Fettched", dto);
        }
        public async Task<ApiResponse<string>> AssignProviderToBookingAsync(int bookingId,int providerId)
        {
            await _bookingRepo.AssignProviderAsync(bookingId, providerId);
            return new ApiResponse<string>(true, "Provider Assigned To Booking Successfully",null);
        }
        public async Task<ApiResponse<List<BookingDto>>> GetPendingBookingsAsync()
        {
            var bookings = await _bookingRepo.GetPendingBookingsAsync();
            var result = _mapper.Map<List<BookingDto>>(bookings);
            return new ApiResponse<List<BookingDto>>(true, "pending bookings fetched", result);
        }
        public async Task UpdateBookingStatusAsync(int bookingId, BookingStatus status)
        {
            await _bookingRepo.UpdateBookingStatusAsync(bookingId, status);
        }
        public async Task<ApiResponse<List<BookingDto>>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepo.GetAllBookingsAsync();
            var result = _mapper.Map<List<BookingDto>>(bookings);
            return new ApiResponse<List<BookingDto>>(true, "All bookings fetched", result);
        }
       
        public async Task<(string orderId,string razorpayKey)> CreateRazorpayOrderAsync(long amount)
        {
            try
            {
                var transactionId = Guid.NewGuid().ToString();
                var input = new Dictionary<string, object>
        {
            { "amount", amount }, // paise
            { "currency", "INR" },
            { "receipt", transactionId }
        };

                var key = _configuration["Razorpay:KeyId"];
                var secret = _configuration["Razorpay:KeySecret"];
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(secret))
                    throw new Exception("Missing Razorpay keys.");

                var client = new RazorpayClient(key, secret);
                var order = client.Order.Create(input);

                var orderId = order["id"].ToString();

                return (orderId, key);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create Razorpay order: {ex.Message}");
            }
        }


        public async Task<bool> VerifyRazorpayPaymentAsync(PaymentDto payment)
        {
            try
            {
                if (payment == null ||
                    string.IsNullOrEmpty(payment.razorpay_payment_id) ||
                    string.IsNullOrEmpty(payment.razorpay_order_id) ||
                    string.IsNullOrEmpty(payment.razorpay_signature))
                    return false;

                var attributes = new Dictionary<string, string>
        {
            { "razorpay_payment_id", payment.razorpay_payment_id },
            { "razorpay_order_id", payment.razorpay_order_id },
            { "razorpay_signature", payment.razorpay_signature }
        };

                Utils.verifyPaymentSignature(attributes);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Payment verification failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<BookingDto>>> GetBookingsByProviderAsync(int providerId)
        {
            var bookings = await _bookingRepo.GetBookingsByProviderIdAsync(providerId);
            var result = _mapper.Map<List<BookingDto>>(bookings);
            return new ApiResponse<List<BookingDto>>(true, "Assigned bookings fetched", result);
        }
        //public async Task<ApiResponse<BookingDto>> SwapPlantsAsync(int userId, int addressId, string transactionId)
        //{
        //    var userPlan = (await _userPlanRepo.GetUserPlansByUserId(userId))
        //        .FirstOrDefault(p => p.IsActive && p.EndDate >= DateTime.UtcNow);

        //    if (userPlan == null)
        //        return new ApiResponse<BookingDto>(false, "No active plan found", null);

        //    var oldBooking = await _bookingRepo.GetBookingByUserAndPlanPeriod(userId, userPlan.StartDate, userPlan.EndDate);
        //    if (oldBooking != null)
        //        await _bookingRepo.RemoveBookingAsync(oldBooking.Id);

        //    var cart = await _cartRepo.GetUserCartAsync(userId);
        //    if (cart == null || !cart.CartItems.Any())
        //        return new ApiResponse<BookingDto>(false, "Cart is empty", null);

        //    var totalPrice = cart.CartItems.Sum(i => i.Plant.Price * i.Quantity);
        //    var booking = new Booking
        //    {
        //        UserId = userId,
        //        PlanId = userPlan.PlanId,
        //        UserAddressId = addressId,
        //        BookingDate = DateTime.UtcNow,
        //        BookingType = BookingType.PlantBooking,
        //        BookingStatus = BookingStatus.Pending,
        //        TransactionId = transactionId,
        //        TotalPrice = totalPrice,
        //        BookingItems = cart.CartItems.Select(i => new BookingItem
        //        {
        //            PlantId = i.PlantId,
        //            Quantity = i.Quantity,
        //            TotalPrice = i.Plant.Price * i.Quantity,
        //            PlantImage = i.Plant.ImageUrl
        //        }).ToList()
        //    };

        //    await _bookingRepo.AddBookingAsync(booking);
        //    await _cartRepo.ClearCartAsync(userId);
        //    await _notificationService.SendNotificationAsync(userId, "Your plants were successfully swapped.");

        //    var result = _mapper.Map<BookingDto>(booking);
        //    return new ApiResponse<BookingDto>(true, "Swap successful", result);
        //}

        //public async Task<ApiResponse<SwapBookingResponseDto>> SwapPlantsAsync(int userId, int addressId, string transactionId)
        //{
        //    var userPlan = (await _userPlanRepo.GetUserPlansByUserId(userId))
        //        .FirstOrDefault(p => p.IsActive && p.EndDate >= DateTime.UtcNow);

        //    if (userPlan == null)
        //        return new ApiResponse<SwapBookingResponseDto>(false, "No active plan found", null);

        //    // 1. Get old booking
        //    var oldBooking = await _bookingRepo.GetBookingByUserAndPlanPeriod(userId, userPlan.StartDate, userPlan.EndDate);
        //    if (oldBooking == null)
        //        return new ApiResponse<SwapBookingResponseDto>(false, "No existing booking found to swap", null);

        //    var oldPrice = oldBooking.TotalPrice;

        //    // 2. Get cart
        //    var cart = await _cartRepo.GetUserCartAsync(userId);
        //    if (cart == null || !cart.CartItems.Any())
        //        return new ApiResponse<SwapBookingResponseDto>(false, "Cart is empty", null);

        //    var newPrice = cart.CartItems.Sum(i => i.Plant.Price * i.Quantity);

        //    // 3. Compare prices
        //    if (newPrice > oldPrice && string.IsNullOrEmpty(transactionId))
        //    {
        //        var extraAmount = newPrice - oldPrice;

        //        // 🔁 Let frontend trigger Razorpay only if extra payment needed
        //        return new ApiResponse<SwapBookingResponseDto>(false, $"Extra payment of ₹{extraAmount} required", null, "EXTRA_PAYMENT_REQUIRED");
        //    }

        //    // 4. Proceed to swap: remove old, create new booking
        //    await _bookingRepo.RemoveBookingAsync(oldBooking.Id);

        //    var booking = new Booking
        //    {
        //        UserId = userId,
        //        PlanId = userPlan.PlanId,
        //        UserAddressId = addressId,
        //        BookingDate = DateTime.UtcNow,
        //        BookingType = BookingType.PlantBooking,
        //        BookingStatus = BookingStatus.Pending,
        //        TransactionId = transactionId,
        //        TotalPrice = newPrice,
        //        BookingItems = cart.CartItems.Select(i => new BookingItem
        //        {
        //            PlantId = i.PlantId,
        //            Quantity = i.Quantity,
        //            TotalPrice = i.Plant.Price * i.Quantity,
        //            PlantImage = i.Plant.ImageUrl
        //        }).ToList()
        //    };

        //    await _bookingRepo.AddBookingAsync(booking);
        //    await _cartRepo.ClearCartAsync(userId);
        //    await _notificationService.SendNotificationAsync(userId, "Your plants were successfully swapped.");

        //    var result = _mapper.Map<SwapBookingResponseDto>(booking);
        //    return new ApiResponse<SwapBookingResponseDto>(true, "Swap successful", result);
        //}
        //public async Task<ApiResponse<SwapBookingResponseDto>> SwapPlantsAsync(int userId, int addressId, string transactionId)
        //{
        //    var userPlan = (await _userPlanRepo.GetUserPlansByUserId(userId))
        //        .FirstOrDefault(p => p.IsActive && p.EndDate >= DateTime.UtcNow);

        //    if (userPlan == null)
        //        return new ApiResponse<SwapBookingResponseDto>(false, "No active plan found", null);

        //    // 1. Get old booking
        //    var oldBooking = await _bookingRepo.GetBookingByUserAndPlanPeriod(userId, userPlan.StartDate, userPlan.EndDate);
        //    if (oldBooking == null)
        //        return new ApiResponse<SwapBookingResponseDto>(false, "No existing booking found to swap", null);

        //    var oldPrice = oldBooking.TotalPrice;

        //    // 2. Get cart
        //    var cart = await _cartRepo.GetUserCartAsync(userId);
        //    if (cart == null || !cart.CartItems.Any())
        //        return new ApiResponse<SwapBookingResponseDto>(false, "Cart is empty", null);

        //    var newPrice = cart.CartItems.Sum(i => i.Plant.Price * i.Quantity);
        //    var extraAmount = newPrice > oldPrice ? newPrice - oldPrice : 0;

        //    // 3. If extra payment is required and transactionId is not yet provided
        //    if (extraAmount > 0 && string.IsNullOrEmpty(transactionId))
        //    {
        //        return new ApiResponse<SwapBookingResponseDto>(
        //            false,
        //            $"Extra payment of ₹{extraAmount} required",
        //            new SwapBookingResponseDto
        //            {
        //                Booking = null,
        //                ExtraAmount = extraAmount
        //            },
        //            "EXTRA_PAYMENT_REQUIRED"
        //        );
        //    }

        //    // 4. Proceed to swap
        //    await _bookingRepo.RemoveBookingAsync(oldBooking.Id);

        //    var booking = new Booking
        //    {
        //        UserId = userId,
        //        PlanId = userPlan.PlanId,
        //        UserAddressId = addressId,
        //        BookingDate = DateTime.UtcNow,
        //        BookingType = BookingType.PlantBooking,
        //        BookingStatus = BookingStatus.Pending,
        //        TransactionId = transactionId,
        //        TotalPrice = newPrice,
        //        BookingItems = cart.CartItems.Select(i => new BookingItem
        //        {
        //            PlantId = i.PlantId,
        //            Quantity = i.Quantity,
        //            TotalPrice = i.Plant.Price * i.Quantity,
        //            PlantImage = i.Plant.ImageUrl
        //        }).ToList()
        //    };

        //    await _bookingRepo.AddBookingAsync(booking);
        //    await _cartRepo.ClearCartAsync(userId);
        //    await _notificationService.SendNotificationAsync(userId, "Your plants were successfully swapped.");

        //    var bookingDto = _mapper.Map<BookingDto>(booking);

        //    var responseDto = new SwapBookingResponseDto
        //    {
        //        Booking = bookingDto,
        //        ExtraAmount = extraAmount
        //    };

        //    return new ApiResponse<SwapBookingResponseDto>(true, "Swap successful", responseDto);
        //}
        public async Task<ApiResponse<SwapBookingResponseDto>> SwapPlantsAsync(int userId, int addressId, string transactionId)
        {
            var userPlan = (await _userPlanRepo.GetUserPlansByUserId(userId))
                .FirstOrDefault(p => p.IsActive && p.EndDate >= DateTime.UtcNow);

            if (userPlan == null)
                return new ApiResponse<SwapBookingResponseDto>(false, "No active plan found", null);

            // ✅ 1. Get most recent booking in plan period
            var oldBooking = await _bookingRepo.GetLastBookingWithPlantsAsync(userId, userPlan.StartDate, userPlan.EndDate);
            if (oldBooking == null)
                return new ApiResponse<SwapBookingResponseDto>(false, "No existing booking found to swap", null);

            var oldPrice = oldBooking.TotalPrice;

            // ✅ 2. Get cart
            var cart = await _cartRepo.GetUserCartAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                return new ApiResponse<SwapBookingResponseDto>(false, "Cart is empty", null);

            var newPrice = cart.CartItems.Sum(i => i.Plant.Price * i.Quantity);
            var extraAmount = newPrice > oldPrice ? newPrice - oldPrice : 0;

            // ✅ 3. If extra payment is needed and not paid yet
            if (extraAmount > 0 && string.IsNullOrEmpty(transactionId))
            {
                return new ApiResponse<SwapBookingResponseDto>(
                    false,
                    $"Extra payment of ₹{extraAmount} required",
                    new SwapBookingResponseDto
                    {
                        Booking = null,
                        ExtraAmount = extraAmount
                    },
                    "EXTRA_PAYMENT_REQUIRED"
                );
            }

            // ✅ 4. Proceed to swap
            await _bookingRepo.RemoveBookingAsync(oldBooking.Id);

            var booking = new Booking
            {
                UserId = userId,
                PlanId = userPlan.PlanId,
                UserAddressId = addressId,
                BookingDate = DateTime.UtcNow,
                BookingType = BookingType.PlantBooking,
                BookingStatus = BookingStatus.Pending,
                TransactionId = transactionId,
                TotalPrice = newPrice,
                BookingItems = cart.CartItems.Select(i => new BookingItem
                {
                    PlantId = i.PlantId,
                    Quantity = i.Quantity,
                    TotalPrice = i.Plant.Price * i.Quantity,
                    PlantImage = i.Plant.ImageUrl
                }).ToList()
            };

            await _bookingRepo.AddBookingAsync(booking);
            await _cartRepo.ClearCartAsync(userId);
            await _notificationService.SendNotificationAsync(userId, "Your plants were successfully swapped.");

            var bookingDto = _mapper.Map<BookingDto>(booking);

            var responseDto = new SwapBookingResponseDto
            {
                Booking = bookingDto,
                ExtraAmount = extraAmount
            };

            return new ApiResponse<SwapBookingResponseDto>(true, "Swap successful", responseDto);
        }



    }
}
