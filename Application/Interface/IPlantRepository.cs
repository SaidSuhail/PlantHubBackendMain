using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface IPlantRepository
    {
        Task AddPlantAsync(Plant plant);
        Task<List<Plant>> GetAllPlantsAsync();
        Task<Plant?> GetPlantByIdAsync(int id);
        Task<List<Plant>> GetPlantsByCategoryAsync(string categoryName);
        Task<List<Plant>> SearchPlantsAsync(string Keyword);
        Task<bool> DeletePlantAsync(Plant plant);
        Task UpdatePlantAsync(Plant plant);
        Task<int> GetTotalPlantCountAsync();
        Task<List<Plant>> GetPaginatedPlantAsync(int pageNumber, int pageSize);
    }
}
