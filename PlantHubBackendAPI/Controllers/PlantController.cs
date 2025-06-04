//using Application.DTOs;
//using Application.Interface;
//using Domain.Model;
//using Microsoft.AspNetCore.Mvc;

//namespace PlantHubBackendAPI.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class PlantController:ControllerBase
//    {
//        private readonly IPlantService _plantService;
//        public PlantController(IPlantService plantService)
//        {
//            _plantService = plantService;
//        }
//        [HttpPost("Add plants")]
//        public async Task<IActionResult> AddPlant([FromBody]AddPlantDto dto,IFormFile image)
//        {
//            await _plantService.AddPlant(dto, image);
//            return Ok("Plant Added Successfully");
//        }
//        [HttpGet("Get All Plants")]
//        public async Task<IActionResult> GetAllPlants()
//        {
//          var plants =   await _plantService.GetPlants();
//            return Ok(plants);
//        }
//    }
//}
using Application.DTOs;
using Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PlantHubBackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;
        public PlantController(IPlantService plantService)
        {
            _plantService = plantService;
        }

       
        [HttpPost("AddPlant")]
        public async Task<IActionResult> AddPlant([FromForm] AddPlantDto dto, IFormFile image)
        {
            await _plantService.AddPlant(dto, image);
            return Ok("Plant Added Successfully");
        }


        [HttpGet("GetAllPlants")]
        public async Task<IActionResult> GetAllPlants()
        {
            var plants = await _plantService.GetPlants();
            return Ok(plants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>GetById(int id)
        {
            var plant = await _plantService.GetPlantById(id);
            return plant != null ? Ok(plant) : NotFound("Plant not found");
        }
        [HttpGet("category/{categoryName}")]
        public async Task<IActionResult>GetByCategory(string categoryName)
        {
            var plants = await _plantService.GetPlantsByCategory(categoryName);
             return Ok(plants);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult>Delete(int id)
        {
            var success = await _plantService.DeletePlant(id);
            return success ? Ok("Deleted successfully") : NotFound("Plant not found");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult>Update(int id, [FromForm]UpdatePlantDto dto ,IFormFile? image)
        {
            // Log incoming values to check what is bound
            Console.WriteLine("Name: " + dto.Name);
            Console.WriteLine("LatinName: " + dto.LatinName);
            Console.WriteLine("Description: " + dto.Description);
            Console.WriteLine("Price: " + dto.Price);
            Console.WriteLine("Color: " + dto.Color);
            Console.WriteLine("CategoryId: " + dto.CategoryId);
            Console.WriteLine("ProviderId: " + dto.ProviderId);
            Console.WriteLine("Stock: " + dto.Stock);
            await _plantService.UpdatePlant(id, dto, image);
            return Ok("Updated successfully");
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var plants = await _plantService.SearchPlant(keyword);
            return Ok(plants);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 10)
        {
            var plants = await _plantService.GetPaginatedPlants(pageNumber, pageSize);
            return Ok(plants);
        }
    }

}
