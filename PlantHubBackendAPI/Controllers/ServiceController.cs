using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController:ControllerBase
    {
        private readonly IServiceService _service;
        public ServiceController(IServiceService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(new { success = true, message = "fetched", data = await _service.GetAllServicesAsync() });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult>GetById(int id)
        {
            var result = await _service.GetServiceByIdAsync(id);
            return result != null ? Ok(new { success = true, data = result })
                : NotFound(new { succeess = false, message = "service not found" });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateServiceDto dto)
        {
            var created = await _service.CreateServiceAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, new { success = true, data = created });
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult>Update(int id, [FromBody]UpdateServiceDto dto)
        {
            var updated = await _service.UpdateServiceAsync(id, dto);
            return updated != null ? Ok(new { success = true, data = updated }) : NotFound(new { success = false, message = "service not fund" });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult>Delete(int id)
        {
            var deleted = await _service.DeleteServiceAsync(id);
            return deleted ? Ok(new { success = true, message = "service deleted" })
                : NotFound(new { success = false, message = "service not found" });
        }
    }
}
