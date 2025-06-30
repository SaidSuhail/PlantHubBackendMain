using Application.DTOs;
using System.Security.Claims;
using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class RoleChangeRequestController:ControllerBase
    {
        private readonly IRoleChangeRequestService _requestService;
        public RoleChangeRequestController(IRoleChangeRequestService requestService)
        {
            _requestService = requestService;
        }
        [Authorize]
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitRequest([FromBody] RequestRoleChangeDTO dto)
        {
            // You can trust UserId from token instead of client
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // Override dto.UserId with authenticated user's ID for security
            dto.UserId = userId;

            var response = await _requestService.SubmitRequestAsync(dto);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var response = await _requestService.GetPendingRequestsAsync();
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("review")]
        public async Task<IActionResult> ReviewRequest([FromBody] ReviewRoleChangeRequestDTO dto)
        {
            var reviewer = User.Identity?.Name ?? "Admin";

            var response = await _requestService.ReviewRequestAsync(dto, reviewer);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRequests()
        {
            var response = await _requestService.GetAllRequestsAsync();
            return Ok(response);
        }

    }
}
