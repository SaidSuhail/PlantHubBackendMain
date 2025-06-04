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
   public class PlanRepository:IPlanRepository
    {
        private readonly AppdbContext _context;
        public PlanRepository (AppdbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Plan>> GetAllAsync()
        {
           return await _context.Plans.ToListAsync();
        }
        public async Task<Plan?>GetByIdAsync(int id)
        {
           return await _context.Plans.FindAsync(id);
        }
        public async Task AddAsync(Plan plan)
        {
           await _context.Plans.AddAsync(plan);
        }
        public void Update(Plan plan)
        {
            _context.Plans.Update(plan);
        }
        public void Delete(Plan plan)
        {
           
                _context.Plans.Remove(plan);
            
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
