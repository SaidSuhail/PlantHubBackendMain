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
    public class RoleRepository:IRoleRepository
    {
        private readonly AppdbContext _context;
        public RoleRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task<User?>GetUserByIdAsync(int userId)
        {
            return await _context.users.FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task ChangeUserRoleAsync(User user,UserRole newRole)
        {
            user.Role = newRole;
            _context.users.Update(user);
            if(newRole == UserRole.Provider)
            {
                var existingProvider = await _context.Providers.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if(existingProvider == null)
                {
                    var provider = new Provider
                    {
                        UserId = user.Id,
                        ProviderName = user.UserName,
                        ContactInfo = user.UserEmail,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };
                    _context.Providers.Add(provider);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
