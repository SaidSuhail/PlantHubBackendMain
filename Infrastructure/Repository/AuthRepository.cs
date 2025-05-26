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
   public class AuthRepository:IAuthRepository
    {
        private readonly AppdbContext _context;
        public AuthRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task<User>GetUserByEmail(string email)
        {
            return await _context.users.FirstOrDefaultAsync(u => u.UserEmail == email);
        }
        public async Task<bool> IsUserExist(string email)
        {
            return await _context.users.AnyAsync(u => u.UserEmail == email);
        }
        public async Task<bool> IsUserNameExist(string userName)
        {
            return await _context.users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
        public async Task<bool> IsPhoneExist(string phone)
        {
            return await _context.users.AnyAsync(u => u.Phone == phone);
        }

        public async Task AddUser(User user)
        {
            _context.users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
