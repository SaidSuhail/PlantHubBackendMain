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
        //[Authorize(Roles = "Admin")]
        [HttpPut("Change-Role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeRoleDTO changeRoleDTO)
        {
            try
            {

                if (changeRoleDTO == null || string.IsNullOrWhiteSpace(changeRoleDTO.NewRole))
                {
                    return BadRequest(new ApiResponse<string>(false, "invalid input data", null));
                }
                var result = await _roleService.ChangeUserRole(changeRoleDTO);
                if (!result)
                {
                    return NotFound(new ApiResponse<string>(false, "User not found or role update failed", null));
                }
                return Ok(new ApiResponse<string>(true, "Role Updated successfully", null));
                }
                catch(Exception ex)
                {
                    return StatusCode(500, new ApiResponse<string>(false, "Server error", null,ex.Message));
                }
        }
    }
}
