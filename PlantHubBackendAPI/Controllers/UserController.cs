using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController:ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<ActionResult<List<UserViewDto>>> GetAllUser()
        {
            var user = await _userService.GetAllUsers();
            return Ok(user);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewDto>> GetUserById(int id)
        {
            var user = await _userService.GetUsersById(id);
            if (user == null)
                return NotFound("User Not Found");
            return Ok(user);
        }

        [HttpPatch("BlockUnBlockUser/{id}")]
        public async Task<ActionResult<BlockUnBlockDto>>TooggleBlock(int id)
        {
            var result = await _userService.BlockUnBlockUser(id);
            return Ok(result);
        }
    }
}
