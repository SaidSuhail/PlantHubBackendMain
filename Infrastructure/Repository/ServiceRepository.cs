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
   public class ServiceRepository:IServiceRepository
    {
        private readonly AppdbContext _context;
        public ServiceRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Service>> GetAllAsync()
        {
           return await _context.Services.ToListAsync();
        }
        public async Task<Service>GetByIdAsync(int id)
        {
           return await _context.Services.FindAsync(id);
        }
        public async Task<Service>AddAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }
        public async Task<Service>UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
            return service;
        }
        public async Task<bool>DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return false;
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Service>> GetServicesByIdsAsync(List<int> ids)
        {
            return await _context.Services
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }

    }
}
