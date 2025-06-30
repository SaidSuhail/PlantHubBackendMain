using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
   public class UserRepository:IUserRepository
    {
        private readonly AppdbContext _context;
        public UserRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task<List<User>> GetAllUsers()
        {
            return await _context.users.Where(u => u.Role != UserRole.Admin).ToListAsync();
        }
        public async Task<User?>GetUserById(int id)
        {
            return await _context.users.FirstOrDefaultAsync(u => u.Id == id && u.Role != UserRole.Admin);
        }
        public async Task<User> BlockUnBlockUser(int id)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                throw new Exception("User Not Found");
            user.IsBlocked = !user.IsBlocked;
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
