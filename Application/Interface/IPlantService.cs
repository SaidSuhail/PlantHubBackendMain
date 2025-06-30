using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interface
{
   public interface IPlantService
    {
        Task <ApiResponse<string>> AddPlant(AddPlantDto plantDto, IFormFile image);
        Task<ApiResponse<List<PlantWithCategoryDto>>> GetPlants();
        Task<ApiResponse<PlantWithCategoryDto?>> GetPlantById(int id);
        Task<ApiResponse<List<PlantWithCategoryDto>>> GetPlantsByCategory(string categoryName);
        Task<ApiResponse<bool>> DeletePlant(int id);
        Task <ApiResponse<string>> UpdatePlant(int id, UpdatePlantDto plantDto, IFormFile? image);
        Task<ApiResponse<List<PlantWithCategoryDto>>> SearchPlant(string search);
        Task<ApiResponse<PagedResponseDto<PlantViewDto>>> GetPaginatedPlants(int pageNumber, int pageSize);
    }
}
