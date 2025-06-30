using Application.DTOs;
using Application.Interface;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlantHubBackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Ensure the user is authenticated
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var userId = GetUserId();
            var response = await _cartService.GetCartItemsAsync(userId);
            return response.Success
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("add/{plantId}")]
        public async Task<IActionResult> AddToCart(int plantId)
        {
            var userId = GetUserId();
            var response = await _cartService.AddToCartAsync(userId, plantId);
            return response.Success
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("remove/{plantId}")]
        public async Task<IActionResult> RemoveFromCart(int plantId)
        {
            var userId = GetUserId();
            var response = await _cartService.RemoveFromCartAsync(userId, plantId);
            return response.Success
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPut("increase/{plantId}")]
        public async Task<IActionResult> IncreaseQuantity(int plantId)
        {
            var userId = GetUserId();
            var response = await _cartService.IncreaseQuantityAsync(userId, plantId);
            return response.Success
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPut("decrease/{plantId}")]
        public async Task<IActionResult> DecreaseQuantity(int plantId)
        {
            var userId = GetUserId();
            var response = await _cartService.DecreaseQuantityAsync(userId, plantId);
            return response.Success
                ? Ok(response)
                : BadRequest(response);
        }
        [HttpGet("initiate-swap/{userId}")]
        public async Task<IActionResult> InitiateSwap(int userId)
        {
            var result = await _cartService.ReplaceCartWithBookingItemsAsync(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var response = await _cartService.ClearCartAsync(userId);
            return response.Success
                ? Ok(response)
                : BadRequest(response);
        }
    }
}
