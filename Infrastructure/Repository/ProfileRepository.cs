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
   public class ProfileRepository:IProfileRepository
    {
        private readonly AppdbContext _context;
        public ProfileRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task<User?>GetUserByIdAsync(int userId)
        {
            return await _context.users.Include(u => u.UserPlans).ThenInclude(up => up.Plan).FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<bool>UpdateUserAsync(User user)
        {
            _context.users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
