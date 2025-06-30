using System.Linq.Expressions;
using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RoleController:ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("Change-Role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeRoleDTO changeRoleDTO)
        {
                if (changeRoleDTO == null || string.IsNullOrWhiteSpace(changeRoleDTO.NewRole))
                {
                    return BadRequest(new ApiResponse<string>(false, "invalid input data", null));
                }
                var response = await _roleService.ChangeUserRole(changeRoleDTO);
                if (!response.Success)
                {
                    if (response.Message == "User Not Found") 
                        return NotFound(response);
                    return BadRequest(response);
                }
                return Ok(response);
            
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("providers")]
        public async Task<IActionResult> GetAllProviders()
        {
            var response = await _roleService.GetAllProvidersAsync();
            return Ok(response);
        }

    }
}
