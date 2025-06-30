using Application.DTOs;
using Application.Interface;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController:ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpPost("Create")]
        public async Task<IActionResult>CreaetBooking(int userId,int addressId,string transactionId)
        {
            var result = await _bookingService.CreateBookingAsync(userId, addressId, transactionId);
            return Ok(result);

        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult>GetUserBookings(int userId)
        {
            var result = await _bookingService.GetUserBookingAsync(userId);
            return Ok(result);
        }

        [HttpGet("id")]
        public async Task<IActionResult>GetBooking(int id)
        {
            var result = await _bookingService.GetBookingByIdAsync(id);
            return Ok(result);
        }
        [HttpPost("Assign-provider")]
        public async Task<IActionResult> AssignProvider([FromQuery]int bookingId, [FromQuery]int providerId)
        {
            var result = await _bookingService.AssignProviderToBookingAsync(bookingId, providerId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingBookings()
        {
            var result = await _bookingService.GetPendingBookingsAsync();
            return Ok(result);
        }
        [HttpPut("update-status")]
        public async Task<IActionResult>UpdateBookingStatus(int bookingId,BookingStatus status)
        {
            await _bookingService.UpdateBookingStatusAsync(bookingId, status);
            return Ok(new { success = true, message = "Booking status updated" });
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBookings()
        {
            var result = await _bookingService.GetAllBookingsAsync();
            return Ok(result);
        }
        [HttpPost("create-service")]
        public async Task<IActionResult> CreateServiceBooking([FromBody] ServiceBookingRequestDto dto)
        {
            var result = await _bookingService.CreateServiceBookingAsync(dto.UserId, dto.AddressId, dto.TransactionId, dto.ServiceIds);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        //[Authorize(Roles = "User")]
        //[HttpPost("razorpay/create-order")]
        //public async Task<IActionResult> CreateRazorpayOrder([FromQuery] long amount)
        //{
        //    if (amount <= 0)
        //        return BadRequest(new ApiResponse<string>(false, "Invalid amount", null));

        //    var orderId = await _bookingService.CreateRazorpayOrderAsync(amount);
        //    return Ok(new ApiResponse<string>(true, "Order created", orderId));
        //}

        //[Authorize(Roles = "User")]
        //[HttpPost("razorpay/verify-payment")]
        //public async Task<IActionResult> VerifyRazorpayPayment([FromBody] PaymentDto payment)
        //{
        //    var verified = await _bookingService.VerifyRazorpayPaymentAsync(payment);
        //    if (!verified)
        //        return BadRequest(new ApiResponse<string>(false, "Payment verification failed", null));

        //    return Ok(new ApiResponse<string>(true, "Payment verified", "Success"));
        //}
        [Authorize(Roles = "User")]
        [HttpPost("razorpay-order-create")]
        public async Task<IActionResult> CreateRazorpayOrder([FromQuery] long amount)
        {
            try
            {
                var (orderId,razorpayKey) = await _bookingService.CreateRazorpayOrderAsync(amount);
                var data = new { orderId, razorpayKey };
                return Ok(new ApiResponse<object>(true, "Order created", data));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(false, "Failed to create order", null, ex.Message));
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost("razorpay-payment-verify")]
        public async Task<IActionResult> VerifyRazorpayPayment([FromBody] PaymentDto payment)
        {
            try
            {
                var isValid = await _bookingService.VerifyRazorpayPaymentAsync(payment);
                if (!isValid)
                    return BadRequest(new ApiResponse<string>(false, "Payment verification failed",null));

                return Ok(new ApiResponse<string>(true, "Payment verified", "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(false, "Verification error", null, ex.Message));
            }
        }
        [HttpGet("assigned/{providerId}")]
        public async Task<IActionResult> GetAssignedBookings(int providerId)
        {
            var result = await _bookingService.GetBookingsByProviderAsync(providerId);
            return Ok(result);
        }

        [HttpPost("swap")]
        public async Task<IActionResult> SwapPlants([FromQuery] int userId, [FromQuery] int addressId, [FromQuery] string transactionId)
        {
            var result = await _bookingService.SwapPlantsAsync(userId, addressId, transactionId);
            return result.Success ? Ok(result) : BadRequest(result);
        }


    }
}
