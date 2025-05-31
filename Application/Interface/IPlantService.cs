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
        Task AddPlant(AddPlantDto plantDto, IFormFile image);
        Task<List<PlantWithCategoryDto>> GetPlants();
        Task<PlantWithCategoryDto?> GetPlantById(int id);
        Task<List<PlantWithCategoryDto>> GetPlantsByCategory(string categoryName);
        Task<bool> DeletePlant(int id);
        Task UpdatePlant(int id, UpdatePlantDto plantDto, IFormFile? image);
        Task<List<PlantWithCategoryDto>> SearchPlant(string search);
        Task<PagedResponseDto<PlantViewDto>> GetPaginatedPlants(int pageNumber, int pageSize);
    }
}
