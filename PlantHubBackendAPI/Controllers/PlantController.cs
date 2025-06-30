using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;
        public PlantController(IPlantService plantService)
        {
            _plantService = plantService;
        }

        [Authorize (Roles ="Admin,Provider")]
        [HttpPost("AddPlant")]
        public async Task<IActionResult> AddPlant([FromForm] AddPlantDto dto, IFormFile image)
        {
          var result =   await _plantService.AddPlant(dto, image);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        [HttpGet("GetAllPlants")]
        public async Task<IActionResult> GetAllPlants()
        {
            var result  = await _plantService.GetPlants();
            return StatusCode(result.Success ? 200 : 404, result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult>GetById(int id)
        {
            var result  = await _plantService.GetPlantById(id);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        [HttpGet("category/{categoryName}")]
        public async Task<IActionResult>GetByCategory(string categoryName)
        {
            var result = await _plantService.GetPlantsByCategory(categoryName);
            return StatusCode(result.Success ? 200 : 404, result);
        }
        [Authorize(Roles ="Admin,Provider")]
        [HttpDelete("{id}")]
        public async Task<IActionResult>Delete(int id)
        {
            var result = await _plantService.DeletePlant(id);
            return StatusCode(result.Success ? 200 : 404, result);
        }
        [Authorize(Roles ="Admin,Provider")]
        [HttpPatch("{id}")]
        public async Task<IActionResult>Update(int id, [FromForm]UpdatePlantDto dto ,IFormFile? image)
        {
           var result =  await _plantService.UpdatePlant(id, dto, image);
            return StatusCode(result.Success ? 200 : 400, result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var result = await _plantService.SearchPlant(keyword);
            return StatusCode(result.Success ? 200 : 404, result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 10)
        {
            var result = await _plantService.GetPaginatedPlants(pageNumber, pageSize);
            return StatusCode(result.Success ? 200 : 404, result);
        }
    }

}
