using Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace PlantHubBackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController:ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetCategories();
            if (!response.Success)
                return StatusCode(500, response);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody]CategoryAddDto category)
        {
            var response = await _categoryService.AddCategory(category);
            if (!response.Success)
                return BadRequest(response);
            return CreatedAtAction(nameof(GetAllCategories), null, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult>DeleteCategory(int id)
        {
            var response = await _categoryService.RemoveCategory(id);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

    }
}
