using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAddressController:ControllerBase
    {
       
        private readonly IUserAddressService _service;
        public UserAddressController(IUserAddressService service)
        {
            _service = service;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult>GetAddresses(int userId)
        {
            var addresses = await _service.GetUserAddressesAsync(userId);
            return Ok(addresses);
        }
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody]CreateUserAddressDto dto)
        {
            var created = await _service.AddUserAddressAsync(dto);
            return CreatedAtAction(nameof(GetAddresses), new { userId = dto.UserId }, created);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult>Delete(int id)
        {
            var success = await _service.DeleteAddressAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
