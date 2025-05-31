using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody]UserRegisterDto newUser)
        {
            try
            {
                bool isDone = await _authService.Register(newUser);
                if (!isDone)
                {
                    return BadRequest(new ApiResponse<string>(false, "User Already Exist", "[]"));
                }
                return Ok(new ApiResponse<string>(true, "User Registered Successfully", "[]"));
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Server Error", null,ex.Message));
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserLoginDto login)
        {
            try
            {
                var res = await _authService.Login(login);
                if (res.Error == "Not found")
                    return NotFound(new ApiResponse<string>(false, "Email is not verified","[]"));
                if (res.Error == "Invalid password")
                    return BadRequest(new ApiResponse<string>(false, "Invalid password", "[]"));
                if (res.Error == "User Blocked")
                    return StatusCode(403, new ApiResponse<string>(false, "User is blocked by admin", "[]"));
                return Ok(new ApiResponse<object>(true, "Login Successfull", res));
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, "Server Error", null,ex.Message));
            }
        }
    }
}
