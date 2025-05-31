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
   public class CategoryRepository:ICategoryRepository
    {
        private readonly AppdbContext _context;
        public CategoryRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>>GetAll()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category?>GetByName(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == name);
        }
        public async Task Add(Category category)
        {
            await _context.Categories.AddAsync(category);
        }
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Category?>GetById(int id)
        {
            return await _context.Categories.FindAsync(id);
        }
        public async Task Delete(Category category)
        {
            _context.Categories.Remove(category);
        }
        public async Task<List<Plant>> GetPlantsByCategoryId(int categoryId)
        {
            return await _context.Plants.Where(p => p.CategoryId == categoryId).ToListAsync();
        }
        // ✅ New Method
        public void RemoveProducts(List<Plant> products)
        {
            _context.Plants.RemoveRange(products);
        }

        // ✅ Optional alias
        public void RemoveCategory(Category category)
        {
            _context.Categories.Remove(category);
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId);
        }
    }
}
