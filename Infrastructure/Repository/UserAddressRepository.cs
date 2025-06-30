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
   public class UserAddressRepository:IUserAddressRepository
    {
        private readonly AppdbContext _context;
        public UserAddressRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task<List<UserAddress>>GetUserAddressesAsync(int userId)
        {
            return await _context.UserAddress.Where(a => a.UserId == userId&&!a.IsDeleted).ToListAsync();
        }
        public async Task<UserAddress?>GetByIdAsync(int id)
        {
            return await _context.UserAddress.FindAsync(id);
        }
        public async Task<UserAddress>AddAsync(UserAddress address)
        {
            _context.UserAddress.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }
        public async Task<bool>DeleteAsync(int id)
        {
            var address = await _context.UserAddress.FindAsync(id);
            if (address == null) return false;
            address.IsDeleted = true;
            _context.UserAddress.Update(address);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
