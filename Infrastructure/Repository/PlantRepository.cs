using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Model;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
   public class PlantRepository:IPlantRepository
    {
        private readonly AppdbContext _context;
        public PlantRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task AddPlantAsync(Plant plant)
        {
            await _context.Plants.AddAsync(plant);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Plant>> GetAllPlantsAsync()
        {
            return await _context.Plants.Include(p => p.Category).ToListAsync();
        }
        public async  Task<Plant?> GetPlantByIdAsync(int id)
        {
            return await _context.Plants.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<List<Plant>> GetPlantsByCategoryAsync(string categoryName)
        {
            if(categoryName.ToLower() == "all")
            {
                return await _context.Plants.Include(p => p.Category).ToListAsync();
            }
            return await _context.Plants.Include(p => p.Category).Where(p => p.Category.CategoryName.ToLower() == categoryName.ToLower()).ToListAsync();
        }
        public async Task<List<Plant>>SearchPlantsAsync(string keyword)
        {
            return await _context.Plants.Include(p => p.Category).Where(p => p.Name.ToLower().Contains(keyword.ToLower())).ToListAsync();
        }
        public async Task<bool>DeletePlantAsync(Plant plant)
        {
            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task UpdatePlantAsync(Plant plant)
        {
            _context.Plants.Update(plant);
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetTotalPlantCountAsync()
        {
            return await _context.Plants.CountAsync();
        }
        public async Task<List<Plant>> GetPaginatedPlantAsync(int pageNumber,int pageSize)
        {
            return await _context.Plants.Include(p => p.Category).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
