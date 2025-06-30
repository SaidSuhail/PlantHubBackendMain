using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PlansController:ControllerBase
    {
        private readonly IPlanService _planService;
        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PlanDto>>>> GetAll()
        {
           var response = await _planService.GetAllPlansAsync();
            return StatusCode(response.Success ? 200 : 500, response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PlanDto>>>GetById(int id)
        {
            var response = await _planService.GetPlanByIdAsync(id);
            return StatusCode(response.Success ? 200 : 400,response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PlanDto>>>Create(PlanDto planDto)
        {
            var response = await _planService.CreatePlanAsync(planDto);
            if (!response.Success)
            {
                return StatusCode(500, response);
            }
            return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponse<PlanDto>>>Update(int id, [FromBody]UpdatePlanDto updateDto)
        {
            var response = await _planService.UpdatePlanAsync(id, updateDto);
            return StatusCode(response.Success ? 200 : 400, response);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
           var response =  await _planService.DeletePlanAsync(id);
            return StatusCode(response.Success ? 200 : 400, response);
        }

    }
}
