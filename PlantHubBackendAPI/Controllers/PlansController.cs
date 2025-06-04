using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController:ControllerBase
    {
        private readonly IPlanService _planService;
        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanDto>>> GetAll()
        {
            var plans = await _planService.GetAllPlansAsync();
            return Ok(plans);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanDto>>GetById(int id)
        {
            var plan = await _planService.GetPlanByIdAsync(id);
            if (plan == null) return NotFound();
            return Ok(plan);
        }
        [HttpPost]
        public async Task<ActionResult<PlanDto>>Create(PlanDto planDto)
        {
            var createdPlan = await _planService.CreatePlanAsync(planDto);
            return CreatedAtAction(nameof(GetById), new { id = createdPlan.Id }, createdPlan);
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult>Update(int id, [FromBody]UpdatePlanDto updateDto)
        {
            var updatedPlan = await _planService.UpdatePlanAsync(id, updateDto);
            if (updatedPlan == null) return NotFound();
            return Ok(updatedPlan);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _planService.DeletePlanAsync(id);
            return NoContent();
        }

    }
}
