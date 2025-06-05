using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    public class UserPlanController:ControllerBase
    {
        private readonly IUserPlanService _userPlanService;
        public UserPlanController(IUserPlanService userPlanService)
        {
            _userPlanService = userPlanService;
        }
        [HttpPost("subscribe")]
        public async Task<IActionResult>Subscribe(AddUserPlanDto dto)
        {
            var result = await _userPlanService.SubscribePlan(dto);
            return Ok(result);
        }
        [HttpPost("Unsubscribe")]
        public async Task<IActionResult>UnSubscribe(int userId)
        {
            var response = await _userPlanService.UnsubscribePlan(userId);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult>DetPlansForUser(int userId)
        {
            var result = await _userPlanService.GetUsersPlan(userId);
            return Ok(result);
        }
    }
}
