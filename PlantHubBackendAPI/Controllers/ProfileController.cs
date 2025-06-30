using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController:ControllerBase
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _profileService.GetProfile();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
        [HttpPatch("Update")]
        public async Task<IActionResult> UpfateProfile([FromForm] UpdateUserprofileDto dto)
        {
            var result = await _profileService.UpdateProfile(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
