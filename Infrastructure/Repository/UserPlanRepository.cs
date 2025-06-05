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
   public class UserPlanRepository:IUserPlanRepository
    {
        private readonly AppdbContext _context;
        public UserPlanRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task<List<UserPlan>> GetUserPlansByUserId(int userId)
        {
            return await _context.Userplans.Include(up => up.Plan).Where(up => up.UserId == userId).ToListAsync();
        }
        public async Task AddAsync(UserPlan userPlan)
        {
            await _context.Userplans.AddAsync(userPlan);
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<bool>HasActivePlanAsync(int userId)
        {
            return await _context.Userplans.AnyAsync(up => up.UserId == userId && up.IsActive);
        }
    }
}
