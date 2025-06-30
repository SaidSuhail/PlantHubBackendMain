using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Domain.Model;

namespace Application.Services
{
   public class CartService:ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly IPlantRepository _plantRepository;
        private readonly IUserPlanRepository _userPlanRepository;
        public CartService(ICartRepository cartRepo,IMapper mapper, IPlantRepository plantRepo, IUserPlanRepository userPlanRepo)
        {
            _cartRepository = cartRepo;
            _plantRepository = plantRepo;
            _userPlanRepository = userPlanRepo;
            _mapper = mapper;
        }


        public async Task<ApiResponse<CartWithTotalPriceDto>> GetCartItemsAsync(int userId)
        {
            try
            {
                var cart = await _cartRepository.GetUserCartAsync(userId);
                if (cart == null)

                return new ApiResponse<CartWithTotalPriceDto>(false, "Cart not found", null);

                var cartDto = _mapper.Map<CartWithTotalPriceDto>(cart);
                cartDto.TotalPrice = cartDto.Items?.Sum(i => i.TotalPrice ?? 0) ?? 0;

                return new ApiResponse<CartWithTotalPriceDto>(true, "Cart fetched successfully", cartDto);
                
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartWithTotalPriceDto>(false, "Error fetching cart", null, ex.Message);
            }
        }

        public async Task<ApiResponse<CartItemDto>> AddToCartAsync(int userId, int plantId)
        {
            try
            {
                var userPlans = await _userPlanRepository.GetUserPlansByUserId(userId);
                var userPlan = userPlans.FirstOrDefault(up => up.IsActive);
                if (userPlan == null)

                    return new ApiResponse<CartItemDto>(false, "User has no active plan", null);

                var maxLimit = userPlan.Plan.MaxPlantsAllowed ?? 0;

                var cart = await _cartRepository.GetUserCartAsync(userId) ?? await _cartRepository.CreateCartAsync(userId);

                var existingItemsCount = cart.CartItems.Sum(ci => ci.Quantity);
                if (existingItemsCount >= maxLimit)

                    return new ApiResponse<CartItemDto>(false, "Plant limit reached for your plan", null);

                var plant = await _plantRepository.GetPlantByIdAsync(plantId);
                if (plant == null)

                return new ApiResponse<CartItemDto>(false, "Plant not found", null);

                CartItem cartItem;

                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.PlantId == plantId);
                //int quantity;

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                    existingItem.Plant = plant;
                    //quantity = existingItem.Quantity;
                    await _cartRepository.UpdateCartItemAsync(existingItem);
                    cartItem = existingItem;
                }
                else
                {
                     cartItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        PlantId = plantId,
                        Quantity = 1,
                        Plant = plant
                    };
                    await _cartRepository.AddCartItemAsync(cartItem);
                }
                var mappedItem = _mapper.Map<CartItemDto>(cartItem);
                return new ApiResponse<CartItemDto>(true, "Item added to cart", mappedItem);
               
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartItemDto>(false, "Error adding to cart", null, ex.Message);
            }
        }
        public async Task<ApiResponse<string>> RemoveFromCartAsync(int userId, int plantId)
        {
            try
            {
                var cartItem = await _cartRepository.GetCartItemAsync(userId, plantId);
                if (cartItem == null)
                    return new ApiResponse<string>(false, "Item not found in cart", null);

                await _cartRepository.RemoveCartItemAsync(cartItem);
                return new ApiResponse<string>(true, "Removed from cart", "Removed from cart");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "Error removing from cart", null, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> IncreaseQuantityAsync(int userId, int plantId)
        {
            try
            {
                var userPlans = await _userPlanRepository.GetUserPlansByUserId(userId);
                var userPlan = userPlans.FirstOrDefault(up => up.IsActive);
                if (userPlan == null)
                    return new ApiResponse<string>(false, "User has no active plan", null);

                var maxLimit = userPlan.Plan.MaxPlantsAllowed ?? 0;

                var cart = await _cartRepository.GetUserCartAsync(userId);
                if (cart == null)
                    return new ApiResponse<string>(false, "Cart not found", null);

                var totalQuantity = cart.CartItems.Sum(ci => ci.Quantity);
                if (totalQuantity >= maxLimit)
                    return new ApiResponse<string>(false, "Maximum plant limit reached", null);

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.PlantId == plantId);
                if (cartItem == null)
                    return new ApiResponse<string>(false, "Item not found", null);

                cartItem.Quantity++;
                await _cartRepository.UpdateCartItemAsync(cartItem);
                return new ApiResponse<string>(true, "Quantity increased", "Quantity increased");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "Error increasing quantity", null, ex.Message);
            }
        }

        public async Task<ApiResponse<string>> DecreaseQuantityAsync(int userId, int plantId)
        {
            try
            {
                var cartItem = await _cartRepository.GetCartItemAsync(userId, plantId);
                if (cartItem == null)
                    return new ApiResponse<string>(false, "Item not found", null);

                cartItem.Quantity--;
                if (cartItem.Quantity <= 0)
                    await _cartRepository.RemoveCartItemAsync(cartItem);
                else
                    await _cartRepository.UpdateCartItemAsync(cartItem);

                return new ApiResponse<string>(true, "Quantity updated", "Quantity updated");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "Error decreasing quantity", null, ex.Message);
            }
        }
        public async Task<ApiResponse<string>> ReplaceCartWithBookingItemsAsync(int userId)
        {
            var userPlans = await _userPlanRepository.GetUserPlansByUserId(userId);
            var activePlan = userPlans.FirstOrDefault(p => p.IsActive && p.EndDate >= DateTime.UtcNow);
            if (activePlan == null)
                return new ApiResponse<string>(false, "No active plan found", null);

            var oldBooking = await _cartRepository.GetLastBookingWithPlantsAsync(userId, activePlan.StartDate, activePlan.EndDate);
            if (oldBooking == null || oldBooking.BookingItems == null)
                return new ApiResponse<string>(false, "No previous booking found", null);

            await _cartRepository.ClearCartAsync(userId);
            var cart = await _cartRepository.GetUserCartAsync(userId) ?? await _cartRepository.CreateCartAsync(userId);

            foreach (var item in oldBooking.BookingItems)
            {
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    PlantId = item.PlantId,
                    Quantity = item.Quantity
                };
                await _cartRepository.AddCartItemAsync(cartItem);
            }

            return new ApiResponse<string>(true, "Cart filled with previous booking plants", "Success");
        }

        public async Task<ApiResponse<string>> ClearCartAsync(int userId)
        {
            try
            {
                await _cartRepository.ClearCartAsync(userId);
                return new ApiResponse<string>(true, "Cart cleared", "Cart cleared");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "Error clearing cart", null, ex.Message);
            }
        }

    }
}
